namespace Common.Models.Base;

public class DTOResultBase : IWarningModelResult, IErrorModelResult
{
    public string TraceId { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
}