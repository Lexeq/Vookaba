using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.ViewModels
{
    public class ErrorViewModel
    {
        private const string ImagesFolder = "images";

        public int Code { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImagePath => $"/{ImagesFolder}/{Code}.png";
    }
}
