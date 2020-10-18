using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace OakChan.Attributes
{
    public class AllowTypesAttributeAdapter : AttributeAdapterBase<AllowTypesAttribute>
    {
        public AllowTypesAttributeAdapter(AllowTypesAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer) { }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-allowedFileTypes"] = GetErrorMessage(context);
            context.Attributes["data-val-allowedFileTypes-types"] = string.Join("|", Attribute.Extensions);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(
                validationContext.ModelMetadata,
                validationContext.ModelMetadata.DisplayName,
                string.Join(", ", Attribute.Extensions));
        }
    }
}
