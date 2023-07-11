using System;
using FluentValidation;

namespace PropertyBase.Features.Properties.DeleteImage
{
    public class DeleteImageValidator : AbstractValidator<DeleteImageRequest>
    {
        public DeleteImageValidator()
        {
            RuleFor(c => c.PropertyId).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(c => c.FileId).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();
        }
    }
}

