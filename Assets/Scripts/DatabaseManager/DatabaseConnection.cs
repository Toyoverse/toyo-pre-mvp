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
        
        private void Awake()
        {
            TextAsset _jsonTextAsset = Resources.Load<TextAsset>("DatabasePlaceholder");    
            OnConnectionSuccess(_jsonTextAsset.text);
            /*
            if(!isDebug)
                GenerateRequest(OnConnectionSuccess);
            else
            {
                TextAsset _jsonTextAsset = Resources.Load<TextAsset>("DatabasePlaceholder");    
                OnConnectionSuccess(_jsonTextAsset.text);
            }
            */
        }
        /*
        curl --location --request POST 'https://players-web-bff-dev.herokuapp.com/player/login' \
        --header 'Content-Type: application/json' \
        --data-raw '{
        "walletAddress": "234etrgd",
        "transactionHash": "dfg54wefd"
        }'
        */

        private void CallLogin()
        {
            var _request = GenerateRequest(HTTP_REQUEST.POST);
            StartCoroutine(ProcessRequest(OnConnectionSuccess, _request));
        }
        
        private void OnConnectionSuccess(string json)
        {
            var _myObject = JsonUtility.FromJson<DatabaseJson>(json);    
            Debug.Log(_myObject.player);
            player = _myObject.player;
        }
        
        private const string URL = "https://players-web-bff-dev.herokuapp.com/player/login";

        private UnityWebRequest GenerateRequest (HTTP_REQUEST requestType) {
            
            return requestType switch
            {
                HTTP_REQUEST.GET => GeneratePost(URL),
                HTTP_REQUEST.PUT => GeneratePost( URL),
                HTTP_REQUEST.POST => GeneratePost( URL),
                _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
            };
            
        }

        private UnityWebRequest GenerateGet(string uri)
        {
            using UnityWebRequest _requestPost = UnityWebRequest.Get (uri);
            return _requestPost;
        }
        
        private UnityWebRequest GeneratePost(string uri)
        {
            WWWForm _form = new WWWForm();
            Debug.Log ("234etrgd");
            _form.AddField("walletAddress", "234etrgd");
            _form.AddField("transactionHash", "dfg54wefd");
            using UnityWebRequest _requestPost = UnityWebRequest.Post (uri, _form);
            return _requestPost;
        }

        private IEnumerator ProcessRequest (Action<string> callback, UnityWebRequest request)
        {

            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest ();
                
            if (request.error != null)
                Debug.Log (request.error);
            else
            {
                var _json = request.downloadHandler.text;
                callback.Invoke(_json);
            }
        }
    }
   
    

    
    //{"token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ3YWxsZXRJZCI6IjIzNGV0cmdkIiwidHJhbnNhY3Rpb24iOiJkZmc1NHdlZmQiLCJpYXQiOjE2NTU0NzE4MzUsImV4cCI6MTY1NjA3NjYzNX0.xOIJt6YIj3dzOBHHOoaPqa32FLTqW-QxPjbfOIc98Z8","expiresAt":"24/06/2022 13:17"}
}

public enum HTTP_REQUEST
{
    GET,
    POST,
    PUT
}