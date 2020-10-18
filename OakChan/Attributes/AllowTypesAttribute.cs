using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OakChan.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowTypesAttribute : ValidationAttribute
    {
        public IReadOnlyCollection<string> Extensions { get; }

        public AllowTypesAttribute(params string[] extensions)
        {
            Extensions = Array.AsReadOnly(NormalizeExtensions(extensions));
        }

        private string[] NormalizeExtensions(string[] extensions)
        {
            var result = new string[extensions.Length];
            for (int i = 0; i < extensions.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(extensions[i]))
                {
                    throw new ArgumentException("Extension can't be empty string.");
                }

                var ext = extensions[i].ToLowerInvariant();
                result[i] = ext[0] == '.' ? ext : "." + ext;
            }
            return result;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is IFormFile file)
            {
                var normalizedName = file.FileName.ToLowerInvariant();
                return Extensions.Any(x => normalizedName.EndsWith(x));
            }

            return false;
        }
    }
}
