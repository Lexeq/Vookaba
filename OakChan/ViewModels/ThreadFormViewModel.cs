using Microsoft.AspNetCore.Http;
using OakChan.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OakChan.ViewModels
{
    public class ThreadFormViewModel
    {
        [Display(Name = "Subject")]
        [MaxLength(32, ErrorMessage = "Subject max length is {1}.")]
        public string Subject { get; set; }

        [Display(Name = "Name")]
        [MaxLength(32, ErrorMessage = "Name max length is {1}.")]
        public string Name { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "Message is required.")]
        [MaxLength(4096, ErrorMessage = "Message max length is {1}.")]
        public string Text { get; set; }

        [Display(Name = "Image")]
        [Required(ErrorMessage = "Image is required.")]
        [AllowTypes(".jpg", ".jpeg", ".png", ".gif", ErrorMessage = "File type not supported.")]
        [MaxFileSize(1536 * 1024, ErrorMessage = "Max file size is {1}")]
        public IFormFile Image { get; set; }
    }
}
