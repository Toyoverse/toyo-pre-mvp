using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database;
using Newtonsoft.Json;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class BlockchainIntegration : MonoBehaviour
{
    public bool skipLogin;
    public Toggle rememberMe;
    
    [DllImport("__Internal")]
    private static extern void Web3Connect();
    [DllImport("__Internal")]
    private static extern string ConnectAccount();
    
    private DatabaseConnection _databaseConnection => DatabaseConnection.Instance;
    private string _account;
    private List<Toyo> _toyoList = new();

    public async void StartLoginMetamask()
    {
        Loading.StartLoading?.Invoke();
        if (skipLogin)
        {
            PlayerPrefs.SetString("TokenJWT", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ3YWxsZXRJZCI6IjB4ZmY2NTRhYTAyMDBlNTUxZGJlOWMwMGM4YjVjMWY0ZTg4ZDc0ZmNjMyIsInRyYW5zYWN0aW9uIjoiMHhmZjY1NGFhMDIwMGU1NTFkYmU5YzAwYzhiNWMxZjRlODhkNzRmY2MzIiwiaWF0IjoxNjU3OTE1MTE3LCJleHAiOjE2NTg1MTk5MTd9.vxOiDNXs6lnCovzSyHfm3DfWMOaQIRkNSdU4gBXIvSg");
            CallDatabaseConnection();
            return;
        }
        
        var _message = ((int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds)
            .ToString();
        var _signature = "";
        
        #if (UNITY_WEBGL || UNITY_WEBGL_API) && !UNITY_EDITOR
        try 
        {
            Web3Connect();
            
            string account;
            account = ConnectAccount();
            while (account == "") {
                await new WaitForSeconds(1f);
                account = ConnectAccount();
            };

            _signature = await Web3GL.Sign(_message);
        } 
        catch (Exception e) 
        {
            Debug.LogException(e, this);
        }
        #else
        try 
        {
            _signature = await Web3Wallet.Sign(_message);
            Debug.Log(_signature);
        } 
        catch (Exception e) 
        {
            Debug.LogException(e, this);
        }
        #endif
        _account = await EVM.Verify(_message, _signature);

        var _parameterList = new[]
        {
            ("walletAddress", _account),
            ("transactionHash", _signature)
        }.ToList();

        _databaseConnection.CallLogin(CallBackLoginWebConnection, _parameterList);
    }

    private void CallBackLoginWebConnection(string json)
    {
        var _loginCallback = JsonUtility.FromJson<LoginCallback>(json);
        if (_account.Length == 42 && _loginCallback.token.Length > 0)
        {
            PlayerPrefs.SetString("Account", _account);
            PlayerPrefs.SetString("TokenJWT", _loginCallback.token);
            //PlayerPrefs.SetInt("RememberMe", rememberMe.isOn ? 1 : 0);
        }

        CallDatabaseConnection();
    }

    private void CallDatabaseConnection()
    {
        Loading.EndLoading += GoToNextScreen;
        _databaseConnection.CallGetPlayerBoxes(OnBoxesSuccess);
    }
    
    public void OnBoxesSuccess(string json)
    {
        var _myObject = JsonUtility.FromJson<DatabasePlayerJson>(json);

        ToyoManager.SetPlayerData(_myObject.player);
        ToyoManager.InitializeBoxes();
        ToyoManager.SetPlayerBoxes();
        _databaseConnection.CallGetPlayerToyo(OnToyoListSuccess);
    }

    public void CallOpenBox(string json)
    {
        var _myBox = JsonUtility.FromJson<OpenBox>(json);
        
        OpenBox(_myBox);
    }

    public async void OpenBox(OpenBox myBox)
    {
        // abi in json format
        string abi = "[{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_typeId\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"_code\",\"type\":\"string\"},{\"internalType\":\"bytes\",\"name\":\"_signature\",\"type\":\"bytes\"}],\"name\":\"swapToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        
        // address of contract
        var contract = "0x53904b4640474d2f79b822ad4e2c40597d886bd5"; //TODO: REMOVE HARD CODED CONTRACT

        // smart contract method to call
        string method = "swapToken";

        // array of arguments for contract
        var _typeId = myBox.typeId;
        var _code = myBox.toyoHash;
        var _signature = myBox.toyoSignature;
        var _tokenId = myBox.tokenId;
        
        string[] obj = { _tokenId, _typeId, _code, _signature };
        string args = JsonConvert.SerializeObject(obj);

        // value in wei
        string value = "0";
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try {
            string tx = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
            Debug.Log(tx);
        } catch (Exception e) {
            Debug.LogException(e, this);
        }

        _databaseConnection.PostOpenBox(ScreenManager.Instance.boxInfoScript.CallOpenBoxAnimation, 
            ScreenManager.Instance.boxInfoScript.GetBoxSelected().boxList[0].objectId);
    }

    public void OnToyoListSuccess(string json)
    {
        var _myObject = JsonUtility.FromJson<CallbackToyoList>(json);
        if (_myObject.toyos == null || _myObject.toyos.Length == 0)
        {
            _myObject.toyos = Array.Empty<Toyo>();    
        }
        ToyoManager.Instance.Player.toyos = _myObject.toyos;
        StartCoroutine(_databaseConnection.CallGetToyoData(OnToyoDetailSuccess, _myObject.toyos));
    }

    public void OnToyoDetailSuccess(string json)
    {
        var _myObject = JsonUtility.FromJson<CallbackToyoDetails>(json);
        _toyoList.Add(_myObject.toyo);
        UpdateToyoDetails(_myObject.toyo);
    }

    private void UpdateToyoDetails(Toyo toyoWithDetails)
    {
        var _databaseToyoList = ToyoManager.Instance.Player.toyos;
        var _toyoListIndex = _databaseToyoList.TakeWhile(toyo => !toyo.objectId.Equals(toyoWithDetails.objectId)).Count();
        _databaseToyoList[_toyoListIndex] = toyoWithDetails;
    }

    public void RefreshButton()
    {
        if (Loading.InLoading)
            return;
        switch (ScreenManager.ScreenState)
        {
            case ScreenState.None:
            case ScreenState.Welcome:
            case ScreenState.LoreTheme:
            case ScreenState.Unboxing:
            case ScreenState.Metamask:
            case ScreenState.TrainingModuleRewards:
                break;
            default:
                GenericPopUp.Instance.ShowPopUp("Are you sure?", YesRefresh, () => { });
                break;
        }
    }

    private void YesRefresh()
    {
        Loading.StartLoading?.Invoke();
        _databaseConnection.CallGetPlayerToyo(OnToyoListSuccess);
        Debug.Log("Refresh!");
    }

    private void GoToNextScreen()
    {
        ScreenManager.Instance.GoToScreen(ToyoManager.Instance.ToyoList.Count > 0
            ? ScreenState.MainMenu
            : ScreenState.Welcome);
    }
}