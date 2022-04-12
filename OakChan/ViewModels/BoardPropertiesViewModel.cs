using OakChan.Common;
using System.ComponentModel.DataAnnotations;

namespace OakChan.ViewModels
{
    public class BoardPropertiesViewModel
    {
        [Required]
        [RegularExpression("[a-zA-Z]+", ErrorMessage = "Only Latin letters are allowed.")]
        [StringLength(OakConstants.BoardConstants.MaxKeyLength, MinimumLength = 1)]
        public string BoardKey { get; set; }

        [Required]
        [StringLength(OakConstants.BoardConstants.MaxNameLength, MinimumLength = 2)]
        public string Name { get; set; }

        public bool IsHidden { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsReadOnly { get; set; }

        public int BumpLimit { get; set; }
    }
}
