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

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUp.Request request)
    {
        var result = await mediator.Send(request);
        if(result.IsSuccess) return Ok( new {result.Message});

        return BadRequest(new ErrorResponse   { ErrorDescription = result.Message});
    }

}