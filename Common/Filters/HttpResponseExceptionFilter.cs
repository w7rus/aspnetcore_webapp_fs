using System.Diagnostics;
using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.Filters;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is HttpResponseException httpResponseException)
        {
            var errorModelResult = new ErrorModelResult
            {
                TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
            };

            errorModelResult.Errors.Add(new ErrorModelResultEntry(httpResponseException.Type,
                httpResponseException.Message));

            context.Result = new ObjectResult(errorModelResult)
            {
                StatusCode = httpResponseException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }

    public int Order => int.MaxValue - 10;
}