using System.Collections.Generic;
using Common.Models.Base;

namespace Common.Models;

public interface IWarningModelResult
{
    public List<KeyValuePair<string, string>> Warnings { get; set; }
}

public class WarningModelResult : DTOResultBase
{
}