using Vookaba.Common;
using Vookaba.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

namespace Vookaba.ViewModels
{
    public class PostsDeletionOptions : IValidatableObject
    {
        public enum DeletingArea
        {
            Single = 0,
            Thread = 1,
            Board = 2,
            All = 3
        }

        [Required, JsonPropertyName("board")]
        public string Board { get; set; }

        [Range(1, int.MaxValue), JsonPropertyName("number")]
        public int PostNumber { get; set; }

        [Required, JsonPropertyName("reason")]
        public string Reason { get; set; }

        [EnumDataType(typeof(DeletingArea))]
        public DeletingArea Area { get; set; }

        public Mode? Mode { get; set; } //flag 1 2 3

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>(1);
            if (Area != DeletingArea.Single)
            {
                if (Mode == null)
                {
                    errors.Add(new ValidationResult($"The {nameof(Mode)} field is required if the selected area is different from 'single'.", new string[] { nameof(Mode) }));
                }
                else if (!Mode.IsValidFlagCombination())
                {
                    errors.Add(new ValidationResult($"The field {nameof(Mode)} is invalid.", new string[] { nameof(Mode) }));
                }
            }
            else
            {
                if(Mode != null)
                {
                    errors.Add(new ValidationResult($"The {nameof(Mode)} field is not available if selected single post deletion.", new string[] { nameof(Mode) }));
                }
            }
            return errors;
        }
    }
}



