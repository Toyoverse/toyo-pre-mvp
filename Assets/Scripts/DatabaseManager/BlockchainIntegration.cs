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
            PlayerPrefs.SetString("TokenJWT", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ3YWxsZXRJZCI6IjB4ZDZkNmU2NmJjNjAxOTQ0MDgzYzA0ZWJjZDEyYTIxMjc1MTU3MWY3MCIsInRyYW5zYWN0aW9uIjoiMHgyNDI3YTQ5ZmQ0ZWY1MDg0OTZkMjhhNGE2YTAxNGI5MTJjNzMwMDE5NmU5N2E0OTgxMmQ2MWJiOTJlMTM2NzRiMzRhMDQxZmFjMGEzNGRiNzM0ZjE0OTdjZTFlYzJlM2FlYmE3MmFhNTljNmY3MTYxOGI2YjY2ZWUyMzcwMWFmNTFjIiwiaWF0IjoxNjYwNTg3MzQzLCJleHAiOjE2NjExOTIxNDN9.U9DQqQqkNsqzfbEl9f5VZVmmK72TDPpkfgqkRgvVB3s");
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

        ApproveNftTransfer(_myBox);
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
        string gasPrice = "8000000";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {
            Debug.Log("Debugging opened box transaction parameters.");
            Debug.Log(_typeId);
            Debug.Log(_code);
            Debug.Log(_signature);
            Debug.Log(_tokenId);
            
            string tx = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
            Debug.Log(tx);
            
            // string txStatus = "pending";
            // while (txStatus == "pending")
            // {
            //     string chain = "polygon";
            //     string network = "testnet";
            //     txStatus = await EVM.TxStatus(chain, network, tx);
            //     Debug.Log(txStatus);
            //     new WaitForSeconds(1.5f);
            // }
            //
            // Debug.Log(txStatus);
            // Debug.Log(tx);
            //
            // if(txStatus == "success")
            
            _databaseConnection.PostOpenBox(ScreenManager.Instance.boxInfoScript.CallOpenBoxAnimation, 
                ScreenManager.Instance.boxInfoScript.GetBoxSelected().boxList[0].objectId);
        } catch (Exception e) {
            Debug.LogException(e, this);
        }

    }

    private async void ApproveNftTransfer(OpenBox myBox)
    {
        // APPROVE
        string approve_abi = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        
        // NftToken (Mumbai)
        string approve_contract = "0xc02173691984D68625C455e0AB45f52581c008Da";
        string approve_method = "approve";
        
        // NftTokenSwap (Mumbai)
        string _approved_to = "0x53904b4640474d2f79b822ad4e2c40597d886bd5";
        string _approved_tokenId = myBox.tokenId;

        string[] approve_obj = { _approved_to, _approved_tokenId };
        string approve_args = JsonConvert.SerializeObject(approve_obj);

        string approve_value = "0";
        string approve_gasLimit = "";
        string approve_gasPrice = "8000000";

        try {
            Debug.Log("DEBUGGING");
            Debug.Log(approve_contract);
            Debug.Log(approve_method);
            Debug.Log(_approved_to);
            Debug.Log(_approved_tokenId);
            Debug.Log(approve_obj);
            Debug.Log(approve_args);
            Debug.Log(approve_value);
            Debug.Log(approve_gasLimit);
            Debug.Log(approve_gasPrice);
            
            string tx = await Web3GL.SendContract(approve_method, approve_abi, approve_contract, approve_args, approve_value, approve_gasLimit, approve_gasPrice);
            
            // string txStatus = "pending";
            // while (txStatus == "pending")
            // {
            //     string chain = "polygon";
            //     string network = "testnet";
            //     txStatus = await EVM.TxStatus(chain, network, tx);
            //     Debug.Log(txStatus);
            //     new WaitForSeconds(1.5f);
            // }
            //
            // Debug.Log(txStatus);
            // Debug.Log(tx);
            // if(txStatus == "success")
                OpenBox(myBox);

        } catch (Exception e) {
            Debug.LogException(e, this);
        }
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