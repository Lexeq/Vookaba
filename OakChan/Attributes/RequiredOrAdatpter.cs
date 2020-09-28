using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace OakChan.Attributes
{
    public class RequiredOrAdatpter : AttributeAdapterBase<RequiredOrAttribute>
    {
        public RequiredOrAdatpter(RequiredOrAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        { }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-requiredor", GetErrorMessage(context));
            context.Attributes.Add("data-val-requiredor-other", Attribute.OtherProperty);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            var otherDisplayName = validationContext.MetadataProvider
                .GetMetadataForProperty(validationContext.ModelMetadata.ContainerType, Attribute.OtherProperty)
                .DisplayName;
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.DisplayName, otherDisplayName);
        }
    }
}
