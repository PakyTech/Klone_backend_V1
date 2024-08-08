using KloneApi.SharedDomain;
using MediatR;

namespace KloneApi.Identity.Command;

public static class VerifySignUp
{
    public class Request : IRequest<BaseResult>
    {
        public string Code {get;set;}

        public string Email {get;set;}
    }

    public class Handler : IRequestHandler<Request, BaseResult>
    {
        public async Task<BaseResult> Handle(Request request, CancellationToken cancellationToken)
        {
            if(request.Code == "123456") return new BaseResult{ IsSuccess = true, Message = "Verification was successful"};

            return new BaseResult {Message = "Verification code was incorrect"};
        }
    }

}