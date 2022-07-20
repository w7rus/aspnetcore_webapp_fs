using System.Diagnostics;
using Common.Models.Base;
using DTO.Models.File;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Base;

public class CustomControllerBase : ControllerBase
{
    #region Fields

    private readonly HttpContext _httpContext;

    #endregion

    #region Methods

    public CustomControllerBase(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    internal IActionResult ResponseWith(DTOResultBase response)
    {
        response.TraceId = Activity.Current?.Id ?? _httpContext.TraceIdentifier;

        if (response.Errors != null && response.Errors.Any())
            return new BadRequestObjectResult(response);
        if (response is FileReadResult result)
        {
            var file = File(result.FileStream, result.ContentType, result.FileName);
            file.EnableRangeProcessing = true;
            return file;
        }

        return new OkObjectResult(response);
    }

    #endregion
}