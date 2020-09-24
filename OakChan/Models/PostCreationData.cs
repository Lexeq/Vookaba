using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models
{
    public class PostCreationData
    {
        public string Subject { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public ImageData Image { get; set; }
    }
}
