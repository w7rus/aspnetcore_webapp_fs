namespace Common.Models.Base;

public interface IWarningModelResult
{
    public List<KeyValuePair<string, string>> Warnings { get; set; }
}

public class WarningModelResult : DTOResultBase
{
}