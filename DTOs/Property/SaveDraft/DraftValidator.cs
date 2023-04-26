using System;
using FluentValidation;

namespace PropertyBase.DTOs.Property.SaveDraft
{
    public class DraftValidator : AbstractValidator<DraftRequest>
    {
        public DraftValidator()
        {
            //RuleForEach(c => c.files).Must(c => c.Length <= 10485760)
            //    .WithMessage("Each file's size must not exceed 10MB");
        }
    }
}

