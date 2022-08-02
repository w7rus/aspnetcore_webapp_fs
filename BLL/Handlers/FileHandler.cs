using System.Net.Mime;
using BLL.Handlers.Base;
using BLL.Services;
using Common.Models;
using Common.Models.Base;
using DTO.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IFileHandler
{
    Task<DTOResultBase> Create(string fileNameOriginal, Stream stream, CancellationToken cancellationToken = default);
    DTOResultBase Read(FileRead data, CancellationToken cancellationToken = default);
    DTOResultBase Delete(FileDelete data);
}

public class FileHandler : HandlerBase, IFileHandler
{
    #region Ctor

    public FileHandler(ILogger<FileHandler> logger, IFileService fileService, IHttpContextAccessor httpContextAccessor)
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _fileService = fileService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Fields

    private readonly string _fullName;
    private readonly ILogger<FileHandler> _logger;
    private readonly IFileService _fileService;
    private readonly HttpContext _httpContext;

    #endregion

    #region Methods

    public async Task<DTOResultBase> Create(
        string fileNameOriginal,
        Stream stream,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Create)));

        var fileInfo = new FileInfo(fileNameOriginal);
        var fileName = Guid.NewGuid() + fileInfo.Extension;
        var file = await _fileService.Save(fileName, stream, cancellationToken);

        _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Create)));

        return new FileCreateResult
        {
            FileName = file.fileName,
            Size = file.size,
            FileNamePreview = file.fileNamePreview
        };
    }

    public DTOResultBase Read(FileRead data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        var fileStream = _fileService.Read(data.FileName, cancellationToken);

        var contentTypeProvider = new FileExtensionContentTypeProvider();
        if (!contentTypeProvider.TryGetContentType(data.FileName, out var contentType))
            contentType = "application/octet-stream";

        var contentDisposition = new ContentDisposition
        {
            FileName = data.FileName,
            Inline = true
        };
        _httpContext.Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

        _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Read)));

        return new FileReadResult
        {
            FileStream = fileStream,
            FileName = data.FileName,
            ContentType = contentType
        };
    }

    public DTOResultBase Delete(FileDelete data)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Delete)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        _fileService.Delete(data.FileName);

        _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Delete)));

        return new FileDeleteResult();
    }

    #endregion
}