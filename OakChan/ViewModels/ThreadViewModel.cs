using Microsoft.AspNetCore.Http;
using OakChan.Models.DB.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.ViewModels
{
    public class ThreadViewModel
    {
        public Thread Thread { get; set; }

        public PostFormViewModel Post { get; set; }
    }
}
