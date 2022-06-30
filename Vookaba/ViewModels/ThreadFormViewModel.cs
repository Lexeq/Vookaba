using Microsoft.AspNetCore.Http;
using Vookaba.Attributes;
using Vookaba.Common;
using System.ComponentModel.DataAnnotations;

namespace Vookaba.ViewModels
{
    public class ThreadFormViewModel
    {
        [Display(Name = "Subject")]
        [MaxLength(ApplicationConstants.ThreadConstants.SubjectMaxLength, ErrorMessage = "Subject max length is {1}.")]
        public string Subject { get; set; }

        [Display(Name = "Name")]
        [MaxLength(ApplicationConstants.PostConstants.NameMaxLength, ErrorMessage = "Name max length is {1}.")]
        public string Name { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "Message is required.")]
        [MaxLength(ApplicationConstants.PostConstants.MessageMaxLength, ErrorMessage = "Message max length is {1}.")]
        public string Text { get; set; }

        [Display(Name = "Image")]
        [Required(ErrorMessage = "Image is required.")]
        [AllowTypes(".jpg", ".jpeg", ".png", ".gif", ErrorMessage = "File type not supported.")]
        [MaxFileSize(ApplicationConstants.ThreadConstants.MaxFileSize, ErrorMessage = "Max file size is {1}")]
        public IFormFile Image { get; set; }
    }
}
