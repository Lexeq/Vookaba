using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vookaba.ViewModels
{
    public class ErrorViewModel
    {
        private const string ImagesFolder = "images";

        public ErrorViewModel()
        { }

        public ErrorViewModel(int code, string title, string description)
        {
            Code = code;
            Title = title;
            Description = description;
        }

        public int Code { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImagePath => $"/{ImagesFolder}/{Code}.png";
    }
}
