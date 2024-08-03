using KloneApi.SharedDomain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

public class ValidationResultMiddleware : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
    {
        var validationErrors = validationProblemDetails?.Errors?.ToList();
        var errorMessage = string.Empty;
        validationErrors?.ForEach(x => errorMessage+= $"; {x.Value[0]}");
        return new BadRequestObjectResult(new ErrorResponse{ ErrorDescription = $"Validation errors: {errorMessage[1..]}"});
    }
}