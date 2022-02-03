using System.Net.Http.Headers;
using Common.Models.Base;

namespace DTO.Models.File;

public class FileReadResult : DTOResultBase
{
    public byte[] Data { get; set; }
    public string ContentType { get; set; }
}