using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VehicleEffects
{
    /// <summary>
    /// Wrapper for UnityEngine.Debug.Log* methods that adds a prefix to the output for easy debugging
    /// </summary>
    public static class Logging
    {
        private const string LOG_PREFIX = "Vehicle Effects";

        public static void Log(object obj)
        {
            Debug.Log(LOG_PREFIX + " " + obj);
        }

        public static void LogWarning(object obj)
        {
            Debug.LogWarning(LOG_PREFIX + " " + obj);
        }

        public static void LogError(object obj)
        {
            Debug.LogError(LOG_PREFIX + " " + obj);
        }

        public static void LogException(Exception e)
        {
            Log("An exception was thrown:");
            Debug.LogException(e);
        }
    }
}
