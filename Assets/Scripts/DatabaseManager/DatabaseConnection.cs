using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Database
{
    public class DatabaseConnection : Singleton<DatabaseConnection>
    {
        public bool isDebug = false;
        public Player player;
        public BlockchainIntegration BlockchainIntegration;
        
        private const string URL = "https://players-web-bff-dev.herokuapp.com/player/login";
        
        private void Awake()
        {
            if (!isDebug) return;
            TextAsset _jsonTextAsset = Resources.Load<TextAsset>("DatabasePlaceholder");    
            OnConnectionSuccess(_jsonTextAsset.text);
        }
        
        public void CallLogin(Action<string> callback, List<(string,string)> parameters = null)
        {
            var _request = GenerateRequest(HTTP_REQUEST.POST, parameters);
            ProcessRequest(callback, _request);
        }
        
        public void GetPlayerInfo()
        {
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            ProcessRequest(OnConnectionSuccess, _request);
        }
        
        private UnityWebRequest GenerateRequest (HTTP_REQUEST requestType, List<(string,string)> parameters = null) {
            
            return requestType switch
            {
                HTTP_REQUEST.GET => GenerateGet(URL),
                HTTP_REQUEST.PUT => GeneratePost( URL),
                HTTP_REQUEST.POST => GeneratePost( URL, parameters),
                _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
            };
        }
        
        private UnityWebRequest GenerateGet(string uri)
        {
            UnityWebRequest _requestPost = UnityWebRequest.Get(uri);
            return _requestPost;
        }
        
        private UnityWebRequest GeneratePost(string uri, List<(string,string)> parameters = null)
        {
            WWWForm _form = new WWWForm();
            
            if (parameters != null)
                foreach (var _parameter in parameters)
                    _form.AddField(_parameter.Item1, _parameter.Item2);

            UnityWebRequest _requestPost = UnityWebRequest.Post (uri, _form);
            return _requestPost;
        }
        
        private async void ProcessRequest (Action<string> callback, UnityWebRequest request)
        {
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            await request.SendWebRequest ();
                
            if (request.error != null)
                Debug.Log (request.error);
            else
            {
                var _json = request.downloadHandler.text;
                callback.Invoke(_json);
            }
            
            request.Dispose();
        }
        
        private void OnConnectionSuccess(string json)
        {
            var _myObject = JsonUtility.FromJson<DatabasePlaserJson>(json);    
            Debug.Log(_myObject.player);
            player = _myObject.player;
        }
        
    }
    
}

public enum HTTP_REQUEST
{
    GET,
    POST,
    PUT
}