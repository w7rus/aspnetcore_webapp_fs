﻿using Common.Enums;
using Common.Models.Base;

namespace Common.Models;

public class WarningModelResultEntry
{
    public WarningModelResultEntry(
        WarningType warningType,
        string message,
        WarningEntryType warningEntryType = WarningEntryType.None
    )
    {
        WarningType = warningType;
        Message = message;
        WarningEntryType = warningEntryType;
    }

    public WarningType WarningType { get; }
    public string Message { get; }
    public WarningEntryType WarningEntryType { get; }
}

public interface IWarningModelResult
{
    public List<WarningModelResultEntry> Warnings { get; set; }
}

public class WarningModelResult : DTOResultBase
{
    public WarningModelResult()
    {
        Warnings = new List<WarningModelResultEntry>();
    }
}