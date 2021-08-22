using OakChan.DAL.Entities;
using System;

namespace OakChan.Tests.Base
{
    class DefaultImage : Image
    {
        public DefaultImage()
        {
            Width = 100;
            Height = 100;
            Extension = "jpg";
            Name = Guid.NewGuid().ToString();
            Hash = Guid.NewGuid().ToString();
            OriginalName = "pic.jpg";
            FileSize = 1024 * 12;
        }
    }

}
