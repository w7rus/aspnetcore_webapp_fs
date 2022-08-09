namespace Common.Models;

public static class Localize
{
    public static class Error
    {
        #region UnhandledException

        public const string UnhandledExceptionContactSystemAdministrator =
            "UnhandledExceptionContactSystemAdministrator";

        #endregion

        #region File

        public const string FileCreateFailed = "FileCreateFailed";
        public const string FileReadFailed = "FileReadFailed";
        public const string FileDeleteFailed = "FileDeleteFailed";

        #endregion

        #region Request

        public const string RequestMultipartExpected = "RequestMultipartExpected";
        public const string RequestContentTypeBoundaryNotFound = "RequestContentTypeBoundaryNotFound";
        public const string RequestMultipartBoundaryLengthExceedsLimit = "RequestMultipartBoundaryLengthExceedsLimit";
        public const string RequestMultipartSectionNotFound = "RequestMultipartSectionNotFound";
        public const string RequestContentDispositionParseFailed = "RequestContentDispositionParseFailed";
        public const string RequestContentDispositionFileExpected = "RequestContentDispositionFileExpected";

        #endregion

        #region Generic

        public static string ObjectDeserializationFailed => "ObjectDeserializationFailed";
        public static string ObjectCastFailed => "ObjectCastFailed";

        #endregion

        #region AccessToken

        public static string AccessTokenNotProvided => "AccessTokenNotProvided";

        #endregion
    }

    public static class Warning
    {
    }

    public static class Log
    {
        #region Middleware

        public static string MiddlewareForwardStart(string assemblyName)
        {
            return $"[Middleware] {assemblyName} (Forward-Start)";
        }

        public static string MiddlewareForwardEnd(string assemblyName)
        {
            return $"[Middleware] {assemblyName} (Forward-End)";
        }

        public static string MiddlewareBackwardStart(string assemblyName)
        {
            return $"[Middleware] {assemblyName} (Backward-End)";
        }

        public static string MiddlewareBackwardEnd(string assemblyName)
        {
            return $"[Middleware] {assemblyName} (Backward-End)";
        }

        #endregion

        #region Method

        public static string MethodStart(string assemblyName, string methodName)
        {
            return $"[{assemblyName}.{methodName}] (Start)";
        }

        public static string Method(string assemblyName, string methodName, string message)
        {
            return $"[{assemblyName}.{methodName}] {message}";
        }

        public static string MethodEnd(string assemblyName, string methodName)
        {
            return $"[{assemblyName}.{methodName}] (End)";
        }

        public static string MethodError(string assemblyName, string methodName, string message)
        {
            return $"[{assemblyName}.{methodName}] (Error) {Environment.NewLine + message}";
        }

        public static string UnhandledMethodError(string traceId, string message)
        {
            return $"[{traceId}] (UnhandledError) {Environment.NewLine + message}";
        }

        #endregion

        #region Background Service

        public static string BackgroundServiceStarting(string assemblyName)
        {
            return $"[{assemblyName}] (Starting)";
        }

        public static string BackgroundServiceStopping(string assemblyName)
        {
            return $"[{assemblyName}] (End)";
        }

        public static string BackgroundServiceWorking(string assemblyName)
        {
            return $"[{assemblyName}] (Working)";
        }

        public static string BackgroundServiceError(string assemblyName, string message)
        {
            return $"[{assemblyName}] (Error) {Environment.NewLine + message}";
        }

        #endregion
    }
}