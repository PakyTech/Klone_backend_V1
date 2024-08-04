using KloneApi.Identity.Command;
using KloneApi.SharedDomain.BaseClasses;
using KloneApi.SharedDomain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KloneApi.Identity.Controllers;

[Route("api/v1/onboarding")]
public class OnboardingController: BaseMobileController
{
    public OnboardingController(IMediator mediator):base(mediator)
    {
        
    }

    [HttpPost("code-verification/{code}")]
    public async Task<IActionResult> VerifyCode(string code)
    {
        if(code == "123456")
            return Ok(new {message = "verification was successful"});

        return BadRequest( new ErrorResponse {ErrorDescription = "verification failed"});
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

}