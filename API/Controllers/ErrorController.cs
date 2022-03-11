using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    #region Fields

    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<ErrorController> _logger;

    #endregion
    
    #region Ctor

    public ErrorController(IHostEnvironment hostEnvironment, ILogger<ErrorController> logger)
    {
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    #endregion

    #region Methods

    [Route("/Error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        var errorModelResult = new ErrorModelResult
        {
            TraceId = traceId
        };
        
        if (!_hostEnvironment.IsDevelopment())
        {
            errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Unhandled, Localize.Error.UnhandledExceptionContactSystemAdministrator, ErrorEntryType.Message));
            return StatusCode(StatusCodes.Status500InternalServerError, errorModelResult);
        }
        
        errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Unhandled, exceptionHandlerFeature.Error.Message, ErrorEntryType.Message));
        errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Unhandled, exceptionHandlerFeature.Error.StackTrace, ErrorEntryType.StackTrace));
        errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Unhandled, exceptionHandlerFeature.Error.Source, ErrorEntryType.Source));
        errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Unhandled, exceptionHandlerFeature.Path, ErrorEntryType.Path));

        return StatusCode(StatusCodes.Status500InternalServerError, errorModelResult);
    }

    #endregion
}