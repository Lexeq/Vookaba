using System.ComponentModel.DataAnnotations;

namespace OakChan.Areas.Administration.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool Remember { get; set; }
    }
}
