using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Newtonsoft.Json;
using UI;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Tools;
using UnityEngine.Serialization;

public class BlockchainIntegration : MonoBehaviour
{
    #region Production Ambient BlockchainInformation

    public bool isProduction;
    public bool enableLogsInProduction;
    
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
    private const string productionToyoApprovedTo = "0xd44ad19885a9a20dbd3f7022409804d8636a8243"; 

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
    private readonly string _stakeInitializeMessage = "The stake transaction has started, check back later to view the training progress." +
                                                      "This may take a while as it depends on the network.";
    private readonly string _claimInitializeMessage = "Claim transaction has started, check back later to see your reward. " +
                                                      "This may take a while as it depends on the network.";
    private readonly string _transactionRefusedMessage = "Transaction on metamask was declined or failed for some reason, please try again.";
    private readonly string _transactionUnderpriced = "Undervalued transaction, please try again with a slightly higher gas fee.";
    private readonly string _approveInitializeMessage = "The transaction to approve Toyo's stake has started, " +
                                                        "check the progress in your Metamask and come back later when the transaction " +
                                                        "is complete to continue your Toyo's stake.";

    private readonly string _baseGasPrice = "120" + "000000000"; //120 GWEI 

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
            Print.LogException(e, this);
            Loading.EndLoading?.Invoke();
        }
        #else
        try 
        {
            _signature = await Web3Wallet.Sign(_message);
            Print.Log(_signature);
        } 
        catch (Exception e) 
        {
            Print.LogException(e, this);
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
        Print.Log("Boxes update success!");
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

        // connects to user's browser wallet (metamask) to update contract state
        try
        {
            
            string tx = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, _baseGasPrice);
            
            _databaseConnection.PostOpenBox(ScreenManager.Instance.boxInfoScript.CallOpenBoxAnimation, 
                ScreenManager.Instance.boxInfoScript.GetBoxSelected().GetFirstUnopenedBoxId());
            
        } catch (Exception e) {
            Print.LogException(e, this);
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

        try {

            string tx = await Web3GL.SendContract(approve_method, approve_abi, approve_contract, approve_args, approve_value, approve_gasLimit, _baseGasPrice);
            
            OpenBox(myBox);

        } catch (Exception e) {
            Print.LogException(e, this);
            Loading.EndLoading?.Invoke();
            GenericPopUp.Instance.ShowPopUp(_openBoxFailMessage);
        }
    }
    
    public async void ToyoApproveNftTransfer(string tokenID)
    {
        const string approveABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        var _approvedTo = isProduction ? productionToyoApprovedTo : testToyoApprovedTo;
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

        Print.Log("SendContractValues { approveMethod:" + approveMethod + ", approveABI: " + approveABI 
                  + ", _approveContract: " + _approveContract + ", _approveArgs: " + _approveArgs + ", approveValue: "
                  + approveValue + ", approveGasLimit: " + approveGasLimit + ", approveGasPrice: " + _baseGasPrice);

        try 
        {
            var _hash = await Web3GL.SendContract(approveMethod, approveABI, _approveContract, _approveArgs,
                approveValue, approveGasLimit, _baseGasPrice);
            Print.Log("TransactionHash: " + _hash);
            
            ToyoStake(tokenID);
        } 
        catch (Exception _exception) 
        {
            Print.LogException(_exception, this);
            Loading.EndLoading?.Invoke();
            //GenericPopUp.Instance.ShowPopUp(_genericFailMessage);
            HandleExceptionTMError(_exception, TRANSACTION_TYPE.APPROVE);
        }
    }

    private async void ToyoStake(string tokenID)
    {
        var _abi = "[{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"stakeToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        var _contract = isProduction ? productionToyoApprovedTo : testToyoApprovedTo;
        var _method = "stakeToken";

        string[] _obj = { tokenID };
        var _args = JsonConvert.SerializeObject(_obj);
        
        var _value = "0";
        var _gasLimit = "";

        Print.Log("SendContractValues { _abi:" + _abi + ", _contract: " + _contract + ", _method: " + _method 
                  + ", _args: " + _args + ", _value: " + _value + ", _gasLimit: " + _gasLimit + ", _gasPrice: " + _baseGasPrice);
        
        try
        {
            var _hash = await Web3GL.SendContract(_method, _abi, _contract, _args, _value, _gasLimit, _baseGasPrice);
            Print.Log("StakeHash: " + _hash);
            //ScreenManager.Instance.trainingModuleScript.SendToyoToTrainingOnServer();
            GenericPopUp.Instance.ShowPopUp(_stakeInitializeMessage, GoToCarousel);

        } 
        catch (Exception _exception) 
        {
            Print.LogException(_exception, this);
            Print.LogError("exceptionMessage: " + _exception.Message);
            Print.LogError("exceptionCode: " + _exception.HResult);
            //TODO: Alert backend the stake error
            Loading.EndLoading?.Invoke();
            
            HandleExceptionTMError(_exception, TRANSACTION_TYPE.STAKE);
            
            /*GenericPopUp.Instance.ShowPopUp(_exception.Message.Contains("User denied transaction signature")
                ? _transactionRefusedMessage
                : _genericFailMessage);*/
        }
    }

    public async void ClaimToken(ClaimParameters parameters)
    {
        const string abi =
            "[{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_claimId\",\"type\":\"string\"},{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_bondAmount\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"_cardCode\",\"type\":\"string\"},{\"internalType\":\"bytes\",\"name\":\"_signature\",\"type\":\"bytes\"}],\"name\":\"claimToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
            
        const string method = "claimToken";
        
        var _contract = isProduction ? productionToyoApprovedTo : testToyoApprovedTo;

        string[] _obj = { parameters.claimID, parameters.tokenID, parameters.bond, parameters.cardCode, parameters.signature };
        var _args = JsonConvert.SerializeObject(_obj);
        
        const string value = "0";
        const string gasLimit = "";

        Print.Log("ClaimTokenValues { _abi:" + abi + ", _contract: " + _contract + ", _method: " + method 
                  + ", _args: " + _args + ", _value: " + value + ", _gasLimit: " + gasLimit + ", _gasPrice: " + _baseGasPrice);
        
        try
        {
            var _hash = await Web3GL.SendContract(method, abi, _contract, _args, value, gasLimit, _baseGasPrice);
            Print.Log("ClaimHash: " + _hash);
            //TrainingConfig.Instance.CloseCurrentTrainingInServer();
            GenericPopUp.Instance.ShowPopUp(_claimInitializeMessage, GoToCarousel);

        } 
        catch (Exception _exception) 
        {
            Print.LogException(_exception, this);
            Print.LogError("exceptionMessage: " + _exception.Message);
            Print.LogError("exceptionCode: " + _exception.HResult);
            //TrainingConfig.Instance.FailedClaim();

            HandleExceptionTMError(_exception, TRANSACTION_TYPE.CLAIM);

            /*GenericPopUp.Instance.ShowPopUp(_exception.Message.Contains("User denied transaction signature")
                ? _transactionRefusedMessage
                : _genericFailMessage);*/
        }
    }

    private void GoToCarousel() => ScreenManager.Instance.GoToScreen(ScreenState.ToyoInfo);

    public void OnToyoListSuccess(string json)
    {
        var _myObject = JsonUtility.FromJson<CallbackToyoList>(json);
        if (_myObject.toyos == null || _myObject.toyos.Length == 0)
        {
            _myObject.toyos = Array.Empty<Toyo>();    
        }
        ToyoManager.SaveIsStakeBackup(_myObject.toyos);
        ToyoManager.Instance.Player.toyos = _myObject.toyos;
        StartCoroutine(_databaseConnection.CallGetToyoData(OnToyoDetailSuccess, _myObject.toyos));
    }

    public void UpdateToyoIsStakedList(string json, Action callback = null)
    {
        var _myObject = JsonUtility.FromJson<CallbackToyoList>(json);
        if (_myObject.toyos == null || _myObject.toyos.Length == 0)
        {
            callback?.Invoke();
            return;
        }
        ToyoManager.SaveIsStakeBackup(_myObject.toyos);
        foreach (var _toyoObject in ToyoManager.Instance.ToyoList)
            _toyoObject.isStaked = ToyoManager.GetIsStakedById(_toyoObject.tokenId);
        callback?.Invoke();
    }

    public void OnToyoDetailSuccess(string json)
    {
        var _myObject = JsonUtility.FromJson<CallbackToyoDetails>(json);
        _toyoList.Add(_myObject.toyo);
        UpdateToyoDetails(_myObject.toyo);
        Print.Log("Toyo Details Success: " + _myObject.toyo.name);
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
        Print.Log("Refresh clicked!");
    }

    private void GoToNextScreen()
    {
        ScreenManager.Instance.GoToScreen(ToyoManager.Instance.ToyoList.Count > 0
            ? ScreenState.MainMenu
            : ScreenState.Welcome);
    }

    private void HandleExceptionTMError(Exception exception, TRANSACTION_TYPE transactionType) //TM = Training Module
    {
        var _text = exception.Message;
        if (_text.Contains("transaction underpriced")) //very low gas price
        {
            GenericPopUp.Instance.ShowPopUp(_transactionUnderpriced);
        }
        else if (_text.Contains("denied transaction signature")) //transaction refused
        {
            GenericPopUp.Instance.ShowPopUp(_transactionRefusedMessage);
        }
        else if (_text.Contains("was not mined within")) //timeout
        {
            switch (transactionType)
            {
                case TRANSACTION_TYPE.CLAIM:
                    GenericPopUp.Instance.ShowPopUp(_claimInitializeMessage, GoToCarousel);
                    break;
                case TRANSACTION_TYPE.STAKE:
                    GenericPopUp.Instance.ShowPopUp(_stakeInitializeMessage, GoToCarousel);
                    break;
                case TRANSACTION_TYPE.APPROVE:
                    GenericPopUp.Instance.ShowPopUp(_approveInitializeMessage, GoToCarousel);
                    break;
            }
        }
        else
            GenericPopUp.Instance.ShowPopUp(_genericFailMessage);
    }
}

public struct ClaimParameters
{
    public string claimID;
    public string tokenID;
    public string bond;
    public string cardCode;
    public string signature;
}