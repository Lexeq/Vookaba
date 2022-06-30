namespace Vookaba.Identity
{
    public class ApplicationInvitation : Invitation<int>
    {
        public ApplicationUser Publisher { get; set; }

        public int? UsedByID { get; set; }

        public ApplicationUser UsedBy { get; set; }

        public string Token => Id.ToString("n");
    }

}
