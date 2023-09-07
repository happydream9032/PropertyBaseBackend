using System;
using FluentValidation;

namespace PropertyBase.Features.Properties.SendPropertyInspectionRequest
{
    public class InspectionCommandValidator : AbstractValidator<InspectionCommand>
    {
        public InspectionCommandValidator()
        {
            RuleFor(c => c.PropertyId).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.SenderFullName).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.SenderEmail).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();
        }
    }
}

