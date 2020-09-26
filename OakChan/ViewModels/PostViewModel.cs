using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.ViewModels
{
    public class PostViewModel 
    {
        [Required]
        public string Board { get; set; }

        [Required]
        public int? Thread { get; set; }

        public string Subject { get; set; }

        public string Name { get; set; }

        [Required]
        public string Text { get; set; }

        public IFormFile Image { get; set; }
    }
}
