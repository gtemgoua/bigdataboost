using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigDataBoost.API.ViewModels.Validations
{
    public class TagDefViewModelValidator : AbstractValidator<TagDefViewModel>
    {
        public TagDefViewModelValidator()
        {
            RuleFor(tag => tag.Source).NotEmpty().WithMessage("Source cannot be empty");
            RuleFor(tag => tag.Name).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(tag => tag.Description).NotEmpty().WithMessage("Description cannot be empty");
        }
    }
}
