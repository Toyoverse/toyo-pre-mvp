using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Database
{
    public class DatabaseConnection : Singleton<DatabaseConnection>
    {
        public bool isDebug = true;
        
        private void Awake()
        {
            
            if(!isDebug)
                GenerateRequest(OnConnectionSuccess);
            else
            {
                TextAsset _jsonTextAsset = Resources.Load<TextAsset>("DatabasePlaceholder");    
                OnConnectionSuccess(_jsonTextAsset.text);
            }
            
        }

        private void OnConnectionSuccess(string json)
        {
            var _myObject = JsonUtility.FromJson<DatabaseJson>(json);    
            Debug.Log(_myObject.player);
        }
        
        private const string URL = "https://nakatoshivault.com/toyoAssets/";

        private void GenerateRequest (Action<string>  callback) {
            StartCoroutine (ProcessRequest(callback, URL));
        }

        private IEnumerator ProcessRequest (Action<string> callback, string uri)
        {
            using UnityWebRequest _request = UnityWebRequest.Get (uri);
            _request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return _request.SendWebRequest ();
                
            if (_request.error != null)
                Debug.Log (_request.error);
            else
            {
                var _json = _request.downloadHandler.text;
                callback.Invoke(_json);
            }
                
        }
        
    }
   
}
