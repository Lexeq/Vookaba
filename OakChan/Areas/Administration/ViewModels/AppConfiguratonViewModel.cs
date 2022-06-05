﻿using OakChan.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OakChan.Areas.Administration.ViewModels
{
    public class AppConfiguratonViewModel
    {
        [Display(Name = "Registration required invitation")]
        public bool RequireInvitation { get; set; }

        [Display(Name = "Allow anonymous posting")]
        [Tooltip("If disabled only registered users can post.")]
        public bool AnonymousCanPost { get; set; }
    }
}