using System;
using FluentValidation;

namespace PropertyBase.Features.Properties.SaveDraft
{
    public class SaveDraftValidator : AbstractValidator<SaveDraftRequest>
    {
        public SaveDraftValidator()
        {
            RuleFor(c => c.Title).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.Description).NotEmpty()
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
                .Must(c=>c>0)
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

            RuleFor(c => c.ParkingSpace).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .Must(c => c > 0)
                .NotNull();

            RuleFor(c => c.TotalLandArea).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .Must(c => c > 0)
                .NotNull();

            RuleFor(c => c.PriceType)
                .NotNull()
                .WithMessage("{PropertyName} is required.");

            RuleFor(c => c.PropertyType)
                .NotNull()
                .WithMessage("{PropertyName} is required.");
        }
    }
}

