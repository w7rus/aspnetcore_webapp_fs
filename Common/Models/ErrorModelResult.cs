using Common.Enums;
using Common.Models.Base;

namespace Common.Models;

public class ErrorModelResultEntry
{
    public ErrorModelResultEntry(
        ErrorType errorType,
        string message,
        ErrorEntryType errorEntryType = ErrorEntryType.None
    )
    {
        ErrorType = errorType;
        Message = message;
        ErrorEntryType = errorEntryType;
    }

    public ErrorType ErrorType { get; }
    public string Message { get; }
    public ErrorEntryType ErrorEntryType { get; }
}

public interface IErrorModelResult
{
    public List<ErrorModelResultEntry> Errors { get; set; }
}

public class ErrorModelResult : DTOResultBase
{
    public ErrorModelResult()
    {
        Errors = new List<ErrorModelResultEntry>();
    }
}