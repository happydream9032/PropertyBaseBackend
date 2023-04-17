using System;
using FluentValidation;

namespace PropertyBase.DTOs.Property.AddProperty
{
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(c => c.Title).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.Locality).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.Street).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.Price).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.NumberOfBathrooms).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.NumberOfBedrooms).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.NumberOfToilets).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.PriceType)
                .NotNull()
                .WithMessage("{PropertyName} is required.");

            RuleFor(c => c.PropertyType)
                .NotNull()
                .WithMessage("{PropertyName} is required.");

            RuleForEach(c => c.files).Must(c => c.Length <= 10485760)
                .WithMessage("Each file's size must not exceed 10MB");
        }
    }

   
}

