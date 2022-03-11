using System;
using Common.Enums;
using Microsoft.AspNetCore.Http;

namespace Common.Exceptions;

public class HttpResponseException : Exception
{
    public int StatusCode { get; }
    public ErrorType Type { get; }

    public HttpResponseException(int? statusCode, ErrorType? type, string message = null, Exception innerException = null) : base(message, innerException)
    {
        StatusCode = statusCode ?? StatusCodes.Status500InternalServerError;
        Type = type ?? ErrorType.None;
    }
}