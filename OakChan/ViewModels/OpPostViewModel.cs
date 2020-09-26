using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.ViewModels
{
    public class OpPostViewModel
    {
        [Required]
        public string Board { get; set; }

        public int? Thread { get; set; }

        [MaxLength(32)]
        public string Subject { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        [Required, MaxLength(4096)]
        public string Text { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
