using Microsoft.AspNetCore.Http;
using OakChan.Attributes;
using OakChan.Common;
using System.ComponentModel.DataAnnotations;

namespace OakChan.ViewModels
{
    public class ThreadFormViewModel
    {
        [Display(Name = "Subject")]
        [MaxLength(OakConstants.ThreadConstants.SubjectMaxLength, ErrorMessage = "Subject max length is {1}.")]
        public string Subject { get; set; }

        [Display(Name = "Name")]
        [MaxLength(OakConstants.PostConstants.NameMaxLength, ErrorMessage = "Name max length is {1}.")]
        public string Name { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "Message is required.")]
        [MaxLength(OakConstants.PostConstants.MessageMaxLength, ErrorMessage = "Message max length is {1}.")]
        public string Text { get; set; }

        [Display(Name = "Image")]
        [Required(ErrorMessage = "Image is required.")]
        [AllowTypes(".jpg", ".jpeg", ".png", ".gif", ErrorMessage = "File type not supported.")]
        [MaxFileSize(OakConstants.ThreadConstants.MaxFileSize, ErrorMessage = "Max file size is {1}")]
        public IFormFile Image { get; set; }
    }
}
