using System.Collections.Generic;

namespace Common.Models.Base;

public class DTOResultBase : IWarningModelResult, IErrorModelResult
{
    public List<KeyValuePair<string, string>> Warnings { get; set; }
    public List<KeyValuePair<string, string>> Errors { get; set; }
}