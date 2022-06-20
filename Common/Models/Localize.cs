using System;

namespace Common.Models;

public static class Localize
{
    public static class Error
    {
        #region File

        public const string FileCreateFailed = "FileCreateFailed";
        public const string FileReadFailed = "FileReadFailed";
        public const string FileDeleteFailed = "FileDeleteFailed";

        #endregion

        #region Request

        public const string RequestMultipartExpected = "RequestMultipartExpected";
        public const string RequestContentTypeBoundaryNotFound = "RequestContentTypeBoundaryNotFound";
        public const string RequestMultipartBoundaryLengthExceedsLimit = "RequestMultipartBoundaryLengthExceedsLimit";
        public const string RequestMultipartSectionEncodingNotSupported = "RequestMultipartSectionEncodingNotSupported";
        public const string RequestMultipartSectionNotFound = "RequestMultipartSectionNotFound";
        public const string RequestContentDispositionParseFailed = "RequestContentDispositionParseFailed";
        public const string RequestContentDispositionFileExpected = "RequestContentDispositionFileExpected";

        #endregion
        
        #region UnhandledException

        public const string UnhandledExceptionContactSystemAdministrator = "UnhandledExceptionContactSystemAdministrator";

        #endregion

        #region Generic

        public static string ObjectDeserializationFailed => "ObjectDeserializationFailed";
        public static string ObjectCastFailed => "ObjectCastFailed";

        #endregion
    }

    public static class Warning
    {
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
        
        public static string Method(string assemblyName, string methodName, string message) =>
            $"[{assemblyName}.{methodName}] {message}";

        public static string MethodEnd(string assemblyName, string methodName) =>
            $"[{assemblyName}.{methodName}] (End)";

        public static string MethodError(string assemblyName, string methodName, string message) =>
            $"[{assemblyName}.{methodName}] (Error) {Environment.NewLine + message}";
        
        public static string UnhandledMethodError(string traceId, string message) =>
            $"[{traceId}] (UnhandledError) {Environment.NewLine + message}";

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