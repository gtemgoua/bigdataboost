using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigDataBoost.API.ViewModels.Validations
{
    public class TagHistViewModelValidator : AbstractValidator<TagHistViewModel>
    {
        public TagHistViewModelValidator()
        {
            RuleFor(rec => rec.TagDefId).NotEmpty().WithMessage("TagDefId cannot be empty");
            RuleFor(rec => rec.Value).NotEmpty().WithMessage("Value cannot be empty");
            RuleFor(rec => rec.TagName).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(rec => rec.TimeStamp).NotEmpty().WithMessage("Time entry cannot be empty");
        }
    }
}
