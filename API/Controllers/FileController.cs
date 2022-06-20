using System.ComponentModel.DataAnnotations;
using System.Net;
using API.Controllers.Base;
using BLL.Handlers;
using Common.Attributes;
using Common.Enums;
using Common.Exceptions;
using Common.Helpers;
using Common.Models;
using DTO.Models.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status400BadRequest)]
public class FileController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<FileController> _logger;
    private readonly IFileHandler _fileHandler;

    #endregion

    #region Ctor

    public FileController(
        ILogger<FileController> logger,
        IFileHandler fileHandler,
        IHttpContextAccessor httpContextAccessor
    ) : base(httpContextAccessor)
    {
        _logger = logger;
        _fileHandler = fileHandler;
    }

    #endregion

    #region Endpoints

    [DisableFormValueModelBinding]
    [RequestSizeLimit(134217728L)]
    [RequestFormLimits(MultipartBodyLengthLimit = 134217728L)]
    [HttpPost]
    [SwaggerOperation(Summary = "Creates file",
        Description = "Creates file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestMultipartExpected);
        }
        
        var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType));
        var reader = new MultipartReader(boundary, HttpContext.Request.Body);

        var multipartSection = await reader.ReadNextSectionAsync(cancellationToken);

        if (multipartSection == null)
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestMultipartSectionNotFound);

        if (!ContentDispositionHeaderValue.TryParse(
                multipartSection.ContentDisposition, out var contentDisposition))
            throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Request,
                Localize.Error.RequestContentDispositionParseFailed);

        if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestContentDispositionFileExpected);

        await using var fileStream = multipartSection.Body;
        return ResponseWith(await _fileHandler.Create(WebUtility.HtmlEncode(
            contentDisposition.FileName.Value), fileStream, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Reads file",
        Description = "Reads file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Read(
        [Required] [FromQuery] FileRead data,
        CancellationToken cancellationToken = new()
    )
    {
        return ResponseWith(_fileHandler.Read(data, cancellationToken));
    }

    [HttpDelete]
    [SwaggerOperation(Summary = "Deletes file",
        Description = "Deletes file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(
        [Required] [FromQuery] FileDelete data,
        CancellationToken cancellationToken = new()
    )
    {
        return ResponseWith(_fileHandler.Delete(data));
    }

    #endregion
}