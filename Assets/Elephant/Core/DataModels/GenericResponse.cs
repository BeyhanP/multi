using UnityEngine;
using UnityEngine.Networking;

namespace ElephantSDK
{
    public class GenericResponse<T>
    {
        public long responseCode;
        public string errorMessage;
        public bool isNetworkError;
        public bool isHttpError;
        public T data;

        public GenericResponse(long responseCode, string errorMessage, bool isNetworkError, bool isHttpError, T data)
        {
            this.responseCode = responseCode;
            this.errorMessage = errorMessage;
            this.isNetworkError = isNetworkError;
            this.isHttpError = isHttpError;
            this.data = data;
        }

        public GenericResponse(UnityWebRequest request)
        {
            this.responseCode = request.responseCode;
            this.errorMessage = request.error;
            this.isNetworkError = request.isNetworkError;
            this.isHttpError = request.isHttpError;
            this.data = JsonUtility.FromJson<T>(request.downloadHandler.text);
        }
    }
}