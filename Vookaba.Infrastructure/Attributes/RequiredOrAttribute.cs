using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Vookaba.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredOrAttribute : ValidationAttribute
    {
        private const string DefaultMessage = "At least one of the prorties must be set ({0} or {1}).";

        public string OtherProperty { get; private set; }

        public bool AllowEmptyStrings { get; set; } = false;

        public RequiredOrAttribute(string otherProperty)
            : base(DefaultMessage)
        {
            OtherProperty = otherProperty;
        }

        public override bool RequiresValidationContext => true;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(OtherProperty);
            if (property == null)
            {
                return new ValidationResult(
                    string.Format(CultureInfo.CurrentCulture, "Can not find property {0}", OtherProperty
                ));
            }
            var otherValue = property.GetValue(validationContext.ObjectInstance, null);

            if (otherValue == null || otherValue as string == string.Empty && !AllowEmptyStrings)
            {
                if (value == null || value as string == string.Empty && !AllowEmptyStrings)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherProperty);
        }
    }
}