using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.ViewModels
{
    public interface IPostViewModel
    {
        public string Board { get; }

        public int? Thread { get; }

        public string Subject { get; }

        public string Name { get; }

        public string Text { get; }

        public IFormFile Image { get;  }
    }
}
