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

    public async void StartLoginMetamask()
    {
        if (skipLogin)
        {
            GoToNextScreen();
            return;;
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
    }

}
