using System.ComponentModel.DataAnnotations;
using System.Net;
using API.Controllers.Base;
using BLL.Handlers;
using Common.Attributes;
using Common.Enums;
using Common.Exceptions;
using Common.Helpers;
using Common.Models;
using Common.Options;
using DTO.Models.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;
using AuthenticationSchemes = Common.Models.AuthenticationSchemes;

namespace API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status400BadRequest)]
public class FileController : CustomControllerBase
{
    #region Ctor

    public FileController(
        ILogger<FileController> logger,
        IFileHandler fileHandler,
        IOptions<MiscOptions> miscOptions,
        IHttpContextAccessor httpContextAccessor
    ) : base(httpContextAccessor)
    {
        _logger = logger;
        _fileHandler = fileHandler;
        _miscOptions = miscOptions.Value;
    }

    #endregion

    #region Fields

    private readonly ILogger<FileController> _logger;
    private readonly IFileHandler _fileHandler;
    private readonly MiscOptions _miscOptions;

    #endregion

    #region Endpoints

    [DisableFormValueModelBinding]
    [RequestSizeLimit(1073741824L)] //1GB
    [RequestFormLimits(MultipartBodyLengthLimit = 1073741824L)] //1GB
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.AccessToken)]
    [SwaggerOperation(Summary = "Creates file",
        Description = "Creates file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestMultipartExpected);

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

        var fileName = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
        await using var fileStream = multipartSection.Body;
        return ResponseWith(await _fileHandler.Create(fileName, fileStream, cancellationToken));
    }

    [HttpGet]
    [AllowAnonymous]
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
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.AccessToken)]
    [SwaggerOperation(Summary = "Deletes file",
        Description = "Deletes file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(
        [Required] [FromQuery] FileDelete data
    )
    {
        return ResponseWith(_fileHandler.Delete(data));
    }

    #endregion
}