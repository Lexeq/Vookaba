using Microsoft.AspNetCore.Http;
using OakChan.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OakChan.ViewModels
{
    public class PostFormViewModel : IPostFormViewModel
    {
        [Required]
        public string Board { get; set; }

        [Required]
        public int? Thread { get; set; }

        [Display(Name = "Subject")]
        [MaxLength(32, ErrorMessage = "Subject max length is {1}.")]
        public string Subject { get; set; }

        [Display(Name = "Name")]
        [MaxLength(32, ErrorMessage = "Name max length is {1}.")]
        public string Name { get; set; }

        [Display(Name = "Message")]
        [RequiredOr(nameof(Image), ErrorMessage = "{0} or {1} is required")]
        [MaxLength(4096, ErrorMessage = "Message max length is {1}.")]
        public string Text { get; set; }

        [Display(Name = "Image")]
        [AllowTypes(".jpg", ".jpeg", ".png", ".gif", ErrorMessage = "File type not supported.")]
        [MaxFileSize(1 * 1024 * 1024, ErrorMessage = "Max file size is {1}")]
        public IFormFile Image { get; set; }
    }
}
