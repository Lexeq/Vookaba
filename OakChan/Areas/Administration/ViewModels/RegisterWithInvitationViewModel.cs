using System.ComponentModel.DataAnnotations;

namespace OakChan.Areas.Administration.ViewModels
{
    public class RegisterWithInvitationViewModel : RegisterViewModel
    {
        [Required]
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
