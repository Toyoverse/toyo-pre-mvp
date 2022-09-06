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
using UnityEngine.Serialization;

public class BlockchainIntegration : MonoBehaviour
{
    #region Production Ambient BlockchainInformation

    public bool isProduction;
    
    //BOXES
    private const string productionApproveContractForTokenUntil9476 = "0x07AE3987C679c0aFd2eC1ED2945278c37918816c";
    private const string productionApproveContractForTokenOver9476 = "0x5c29302b5ae9e99f866704e28528d5be9b7b6a40";
    private const string productionApprovedTo = "0xB86743535e2716E2cea0D285A3fc3c1A58e44318";

    private const string productionSwapTokenContract = "0xB86743535e2716E2cea0D285A3fc3c1A58e44318";

    #endregion

    #region Test Ambient BlockchainInformation

    private const string testApproveContract = "0xc02173691984D68625C455e0AB45f52581c008Da";
    private const string testApprovedTo = "0x53904b4640474d2f79b822ad4e2c40597d886bd5";
    private const string testSwapTokenContract = "0x53904b4640474d2f79b822ad4e2c40597d886bd5";
    
    #endregion

    #region Toyos contracts

    private const string testToyoApproved = "0xb9F84081B4a621C819f8D206036F7548aa06638a";
    private const string testToyoApprovedTo = "0x39a66bb85ec5f0ba8572b6e2452f78b6301843d1";

    private const string productionToyoApproveContractForTokenOver9476 = "0xaf5107e0a3Ea679B6Fc23A9756075559e2e4649b";
    private const string productionToyoApprovedTo = ""; //TODO

    #endregion
    
    public bool skipLogin;
    public string tokenForTests;
    public Toggle rememberMe;
    
    [DllImport("__Internal")]
    private static extern void Web3Connect();
    [DllImport("__Internal")]
    private static extern string ConnectAccount();
    
    private DatabaseConnection _databaseConnection => DatabaseConnection.Instance;
    private string _account;
    private List<Toyo> _toyoList = new();

    private readonly string _openBoxFailMessage = "Metamask Transaction Fail. Click to open box again.";
    private readonly string _genericFailMessage = "Metamask Transaction Fail. Try again.";

    public async void StartLoginMetamask()
    {
        Loading.StartLoading?.Invoke();
        if (skipLogin)
        {
            PlayerPrefs.SetString("TokenJWT", tokenForTests);
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
            Loading.EndLoading?.Invoke();
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
            Loading.EndLoading?.Invoke();
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
        Debug.Log("Boxes update success!");
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
        var contract = isProduction ? productionSwapTokenContract : testSwapTokenContract;

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
        string gasPrice = "80000000000";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {
            
            string tx = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
            
            _databaseConnection.PostOpenBox(ScreenManager.Instance.boxInfoScript.CallOpenBoxAnimation, 
                ScreenManager.Instance.boxInfoScript.GetBoxSelected().GetFirstUnopenedBoxId());
            
        } catch (Exception e) {
            Debug.LogException(e, this);
            Loading.EndLoading?.Invoke();
        }

    }

    private async void ApproveNftTransfer(OpenBox myBox)
    {
        // APPROVE
        string approve_abi = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        
        // NftTokenSwap (Mumbai)
        string _approved_to = isProduction ? productionApprovedTo : testApprovedTo;
        string _approved_tokenId = myBox.tokenId;
        
        // NftToken (Mumbai)
        string approve_contract = "";
        if (isProduction)
            approve_contract = int.Parse(_approved_tokenId) <= 9476
                ? productionApproveContractForTokenUntil9476
                : productionApproveContractForTokenOver9476;
        else
            approve_contract = testApproveContract;
        
        string approve_method = "approve";

        string[] approve_obj = { _approved_to, _approved_tokenId };
        string approve_args = JsonConvert.SerializeObject(approve_obj);

        string approve_value = "0";
        string approve_gasLimit = "";
        string approve_gasPrice = "80000000000";

        try {

            string tx = await Web3GL.SendContract(approve_method, approve_abi, approve_contract, approve_args, approve_value, approve_gasLimit, approve_gasPrice);
            
            OpenBox(myBox);

        } catch (Exception e) {
            Debug.LogException(e, this);
            Loading.EndLoading?.Invoke();
            GenericPopUp.Instance.ShowPopUp(_openBoxFailMessage);
        }
    }
    
    public async void ToyoApproveNftTransfer(string tokenID)
    {
        // APPROVE
        const string approveABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

        // NftTokenSwap (Mumbai)
        var _approvedTo = isProduction ? productionToyoApprovedTo : testToyoApprovedTo;

        // NftToken (Mumbai)
        var _approveContract = "";
        if (isProduction)
            _approveContract = int.Parse(tokenID) <= 9476
                ? productionApproveContractForTokenUntil9476
                : productionToyoApproveContractForTokenOver9476;
        else
            _approveContract = testToyoApproved;
        
        const string approveMethod = "approve";

        string[] _approveObj = { _approvedTo, tokenID };
        var _approveArgs = JsonConvert.SerializeObject(_approveObj);

        const string approveValue = "0";
        const string approveGasLimit = "";
        const string approveGasPrice = "80000000000";
        
        Debug.Log("SendContractValues { approveMethod:" + approveMethod + ", approveABI: " + approveABI 
                  + ", _approveContract: " + _approveContract + ", _approveArgs: " + _approveArgs + ", approveValue: "
                  + approveValue + ", approveGasLimit: " + approveGasLimit + ", approveGasPrice: " + approveGasPrice);

        try {

            var _hash = await Web3GL.SendContract(approveMethod, approveABI, _approveContract, _approveArgs, approveValue, approveGasLimit, approveGasPrice);
            Debug.Log("TransactionHash: " + _hash);
            
            ToyoStake(tokenID);

        } catch (Exception _exception) {
            Debug.LogException(_exception, this);
            Loading.EndLoading?.Invoke();
            GenericPopUp.Instance.ShowPopUp(_genericFailMessage);
        }
    }

    public async void ToyoStake(string tokenID)
    {
        // abi in json format
        var _abi = "[{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"stakeToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        
        // address of contract
        var _contract = isProduction ? productionToyoApprovedTo : testToyoApprovedTo;

        // smart contract method to call
        var _method = "stakeToken";

        string[] _obj = { tokenID };
        var _args = JsonConvert.SerializeObject(_obj);

        // value in wei
        var _value = "0";
        // gas limit OPTIONAL
        var _gasLimit = "";
        // gas price OPTIONAL
        var _gasPrice = "80000000000";
        // connects to user's browser wallet (metamask) to update contract state
        
        Debug.Log("SendContractValues { _abi:" + _abi + ", _contract: " + _contract + ", _method: " + _method 
                  + ", _args: " + _args + ", _value: " + _value + ", _gasLimit: " + _gasLimit + ", _gasPrice: " + _gasPrice);
        
        try
        {
            var _hash = await Web3GL.SendContract(_method, _abi, _contract, _args, _value, _gasLimit, _gasPrice);
            Debug.Log("StakeHash: " + _hash);
            ScreenManager.Instance.trainingModuleScript.SendToyoToTraining();

        } catch (Exception e) {
            Debug.LogException(e, this);
            Loading.EndLoading?.Invoke();
            GenericPopUp.Instance.ShowPopUp(_genericFailMessage);
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
        Debug.Log("Toyo Details Success: " + _myObject.toyo.name);
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
        _databaseConnection.CallGetPlayerBoxes(OnBoxesSuccess);
        ScreenManager.Instance.GetActualScreen().CallUpdateUI();
        Debug.Log("Refresh clicked!");
    }

    private void GoToNextScreen()
    {
        ScreenManager.Instance.GoToScreen(ToyoManager.Instance.ToyoList.Count > 0
            ? ScreenState.MainMenu
            : ScreenState.Welcome);
    }
}