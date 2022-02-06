using Microsoft.AspNetCore.Http;
using OakChan.Attributes;
using OakChan.Common;
using System.ComponentModel.DataAnnotations;

namespace OakChan.ViewModels
{
    public class PostFormViewModel
    {
        [Display(Name = "Name")]
        [MaxLength(OakConstants.PostConstants.NameMaxLength, ErrorMessage = "Name max length is {1}.")]
        public string Name { get; set; }

        [Display(Name = "Message")]
        [RequiredOr(nameof(Image), ErrorMessage = "{0} or {1} is required")]
        [MaxLength(OakConstants.PostConstants.MessageMaxLength, ErrorMessage = "Message max length is {1}.")]
        public string Text { get; set; }

        [Display(Name = "Image")]
        [AllowTypes(".jpg", ".jpeg", ".png", ".gif", ErrorMessage = "File type not supported.")]
        [MaxFileSize(OakConstants.PostConstants.MaxFileSize, ErrorMessage = "Max file size is {1}")]
        public IFormFile Image { get; set; }

        [Display(Name = "Sage")]
        public bool IsSaged { get; set; }
    }
}
