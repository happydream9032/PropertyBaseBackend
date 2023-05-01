using System;
using FluentValidation;

namespace PropertyBase.Features.Properties.PublishProperty
{
    public class PublishPropertyValidator : AbstractValidator<PublishPropertyRequest>
    {
        public PublishPropertyValidator()
        {
            RuleFor(c => c.PropertyId).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();
        }
    }
}

