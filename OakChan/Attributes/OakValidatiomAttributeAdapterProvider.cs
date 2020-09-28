using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace OakChan.Attributes
{
    public class OakValidatiomAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        ValidationAttributeAdapterProvider defaultProvider = new ValidationAttributeAdapterProvider();
        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            if (attribute is RequiredOrAttribute requiredAttribute)
            {
                return new RequiredOrAdatpter(requiredAttribute, stringLocalizer);
            }
            return defaultProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
