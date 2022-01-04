using System.ComponentModel.DataAnnotations;

namespace OakChan.Areas.Administration.ViewModels
{
    public class RegisterWithInvitationViewModel : RegisterViewModel
    {
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Invitaion")]
        public string Invitaion { get; set; }

        public RegisterWithInvitationViewModel() { }
        public RegisterWithInvitationViewModel(RegisterViewModel vm, string invitation)
        {
            this.Login = vm.Login;
            this.Email = vm.Email;
            this.Password = vm.Password;
            this.PasswordConfirmation = vm.PasswordConfirmation;
            this.Invitaion = invitation;
        }
    }

}
