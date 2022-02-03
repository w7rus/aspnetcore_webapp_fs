using System.Collections.Generic;
using Common.Models.Base;

namespace Common.Models;

public interface IErrorModelResult
{
    public List<KeyValuePair<string, string>> Errors { get; set; }
}

public class ErrorModelResult : DTOResultBase
{
}