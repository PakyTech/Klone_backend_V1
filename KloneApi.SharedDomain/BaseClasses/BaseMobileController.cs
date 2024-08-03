using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace KloneApi.SharedDomain.BaseClasses;

[AutoValidation]
public class BaseMobileController : ControllerBase
{
    protected IMediator mediator;
    
    public BaseMobileController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
}
