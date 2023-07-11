using System;
using FluentValidation;

namespace PropertyBase.Features.Properties.UpdateProperty
{
    public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyRequest>
    {
        public UpdatePropertyValidator()
        {
            RuleFor(c => c.PropertyId).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();
        }
    }
}

