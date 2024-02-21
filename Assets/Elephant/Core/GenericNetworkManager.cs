using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ElephantSDK
{
    public class GenericNetworkManager<T> where T : new()
    {
        private readonly string _gameID;
        private readonly string _gameSecret;

        public GenericNetworkManager()
        {
            _gameID = ElephantCore.Instance.GameID;
            _gameSecret = ElephantCore.Instance.GameSecret;
        }

        public IEnumerator PostWithResponse(string url, string bodyJsonString, Action<GenericResponse<T>> onResponse, Action<string> onError)
        {
            Debug.Log("Requesting To: " + url);
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            var bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Content-Encoding", "gzip");
            request.SetRequestHeader("Authorization", Utils.SignString(bodyJsonString, _gameSecret));
            request.SetRequestHeader("GameID", _gameID);

            if (typeof(T) == typeof(OpenResponse))
            {
                request.SetRequestHeader("Accept-Encoding", "gzip");
            }

            yield return request.SendWebRequest();
            
            Debug.Log("Body: " + request.downloadHandler.text);
            
            if (request.isNetworkError)
            {
                Debug.Log("[Elephant SDK] Request Error");
                onError("Request Error");
            }
            else
            {
                try
                {
                    onResponse(new GenericResponse<T>(request));
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    onError(e.Message);
                }
            }
        }
    }
}