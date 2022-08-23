using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Database
{
    public class DatabaseConnection : Singleton<DatabaseConnection>
    {
        public bool isDebug = false;
        public BlockchainIntegration blockchainIntegration;
        private IEnumerator _requestCoroutine;
        
        private string _url = "";
        
        [SerializeField]
        private string loginURL = "https://players-web-bff-dev.herokuapp.com/player/login";

        [SerializeField]
        private string baseURL = "https://ts-toyo-web-bff.herokuapp.com";

        [SerializeField] private string openBoxBaseURL = "https://ts-unboxing-web-bff.herokuapp.com";

        [SerializeField]
        private string boxesSuffixURL = "/player/boxes";

        [SerializeField]
        private string toyosSuffixURL = "/player/toyos";
        
        [SerializeField]
        private string toyosDetailSuffixURL = "/player/toyo/";

        [SerializeField]
        private string closedBoxSuffixURL = "/player/box/closedbox/";

        [SerializeField]
        private string openBoxSuffixURL = "/player/box/";

        [SerializeField] private string trainingBaseURL = "https://ts-trainning-web-bff.herokuapp.com";
        [SerializeField] private string registerTrainingSuffixURL = "/training-events";
        
        private void Awake()
        {
            if (!isDebug) return;
            var _jsonTextAsset = Resources.Load<TextAsset>("DatabasePlaceholder");
            OnConnectionSuccess(_jsonTextAsset.text);
        }
        
        public void CallLogin(Action<string> callback, List<(string,string)> parameters = null)
        {
            _url = loginURL;
            var _request = GenerateRequest(HTTP_REQUEST.POST, parameters);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }
        
        public void CallGetPlayerBoxes(Action<string> callback)
        {
            _url = baseURL + boxesSuffixURL;
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }
        
        public void CallGetPlayerToyo(Action<string> callback)
        {
            _url = baseURL + toyosSuffixURL;
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }
        
        public void GetOpenBox(Action<string> callback, string boxId)
        {
            _url = openBoxBaseURL + closedBoxSuffixURL + boxId;
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }

        public void PostOpenBox(Action<string> callback, string boxId)
        {
            _url = openBoxBaseURL + openBoxSuffixURL + boxId;
            var _request = GenerateRequest(HTTP_REQUEST.POST);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }

        public IEnumerator CallGetToyoData(Action<string> callback, Toyo[] toyoList)
        {
            foreach (var _toyo in toyoList)
            {
                yield return new WaitForSeconds(2f);
                
                _url = baseURL + toyosDetailSuffixURL + _toyo.objectId;
                var _request = GenerateRequest(HTTP_REQUEST.GET);
                yield return ProcessRequestCoroutine(callback, _request);
            }
            
            ToyoManager.StartGame();
        }

        public void PostTrainingConfig(Action<string> callback, string jsonString)
        {
            _url = trainingBaseURL + registerTrainingSuffixURL;
            var _request = GeneratePost(_url, jsonString);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }
        
        private UnityWebRequest GenerateRequest (HTTP_REQUEST requestType, List<(string,string)> parameters = null) {
            
            return requestType switch
            {
                HTTP_REQUEST.GET => GenerateGet(_url),
                HTTP_REQUEST.PUT => GeneratePost(_url),
                HTTP_REQUEST.POST => GeneratePost(_url, parameters),
                _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
            };
        }

        private UnityWebRequest GenerateGet(string uri)
        {
            var _requestGet = UnityWebRequest.Get(uri);
            _requestGet.SetRequestHeader("Authorization", PlayerPrefs.GetString("TokenJWT"));
            return _requestGet;
        }
        
        private UnityWebRequest GeneratePost(string uri, List<(string,string)> parameters = null)
        {
            var _form = new WWWForm();
            
            if (parameters != null)
                foreach (var _parameter in parameters)
                    _form.AddField(_parameter.Item1, _parameter.Item2);

            var _requestPost = UnityWebRequest.Post (uri, _form);
            _requestPost.SetRequestHeader("Authorization", PlayerPrefs.GetString("TokenJWT"));
            return _requestPost;
        }
        
        private UnityWebRequest GeneratePost(string uri, string jsonString)
        {
            var _requestPost = new UnityWebRequest(uri, "POST");
            var _jsonToSend = new UTF8Encoding().GetBytes(jsonString);
            _requestPost.uploadHandler = new UploadHandlerRaw(_jsonToSend);
            _requestPost.downloadHandler = new DownloadHandlerBuffer();
            
            _requestPost.SetRequestHeader("Authorization", PlayerPrefs.GetString("TokenJWT"));
            _requestPost.SetRequestHeader("Content-Type", "application/json");

            return _requestPost;
        }
        
        private IEnumerator ProcessRequestCoroutine (Action<string> callback, UnityWebRequest request)
        {
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest();

            if (request.error != null)
                 Debug.Log (request.error + " | " + request.downloadHandler.text + " / " + _url);
            else
            {
                var _requestResult = request.downloadHandler.text;
                var _responseCode = request.responseCode;
                callback.Invoke(_requestResult);
                Debug.Log("ResponseCode: " + _responseCode + " | RequestResult: " 
                          + _requestResult + " | URL: " + _url);
            }

            yield return new WaitForSeconds(2);
            request.Dispose();
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