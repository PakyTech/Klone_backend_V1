using KloneApi.Identity.Command;
using KloneApi.SharedDomain.BaseClasses;
using KloneApi.SharedDomain.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KloneApi.Identity.Controllers;

[Route("api/v1/onboarding")]
public class OnboardingController: BaseMobileController
{
    public OnboardingController(IMediator mediator):base(mediator)
    {
        
    }

    [HttpPost("signup-verification")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifySignUp.Request code)
    {
        var result = await mediator.Send(code);
        
        if(result.IsSuccess) return Ok(new {result.Message});
        
        return BadRequest( new ErrorResponse {ErrorDescription = result.Message!});
    }


    [HttpPost("send-verification-code")]
    public async Task<IActionResult> SendVerificationCode(string phone)
    {
        return Ok(new { message = "code sent"});
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUp.Request request)
    {
        var result = await mediator.Send(request);
        if(result.IsSuccess) return Ok( result);

        return BadRequest(new ErrorResponse   { ErrorDescription = result.Message!});
    }

    [HttpPost("create-pin")]
    public async Task<IActionResult> CreatePin([FromBody] CreatePin.Request request,CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request,cancellationToken);
        if(response.IsSuccess) return Ok( new {response.Message});

        return BadRequest(new ErrorResponse {ErrorDescription = response.Message!});
    }   

    [HttpGet("test")]
    public IActionResult Test() =>  Ok(new {message = "we are good to go"});

}