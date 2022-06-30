using System.ComponentModel.DataAnnotations;

namespace Vookaba.Areas.Administration.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Field is required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(Common.ApplicationConstants.Identity.MinPasswordLength, ErrorMessage ="Password to short")]
        [RegularExpression(".*\\d.*", ErrorMessage = "Required digits")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwors are not same")]
        [Display(Name = "Confirmation")]
        public string PasswordConfirmation { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Login")]
        public string Login { get; set; }
    }

}
