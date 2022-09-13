using System;
using Database;
using Org.BouncyCastle.Crypto.Tls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools
{
    public static class Print 
    {
        private static bool IsProduction => DatabaseConnection.Instance != null && DatabaseConnection.Instance.blockchainIntegration.isProduction;
        
        public static void Log(string message)
        {
        #if !UNITY_EDITOR
            if (IsProduction)
                return;
        #endif
            if (IsProduction)
                return;
            Debug.Log(message);
        }
        
        public static void LogError(string message)
        {
        #if !UNITY_EDITOR
            if (IsProduction)
                return;
        #endif
            Debug.LogError(message);
        }
        
        public static void LogWarning(string message)
        {
        #if !UNITY_EDITOR
            if (IsProduction)
                return;
        #endif
            Debug.LogWarning(message);
        }
        
        public static void LogException(Exception exception, Object context)
        {
        #if !UNITY_EDITOR
            if (IsProduction)
                return;
        #endif
            Debug.LogException(exception);
        }
    }
}
