using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Database
{
    public class DatabaseConnection : Singleton<DatabaseConnection>
    {
        public bool IsDebug = true;
        
        private void Awake()
        {
            
            if(!IsDebug)
                GenerateRequest(OnConnectionSuccess);
            else
            {
                TextAsset _jsonTextAsset = Resources.Load<TextAsset>("DatabasePlaceholder");    
                OnConnectionSuccess(_jsonTextAsset.text);
            }
            
        }

        private void OnConnectionSuccess(string json)
        {
            var myObject = JsonUtility.FromJson<DatabaseJson>(json);    
            Debug.Log(myObject.player);
        }
        
        private const string URL = "https://nakatoshivault.com/toyoAssets/";

        private void GenerateRequest (Action<string>  callback) {
            StartCoroutine (ProcessRequest(callback, URL));
        }

        private IEnumerator ProcessRequest (Action<string> callback, string uri)
        {
            using UnityWebRequest request = UnityWebRequest.Get (uri);
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest ();

            if (request.error != null)
                Debug.Log (request.error);
            else
            {
                var json = request.downloadHandler.text;
                callback.Invoke(json);
            }
                
        }
        
    }
   
}
