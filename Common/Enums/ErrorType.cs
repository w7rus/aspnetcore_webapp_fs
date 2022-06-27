namespace Common.Enums;

public enum ErrorType
{
    None = 0,
    Generic,
    Unhandled,
    ModelState,
    HttpContext,
    Request,
    HttpClient
}