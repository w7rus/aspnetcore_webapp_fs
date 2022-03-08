using System.ComponentModel.DataAnnotations;
using API.Controllers.Base;
using BLL.Handlers;
using BrunoZell.ModelBinding;
using Common.Models;
using Common.Models.Base;
using DTO.Models.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ASP.NET_Core_Web_Application_File_Server.Controllers;

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

    [HttpPost]
    [SwaggerOperation(Summary = "Creates file",
        Description = "Creates file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [Required] [FromForm] [ModelBinder(BinderType = typeof(JsonModelBinder))]
        FileCreate data,
        IFormFile file,
        CancellationToken cancellationToken = new()
    )
    {
        return ResponseWith(await _fileHandler.Create(data, file, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Reads file",
        Description = "Reads file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] FileRead data,
        CancellationToken cancellationToken = new()
    )
    {
        return ResponseWith(await _fileHandler.Read(data, cancellationToken));
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