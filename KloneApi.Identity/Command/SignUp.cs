using MediatR;
using BCrypt.Net;
using KloneApi.SharedDomain.Persistence;
using KloneApi.SharedDomain.Entities.Identity;
using KloneApi.SharedDomain;
using KloneApi.SharedDomain.Middlewares;
using FluentValidation;
using FluentValidation.Results;

namespace KloneApi.Identity.Command;
public static class SignUp
{
    
    public class SignUpRequestValidator : AbstractValidator<Request>
    {
        //TODO: add validations for phone and password
        public SignUpRequestValidator()
        {
            RuleFor((x) => x.Email).EmailAddress();
        }
    }
    public class Request : IRequest<Result>
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public string? VerifiedPassword {get; set;}
        public string? Pin { get; set; }
        public string? Phone { get; set; }
    }

    public class Result: BaseResult<string> {}


    public class Handler : IRequestHandler<Request,Result>
    {
        private readonly KloneDbContext kloneDbContext;

        public Handler(KloneDbContext kloneDbContext)
        {
            this.kloneDbContext = kloneDbContext;
        }

        public async Task<Result> Handle(Request request,CancellationToken cancellationToken = default)
        {
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password, 13);
            var user = new User
            {
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                Phone = request.Phone!,
                Pin = request.Pin!,
                Email = request.Email!,
                Password = passwordHash!,
                CreatedBy = request.Email!,
            };

            kloneDbContext.Add(user);
            var rowsAffected = await kloneDbContext.SaveChangesAsync();
            if(rowsAffected == 0) throw new CustomException("Error occurred while saving your details; please retry");

            return new Result { Message = "Sign up was successful", IsSuccess = true };
        }

    }
}