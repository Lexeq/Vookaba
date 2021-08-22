using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OakChan.Tests.Base
{
    public class ViewModelValidationTestsBase
    {
        protected bool ValidateViewModel(object vm, out IEnumerable<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            var context = new ValidationContext(vm, null, null);
            return Validator.TryValidateObject(vm, context, (ICollection<ValidationResult>)validationResults, true);
        }
    }
}
