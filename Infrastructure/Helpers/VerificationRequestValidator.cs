using FluentValidation;
using Infrastructure.Models;

namespace Infrastructure.Helpers;

public class VerificationRequestValidator : AbstractValidator<VerificationRequest>
{
    public VerificationRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.VerificationCode).NotEmpty();
    }
}
