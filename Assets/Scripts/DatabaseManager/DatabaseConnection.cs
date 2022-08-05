using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Database
{
    public class DatabaseConnection : Singleton<DatabaseConnection>
    {
        public bool isDebug = false;
        public BlockchainIntegration BlockchainIntegration;
        private IEnumerator _requestCoroutine;
        
        private string URL = "";
        
        [SerializeField]
        private string loginURL = "https://players-web-bff-dev.herokuapp.com/player/login";

        [SerializeField]
        private string baseURL = "https://ts-toyo-web-bff.herokuapp.com";

        [SerializeField] private string openBoxBaseURL = "https://ts-unboxing-web-bff.herokuapp.com";

        [SerializeField]
        private string boxesSufixURL = "/player/boxes";

        [SerializeField]
        private string toyosSufixURL = "/player/toyos";
        
        [SerializeField]
        private string toyosDetailSufixURL = "/player/toyo/";

        [SerializeField]
        private string closedBoxSufixURL = "/player/box/closedbox/";

        [SerializeField]
        private string openBoxSufixURL = "/player/box/";
        
        private void Awake()
        {
            if (!isDebug) return;
            TextAsset _jsonTextAsset = Resources.Load<TextAsset>("DatabasePlaceholder");
            OnConnectionSuccess(_jsonTextAsset.text);
        }
        
        public void CallLogin(Action<string> callback, List<(string,string)> parameters = null)
        {
            URL = loginURL;
            var _request = GenerateRequest(HTTP_REQUEST.POST, parameters);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }
        
        public void CallGetPlayerBoxes(Action<string> callback)
        {
            URL = baseURL + boxesSufixURL;
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }
        
        public void CallGetPlayerToyo(Action<string> callback)
        {
            URL = baseURL + toyosSufixURL;
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }
        
        public void GetOpenBox(Action<string> callback, string boxId)
        {
            URL = openBoxBaseURL + closedBoxSufixURL + boxId;
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }

        public void PostOpenBox(Action<string> callback, string boxId)
        {
            URL = openBoxBaseURL + openBoxSufixURL + boxId;
            var _request = GenerateRequest(HTTP_REQUEST.POST);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }

        public IEnumerator CallGetToyoData(Action<string> callback, Toyo[] toyoList)
        {
            foreach (var _toyo in toyoList)
            {
                URL = baseURL + toyosDetailSufixURL + _toyo.objectId;
                var _request = GenerateRequest(HTTP_REQUEST.GET);
                yield return ProcessRequestCoroutine(callback, _request);
            }
            
            ToyoManager.StartGame();
        }
        
        private UnityWebRequest GenerateRequest (HTTP_REQUEST requestType, List<(string,string)> parameters = null) {
            
            return requestType switch
            {
                HTTP_REQUEST.GET => GenerateGet(URL),
                HTTP_REQUEST.PUT => GeneratePost(URL),
                HTTP_REQUEST.POST => GeneratePost(URL, parameters),
                _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
            };
        }
        
        private UnityWebRequest GenerateGet(string uri)
        {
            UnityWebRequest _requestGet = UnityWebRequest.Get(uri);
            _requestGet.SetRequestHeader("Authorization", PlayerPrefs.GetString("TokenJWT"));
            return _requestGet;
        }
        
        private UnityWebRequest GeneratePost(string uri, List<(string,string)> parameters = null)
        {
            WWWForm _form = new WWWForm();
            
            if (parameters != null)
                foreach (var _parameter in parameters)
                    _form.AddField(_parameter.Item1, _parameter.Item2);

            UnityWebRequest _requestPost = UnityWebRequest.Post (uri, _form);
            _requestPost.SetRequestHeader("Authorization", PlayerPrefs.GetString("TokenJWT"));
            return _requestPost;
        }
        
        private IEnumerator ProcessRequestCoroutine (Action<string> callback, UnityWebRequest request)
        {
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest();

            if (request.error != null)
                Debug.Log (request.error + " | " + URL);
            else
            {
                var _json = request.downloadHandler.text;
                callback.Invoke(_json);
                /*Debug.Log("Request: " + URL);
                Debug.Log("json: " + _json);*/
            }
        }
        
        private void OnConnectionSuccess(string json)
        {
            var _myObject = JsonUtility.FromJson<DatabasePlayerJson>(json);    
            Debug.Log(_myObject.player);
            ToyoManager.SetPlayerData(_myObject.player);
        }
    }
}

public enum HTTP_REQUEST
{
    GET,
    POST,
    PUT
}