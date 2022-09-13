using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools
{
    public static class Print 
    {
        public static void Log(string message)
        {
        #if !UNITY_EDITOR
            if (DatabaseConnection.Instance.blockchainIntegration.isProduction)
                return;
        #endif
            Debug.Log(message);
        }
        
        public static void LogError(string message)
        {
        #if !UNITY_EDITOR
            if (DatabaseConnection.Instance.blockchainIntegration.isProduction)
                return;
        #endif
            Debug.LogError(message);
        }
        
        public static void LogWarning(string message)
        {
        #if !UNITY_EDITOR
            if (DatabaseConnection.Instance.blockchainIntegration.isProduction)
                return;
        #endif
            Debug.LogWarning(message);
        }
        
        public static void LogException(Exception exception, Object context)
        {
        #if !UNITY_EDITOR
            if (DatabaseConnection.Instance.blockchainIntegration.isProduction)
                return;
        #endif
            Debug.LogException(exception);
        }
    }
}
