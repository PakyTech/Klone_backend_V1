using KloneApi.SharedDomain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace KloneApi.SharedDomain.Middlewares;

public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ResponseWrapperMiddleware> logger;

    public ResponseWrapperMiddleware(RequestDelegate next,ILogger<ResponseWrapperMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if(context.Request.ContentType == "application/grpc")
        {
            await next(context);
            return;
        }
        
        var currentBody = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next(context);

        context.Response.Body = currentBody;
        memoryStream.Seek(0, SeekOrigin.Begin);
        using var streamReader = new StreamReader(memoryStream);
        var responseBody = streamReader.ReadToEnd();
        
        context.Response.Headers.ContentLength = null;

        try
        {
            var isError = IsErrorStatus(context.Response.StatusCode);
            if (isError && !string.IsNullOrWhiteSpace(responseBody))
            {
                var response = CommonApiResponse<object>.Create((HttpStatusCode)context.Response.StatusCode, null, responseBody);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                return;
            }

            var objResult = JsonConvert.DeserializeObject(responseBody);
            var result = CommonApiResponse<object>.Create((HttpStatusCode)context.Response.StatusCode, objResult, null);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
        catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            var result = CommonApiResponse<object>.Create(HttpStatusCode.InternalServerError, null, null);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }

    public static bool IsErrorStatus(int httpStatusCode) =>  httpStatusCode >= 400 && httpStatusCode <= 599;
}

public sealed class CommonApiResponse<T>
{
    CommonApiResponse(HttpStatusCode statusCode, T? result = default, string? errorMessage = null)
    {
        StatusCode = (int)statusCode;
        Result = result;
       
        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            var errObj = JsonConvert.DeserializeObject(errorMessage);
            if (errObj is JObject)
            {
                ErrorMessage = JsonConvert.DeserializeObject<ErrorResponse>(errorMessage)?.ErrorDescription;
                return;
            }
            ErrorMessage = errorMessage;
        }
    
    }

    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonProperty("result")]
    public T? Result { get; set; }

    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    public static CommonApiResponse<object> Create(HttpStatusCode statusCode, object? result = null, string? errorMessage = null)
    {
        return new CommonApiResponse<object>(statusCode, result, errorMessage);
    }
}
