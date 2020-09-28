using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB.Entities
{
    public class Image
    {
        public int Id { get; set; }

        public string OriginalName { get; set; }

        public string Type { get; set; }

        public DateTime UploadDate { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Size { get; set; }

        public byte[] Hash { get; set; }

        public string Name { get; set; }
    }
}
