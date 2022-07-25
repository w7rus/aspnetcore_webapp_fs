using Common.Models.Base;

namespace DTO.Models.File;

public class FileCreateResult : DTOResultBase
{
    public string FileName { get; set; }
    public long Size { get; set; }
}