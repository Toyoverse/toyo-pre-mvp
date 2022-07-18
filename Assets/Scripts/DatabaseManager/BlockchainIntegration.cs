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
    private List<Toyo> _toyoList = new();

    public async void StartLoginMetamask()
    {
        Loading.StartLoading?.Invoke();
        if (skipLogin)
        {
            //PlayerPrefs.SetString("TokenJWT", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ3YWxsZXRJZCI6IjB4MGM0ZWJmMzBlZTRhNjA3ZTJlMTBhYWI2Y2ZhMzVkMDQzNDJlYWVlYiIsInRyYW5zYWN0aW9uIjoiZGZnNTR3ZWZkIiwiaWF0IjoxNjU2MzY0MjQ3LCJleHAiOjE2NTY5NjkwNDd9.Hl_B8b5xdcCn5p9slJPs1-b26sZSpdYBZSCRsH6akgk");
            //PlayerPrefs.SetString("TokenJWT", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ3YWxsZXRJZCI6IjB4NEU4QTM1NTc2RkJhZkM1ODBEZTliNGIxYUM2OGI2QmU3OWIyQTJFOCIsInRyYW5zYWN0aW9uIjoiX2NyZWF0ZWRfYnlfbWlncmF0aW9uX3Rvb2wiLCJpYXQiOjE2NTY2MTczNzksImV4cCI6MTY1NzIyMjE3OX0.MdMmFaMphpid7juVKfd-RdOidxTA8_jLnl-U2FgEcEs");
            //PlayerPrefs.SetString("TokenJWT", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ3YWxsZXRJZCI6IjB4NEU4QTM1NTc2RkJhZkM1ODBEZTliNGIxYUM2OGI2QmU3OWIyQTJFOCIsInRyYW5zYWN0aW9uIjoiZGZnNTR3ZWZkIiwiaWF0IjoxNjU3MjI3NDY5LCJleHAiOjE2NTc4MzIyNjl9.ZnK-aYXfFKNFpsSAqbJSt8ZGQdWSRjmQpiRmZNSN5rg");
            PlayerPrefs.SetString("TokenJWT", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ3YWxsZXRJZCI6IjB4ZmY2NTRhYTAyMDBlNTUxZGJlOWMwMGM4YjVjMWY0ZTg4ZDc0ZmNjMyIsInRyYW5zYWN0aW9uIjoiMHhmZjY1NGFhMDIwMGU1NTFkYmU5YzAwYzhiNWMxZjRlODhkNzRmY2MzIiwiaWF0IjoxNjU3OTE1MTE3LCJleHAiOjE2NTg1MTk5MTd9.vxOiDNXs6lnCovzSyHfm3DfWMOaQIRkNSdU4gBXIvSg");
            GoToNextScreen();
            return;
        }
        
        var _message = ((int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds)
            .ToString();
        var _signature = "";
        
        #if (UNITY_WEBGL || UNITY_WEBGL_API) && !UNITY_EDITOR
        try 
        {
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
}