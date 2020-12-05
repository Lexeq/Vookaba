using System;
using System.Collections.Generic;
using System.Text;

namespace OakChan.Services.DTO
{
    public class ImageDto
    {
        public int ImageId { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Size{ get; set; }
        public string Name{ get; set; }
        public string OriginalName{ get; set; }
    }
}
