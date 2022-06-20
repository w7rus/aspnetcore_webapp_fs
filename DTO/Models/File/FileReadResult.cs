using Common.Models.Base;

namespace DTO.Models.File;

public class FileReadResult : DTOResultBase
{
    public FileStream FileStream { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}