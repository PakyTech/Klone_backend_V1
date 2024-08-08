using MediatR;
using BCrypt.Net;
using KloneApi.SharedDomain.Persistence;
using KloneApi.SharedDomain.Entities.Identity;
using KloneApi.SharedDomain;
using KloneApi.SharedDomain.Middlewares;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace KloneApi.Identity.Command;
public static class SignUp
{
    
    public class SignUpRequestValidator : AbstractValidator<Request>
    {
        //TODO: Add column for country code
        public SignUpRequestValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Phone).Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number format is (+234xxxxxxxxxx)");
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.ConfirmPassword).NotEmpty();
            RuleFor(x => x.Pin).Empty();
            RuleFor(x => x).Must(x => x.Password == x.ConfirmPassword).WithMessage("Password and Confirm Password must be the same.");
        }
    }
    public class Request : IRequest<Result>
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword {get; set;}
        public string? Pin { get; set; }
        public string? Phone { get; set; }
    }

    public class Result : BaseResult
    {
        public bool IsAccountVerified {get; set;}
    }


    public class Handler : IRequestHandler<Request,Result>
    {
        private readonly KloneDbContext kloneDbContext;

        public Handler(KloneDbContext kloneDbContext)
        {
            this.kloneDbContext = kloneDbContext;
        }

        public async Task<Result> Handle(Request request,CancellationToken cancellationToken = default)
        {
            var userInDb = await kloneDbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email || x.Phone == request.Phone).ConfigureAwait(true);
            if(userInDb != null && userInDb.Email == request.Email)
                return new Result  { IsSuccess = true, Message = "You already have an account with us.", IsAccountVerified = userInDb.IsAccountVerified};
                
            if(userInDb != null && userInDb.Phone == request.Phone)
                return new Result  { IsSuccess = false, Message = "Phone number is already taken", IsAccountVerified = userInDb.IsAccountVerified};

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
            var rowsAffected = await kloneDbContext.SaveChangesAsync().ConfigureAwait(true);
            if(rowsAffected == 0) throw new CustomException("Error occurred while saving your details; please retry");

            //TODO: send verification code

            return new Result { Message = "Sign up was successful", IsSuccess = true };
        }
    }
}