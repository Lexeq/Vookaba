using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace OakChan.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private const string defaultErrorMessage = "Max size for {0} is {1}.";
        public long MaxSize { get; }


        public MaxFileSizeAttribute(long maxSize)
            : base()
        {
            MaxSize = maxSize;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if(value is IFormFile file)
            {
                return file.Length < MaxSize;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.InvariantCulture, defaultErrorMessage, name, MaxSize);
        }

    }
}
