using BigDataBoost.API.ViewModels.Validations;
using BigDataBoost.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BigDataBoost.API.ViewModels
{
    public class TagDefViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExtendedDescription { get; set; }
        public double Value { get; set; }
        public TagStatus Status { get; set; }
        public DateTime TimeStamp { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new TagDefViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
