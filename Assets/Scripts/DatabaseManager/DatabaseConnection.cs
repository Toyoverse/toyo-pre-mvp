using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace Database
{
    public class DatabaseConnection : Singleton<DatabaseConnection>
    {
        #region Production Ambient DatabaseConnection Info
        
        [SerializeField]
        private string productionLoginURL = "https://4nwemmj4hh.execute-api.us-east-1.amazonaws.com/production/player/login";
        [SerializeField]
        private string productionBaseURL = "https://r99i910fe6.execute-api.us-east-1.amazonaws.com/production";
        [SerializeField]
        private string productionOpenBoxBaseURL = "https://v49y0yol5a.execute-api.us-east-1.amazonaws.com/production";
        [SerializeField]
        private string productionTrainingBaseURL = "https://la5bj71yml.execute-api.us-east-1.amazonaws.com/production";

        #endregion
        
        #region Test Ambient DatabaseConnection Info

        [SerializeField]
        private string testLoginURL = "https://players-web-bff-dev.herokuapp.com/player/login";
        [SerializeField]
        private string testBaseURL = "https://ts-toyo-web-bff.herokuapp.com";
        [SerializeField]
        private string testOpenBoxBaseURL = "https://ts-unboxing-web-bff.herokuapp.com";
        [SerializeField] 
        private string testTrainingBaseURL = "https://ts-trainning-web-bff.herokuapp.com";

        #endregion
        
        public bool isDebug = false;
        public BlockchainIntegration blockchainIntegration;
        private IEnumerator _requestCoroutine;
        
        private string _url = "";
        private string loginURL = "";
        private string baseURL = "";
        private string openBoxBaseURL = "";
        private string trainingBaseURL = "";

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
        [SerializeField]
        private string registerTrainingEventSuffix = "/training-events";
        [SerializeField]
        private string getCurrentTrainingEventSuffix = "/training-events/search/current";
        [SerializeField]
        private string registerCardRewardSuffixURL = "/toyo-persona-training-events";
        [SerializeField]
        private string toyoTrainingSuffixURL = "/training";

        private void Awake()
        {
            loginURL = blockchainIntegration.isProduction ? productionLoginURL : testLoginURL;
            baseURL = blockchainIntegration.isProduction ? productionBaseURL : testBaseURL;
            openBoxBaseURL = blockchainIntegration.isProduction ? productionOpenBoxBaseURL : testOpenBoxBaseURL;
            trainingBaseURL = blockchainIntegration.isProduction ? productionTrainingBaseURL : testTrainingBaseURL;
            
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
                _url = baseURL + toyosDetailSuffixURL + _toyo.objectId;
                var _request = GenerateRequest(HTTP_REQUEST.GET);
                yield return ProcessRequestCoroutine(callback, _request);
            }
            
            TrainingConfig.Instance.InitializeTrainingModule();
        }

        public void PostTrainingConfig(Action<string> callback, string jsonString)
        {
            _url = trainingBaseURL + registerTrainingEventSuffix;
            Print.Log(_url);
            var _request = GeneratePost(_url, jsonString);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }

        public void PostCardReward(Action<string> callback, string jsonString)
        {
            _url = trainingBaseURL + registerCardRewardSuffixURL;
            var _request = GeneratePost(_url, jsonString);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }

        public void GetCurrentTrainingConfig(Action<string> callback, Action<string> failedCallback)
        {
            _url = trainingBaseURL + getCurrentTrainingEventSuffix;
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            StartCoroutine(ProcessRequestCoroutine(callback, _request, failedCallback));
        }

        public void CallGetInTrainingList(Action<string> callback)
        {
            _url = trainingBaseURL + toyoTrainingSuffixURL; 
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }

        public void PostToyoInTraining(Action<string> callback, string jsonString)
        {
            _url = trainingBaseURL + toyoTrainingSuffixURL /*+ "/" + ToyoManager.GetSelectedToyo().tokenId*/;
            var _request = GeneratePost(_url, jsonString);
            StartCoroutine(ProcessRequestCoroutine(callback, _request));
        }

        public void CallCloseTraining(Action<string> successCallback, Action<string> failedCallback, string trainingID)
        {
            _url = trainingBaseURL + toyoTrainingSuffixURL + "/" + trainingID;
            var _request = GenerateRequest(HTTP_REQUEST.PUT);
            StartCoroutine(ProcessRequestCoroutine(successCallback, _request, failedCallback));
        }

        public void GetRewardValues(Action<string> successCallback, Action<string> failedCallback, string trainingID)
        {
            _url = trainingBaseURL + toyoTrainingSuffixURL + "/" + trainingID;
            var _request = GenerateRequest(HTTP_REQUEST.GET);
            StartCoroutine(ProcessRequestCoroutine(successCallback, _request, failedCallback));
        }

        private UnityWebRequest GenerateRequest (HTTP_REQUEST requestType, List<(string,string)> parameters = null) {
            
            return requestType switch
            {
                HTTP_REQUEST.GET => GenerateGet(_url),
                HTTP_REQUEST.PUT => GeneratePut(_url),
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
        
        private UnityWebRequest GeneratePut(string uri, string jsonString)
        {
            var _requestPost = new UnityWebRequest(uri, "PUT");
            var _jsonToSend = new UTF8Encoding().GetBytes(jsonString);
            _requestPost.uploadHandler = new UploadHandlerRaw(_jsonToSend);
            _requestPost.downloadHandler = new DownloadHandlerBuffer();
            
            _requestPost.SetRequestHeader("Authorization", PlayerPrefs.GetString("TokenJWT"));
            _requestPost.SetRequestHeader("Content-Type", "application/json");

            return _requestPost;
        }
        
        private UnityWebRequest GeneratePut(string uri)
        {
            var _postData = "";
            var _requestPut = UnityWebRequest.Put(uri, _postData);
            _requestPut.SetRequestHeader("Authorization", PlayerPrefs.GetString("TokenJWT"));
            return _requestPut;
        }
        
        private IEnumerator ProcessRequestCoroutine (Action<string> callback, UnityWebRequest request,
            Action<string> failedCallback = null)
        {
            if(!blockchainIntegration.isProduction)
                request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                var _requestResult = request.downloadHandler.text;
                Print.Log (request.error + " | " + _requestResult + " / " + _url);
                failedCallback?.Invoke(_requestResult);
            }
            else
            {
                var _requestResult = request.downloadHandler.text;
                var _responseCode = request.responseCode;
                callback.Invoke(_requestResult);
                Print.Log("ResponseCode: " + _responseCode + " | RequestResult: " 
                          + _requestResult + " | URL: " + _url);
            }

            yield return new WaitForSeconds(2);
            request.Dispose();
        }
        
        private void OnConnectionSuccess(string json)
        {
            var _myObject = JsonUtility.FromJson<DatabasePlayerJson>(json);    
            Print.Log(_myObject.player.ToString());
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