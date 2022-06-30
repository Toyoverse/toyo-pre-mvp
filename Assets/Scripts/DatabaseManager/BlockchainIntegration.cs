using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlockchainIntegration : MonoBehaviour
{
    public bool skipLogin;
    public Toggle rememberMe;
    
    private DatabaseConnection _databaseConnection => DatabaseConnection.Instance;
    private string _account;
    private List<Toyo> _toyoList;

    public async void StartLoginMetamask()
    {
        if (skipLogin)
        {
            PlayerPrefs.SetString("TokenJWT", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ3YWxsZXRJZCI6IjB4MGM0ZWJmMzBlZTRhNjA3ZTJlMTBhYWI2Y2ZhMzVkMDQzNDJlYWVlYiIsInRyYW5zYWN0aW9uIjoiZGZnNTR3ZWZkIiwiaWF0IjoxNjU2MzY0MjQ3LCJleHAiOjE2NTY5NjkwNDd9.Hl_B8b5xdcCn5p9slJPs1-b26sZSpdYBZSCRsH6akgk");
            GoToNextScreen();
            return;
        }
        
        var _message = ((int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds)
            .ToString();
        var _signature = await Web3Wallet.Sign(_message);
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

        GoToNextScreen();
    }

    private void GoToNextScreen()
    {
        ScreenManager.Instance.GoToScreen(ScreenManager.Instance.haveToyo
            ? ScreenState.MainMenu
            : ScreenState.Welcome);
        
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

    public void OnToyoListSuccess(string json)
    {
        var _myObject = JsonUtility.FromJson<CallbackToyoList>(json);

        if (_myObject.toyos == null || _myObject.toyos.Length == 0)
        {
            _myObject.toyos = Array.Empty<Toyo>();    
        }

        ToyoManager.Instance.Player.toyos = _myObject.toyos;
        
        //This isn't finished in the backend yet
        _databaseConnection.CallGetToyoData(OnToyoDetailSuccess, _myObject.toyos); //Might be tokenid, check later
    }

    public void OnToyoDetailSuccess(string json)
    {
        var _myObject = JsonUtility.FromJson<CallbackToyoDetails>(json);
        _toyoList.Add(_myObject.toyo);
        ToyoManager.InitializeToyos();
    }
}