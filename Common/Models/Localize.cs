using System;

namespace Common.Models;

public static class Localize
{
    public static class ErrorType
    {
        public static string ModelState => "ModelState";
        public static string File => "File";
    }

    public static class Error
    {
        #region File

        public static string FileCreateFailed => "FileCreateFailed";
        public static string FileReadFailed => "FileReadFailed";
        public static string FileDeleteFailed => "FileDeleteFailed";

        #endregion
    }

    public static class WarningType
    {
        public static string File => "File";
    }

    public static class Warning
    {
        public static string XssVulnerable => "XSSVulnerable";
    }

    public static class Log
    {
        #region Middleware

        public static string MiddlewareForwardStart(string assemblyName) =>
            $"[Middleware] {assemblyName} (Forward-Start)";

        public static string MiddlewareForwardEnd(string assemblyName) => $"[Middleware] {assemblyName} (Forward-End)";

        public static string MiddlewareBackwardStart(string assemblyName) =>
            $"[Middleware] {assemblyName} (Backward-End)";

        public static string MiddlewareBackwardEnd(string assemblyName) =>
            $"[Middleware] {assemblyName} (Backward-End)";

        #endregion

        #region Method

        public static string MethodStart(string assemblyName, string methodName) =>
            $"[{assemblyName}.{methodName}] (Start)";

        public static string MethodEnd(string assemblyName, string methodName) =>
            $"[{assemblyName}.{methodName}] (End)";

        public static string MethodError(string assemblyName, string methodName, string message) =>
            $"[{assemblyName}.{methodName}] (Error) {Environment.NewLine + message}";

        #endregion

        #region Background Service

        public static string BackgroundServiceStarting(string assemblyName) =>
            $"[{assemblyName}] (Starting)";

        public static string BackgroundServiceStopping(string assemblyName) =>
            $"[{assemblyName}] (End)";

        public static string BackgroundServiceWorking(string assemblyName) =>
            $"[{assemblyName}] (Working)";

        public static string BackgroundServiceError(string assemblyName, string message) =>
            $"[{assemblyName}] (Error) {Environment.NewLine + message}";

        #endregion
    }
}