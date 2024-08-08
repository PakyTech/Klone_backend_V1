using FluentValidation;
using KloneApi.SharedDomain;
using KloneApi.SharedDomain.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KloneApi.Identity.Command;

public static class CreatePin 
{
    public class CreatePinValidator: AbstractValidator<Request>
    {
        public CreatePinValidator()
        {
            RuleFor(x => x.EmailAddress).EmailAddress();
            RuleFor(x => x.Pin).MinimumLength(4).MaximumLength(4).Matches(@"^\d{4}$");
        }
    }

    public class Request : IRequest<BaseResult>
    {
        public string Pin {get;set;}
        public string EmailAddress {get;set;}
    }

    public class Handler : IRequestHandler<Request, BaseResult>
    {
        private readonly KloneDbContext kloneDbContext;

        public Handler(KloneDbContext kloneDbContext)
        {
            this.kloneDbContext = kloneDbContext;
        }

        public async Task<BaseResult> Handle(Request request, CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                var existingUser = await kloneDbContext.Users.FirstOrDefaultAsync(x => x.Email == request.EmailAddress).ConfigureAwait(false);
                if(existingUser == null) return new BaseResult{Message = "User was not found"};

                if(!string.IsNullOrWhiteSpace(existingUser.Pin)) return new BaseResult {Message = "You already created your pin"};

                existingUser.Pin = request.Pin;
                var rowsAffected = await kloneDbContext.SaveChangesAsync().ConfigureAwait(false);
                if(rowsAffected < 1) return new BaseResult{Message = "Failed to update pin"}; 

                return new BaseResult{IsSuccess = true, Message = "pin created successfully"};

            }
            return new BaseResult();
        }
    }
}