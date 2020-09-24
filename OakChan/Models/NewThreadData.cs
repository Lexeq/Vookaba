using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan
{
    public class NewThreadData
    {
        [Required, StringLength(maximumLength: 8, MinimumLength = 1)]
        public string Board { get; set; }

        [MaxLength(64)]
        public string Subject { get; set; }

        [Required, MaxLength(4096)]
        public string Text { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        [Required]
        public IFormFile AttachedImage { get; set; }
    }
}
