namespace Common.Models.Base;

public interface IErrorModelResult
{
    public List<KeyValuePair<string, string>> Errors { get; set; }
}

public class ErrorModelResult : DTOResultBase
{
}