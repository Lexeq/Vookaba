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
            return attribute switch
            {
                RequiredOrAttribute requiredAttribute => new RequiredOrAdatpter(requiredAttribute, stringLocalizer),
                MaxFileSizeAttribute maxFileSizeAttribute => new MaxFileSizeAttributeAdapter(maxFileSizeAttribute, stringLocalizer),
                AllowTypesAttribute allowTypesAttribute => new AllowTypesAttributeAdapter(allowTypesAttribute, stringLocalizer),
                _ => defaultProvider.GetAttributeAdapter(attribute, stringLocalizer)
            };
        }
    }
}
