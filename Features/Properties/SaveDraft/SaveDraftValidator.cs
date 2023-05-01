using System;
using FluentValidation;

namespace PropertyBase.Features.Properties.SaveDraft
{
    public class SaveDraftValidator : AbstractValidator<SaveDraftRequest>
    {
        public SaveDraftValidator()
        {
            RuleFor(c => c.Price).Must(c => c > 0)
                .WithMessage("{PropertyName} must be greater than 0");
            RuleFor(c => c.TotalLandArea).Must(c => c > 0)
                .WithMessage("{PropertyName} must be greater than 0");
        }
    }
}

