using System.Linq;
using System.Threading.Tasks;
using Common.Models.Base;
using DTO.Models.File;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Base;

public class CustomControllerBase : ControllerBase
{
    internal IActionResult ResponseWith(DTOResultBase response)
    {
        if (response.Errors != null && response.Errors.Any())
            return new BadRequestObjectResult(response);
        if (response is FileReadResult result)
        {
            return new FileContentResult(result.Data, result.ContentType);
        }

        return new OkObjectResult(response);
    }
}