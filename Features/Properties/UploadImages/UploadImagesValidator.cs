using System;
using FluentValidation;

namespace PropertyBase.Features.Properties.UploadImages
{
    public class UploadImagesValidator : AbstractValidator<UploadImagesRequest>
    {
        public UploadImagesValidator()
        {
            RuleFor(c => c.PropertyId).NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleForEach(c => c.Files)
                .NotEmpty()
                .Must(c => c.Length <= 10485760)
                .WithMessage("Each file's size must not exceed 10MB.");
               
        }
    }
}

