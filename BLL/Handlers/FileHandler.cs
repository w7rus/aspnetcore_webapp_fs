using BLL.Handlers.Base;
using BLL.Services;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DTO.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IFileHandler
{
    Task<DTOResultBase> Create(FileCreate data, IFormFile file, CancellationToken cancellationToken);
    Task<DTOResultBase> Read(FileRead data, CancellationToken cancellationToken);
    DTOResultBase Delete(FileDelete data, CancellationToken cancellationToken);
}

public class FileHandler : HandlerBase, IFileHandler
{
    #region Fields

    private readonly string _fullName;
    private readonly ILogger<FileHandler> _logger;
    private readonly IFileService _fileService;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public FileHandler(ILogger<FileHandler> logger, IFileService fileService, IHttpContextAccessor httpContextAccessor)
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _fileService = fileService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    public async Task<DTOResultBase> Create(FileCreate data, IFormFile file, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Create)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            var fileInfo = new FileInfo(file.FileName);
            var fileName = Guid.NewGuid() + fileInfo.Extension;
            var ms = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(ms, cancellationToken);
            await _fileService.Save(fileName, ms.ToArray(), cancellationToken);

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Create)));

            return new FileCreateResult
            {
                FileName = fileName
            };
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(Create), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.File, Localize.Error.CreateFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.File, e.Message));

            return errorModelResult;
        }
    }

    public async Task<DTOResultBase> Read(FileRead data, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            var fileData = await _fileService.Read(data.FileName, cancellationToken);

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (!contentTypeProvider.TryGetContentType(data.FileName, out var contentType))
                contentType = "application/octet-stream";

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = data.FileName,
                Inline = true,
            };
            _httpContext.Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Read)));

            return new FileReadResult
            {
                Data = fileData,
                ContentType = contentType
            };
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(Read), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.File, Localize.Error.ReadFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.File, e.Message));

            return errorModelResult;
        }
    }

    public DTOResultBase Delete(FileDelete data, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Delete)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            _fileService.Delete(data.FileName);

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Delete)));

            return new FileDeleteResult();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(Delete), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.File, Localize.Error.DeleteFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.File, e.Message));

            return errorModelResult;
        }
    }

    #endregion
}