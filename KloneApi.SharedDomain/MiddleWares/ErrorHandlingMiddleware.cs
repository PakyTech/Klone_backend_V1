
using System.Net;
using KloneApi.SharedDomain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KloneApi.SharedDomain.Middlewares;
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ErrorHandlingMiddleware> logger;


    public ErrorHandlingMiddleware(RequestDelegate next,ILogger<ErrorHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;

    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        logger.LogError(ex,ex.ToString());
        var code = HttpStatusCode.InternalServerError;
        var errorMessageDisplayedToUser = "An error occured please try again later.";

        if(ex is CustomException) 
            errorMessageDisplayedToUser = ex.Message;
        
        if(ex is not CustomException)
        {
            //TODO: log error
        }

        var result = JsonConvert.SerializeObject(new ErrorResponse
        {
            ErrorDescription = errorMessageDisplayedToUser

        });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}

public class CustomException : Exception
{
    public CustomException(string message) : base(message)
    {
    }

    public CustomException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CustomException()
    {
    }
}
