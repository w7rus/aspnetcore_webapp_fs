#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Enums;
using Common.Models;
using Common.Models.Base;

namespace BLL.Handlers.Base;

public interface IHandlerBase
{
    public ErrorModelResult? ValidateModel(object data);
}

public class HandlerBase : IHandlerBase
{
    public ErrorModelResult? ValidateModel(object data)
    {
        var context = new ValidationContext(data);
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateObject(data, context, validationResults, true))
            return null;

        var errorModelResult = new ErrorModelResult
        {
            Errors = new List<ErrorModelResultEntry>()
        };

        foreach (var validationResult in validationResults)
        {
            errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.ModelState, validationResult.ErrorMessage));
        }

        return errorModelResult;
    }
}