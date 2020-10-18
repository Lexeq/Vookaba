using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OakChan.Attributes
{
    public class MaxFileSizeAttributeAdapter : AttributeAdapterBase<MaxFileSizeAttribute>
    {
        private readonly IReadOnlyDictionary<int, string> suffixes = new Dictionary<int, string>
        {
            [0] = "Bytes",
            [1] = "KB",
            [2] = "MB",
            [3] = "GB",
            [4] = "TB"
        };

        private readonly IStringLocalizer stringLocalizer;
        private readonly long maxSize;

        public MaxFileSizeAttributeAdapter(MaxFileSizeAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            maxSize = attribute.MaxSize;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-maxsize"] = GetErrorMessage(context);
            context.Attributes["data-val-maxsize-size"] = maxSize.ToString();
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(
                validationContext.ModelMetadata,
                validationContext.ModelMetadata.DisplayName,
                SimplifySize(maxSize));
        }

        private string SimplifySize(long bytes)
        {
            var factor = (int)(Math.Log2(bytes) / 10);
            var maxKnownFactor = suffixes.Keys.Max();
            factor = factor > maxKnownFactor ? maxKnownFactor : factor;
            float value = bytes / MathF.Pow(2, factor * 10);
            return Math.Round(value, 1).ToString() + stringLocalizer[suffixes[factor]];
        }
    }
}
