using System.Collections.Generic;
using Common.Enums;

namespace Common.Models.Base;

public class DTOResultBase : IWarningModelResult, IErrorModelResult
{
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}