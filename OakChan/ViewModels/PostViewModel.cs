using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OakChan.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace OakChan.ViewModels
{
    public class PostViewModel : IPostViewModel
    {
        [Required]
        public string Board { get; set; }

        [Required]
        public int? Thread { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [RequiredOr(nameof(Image), ErrorMessage = "{0} or {1} is required"), Display(Name = "Message")]
        public string Text { get; set; }

        [Display(Name = "Image")]
        public IFormFile Image { get; set; }
    }
}
