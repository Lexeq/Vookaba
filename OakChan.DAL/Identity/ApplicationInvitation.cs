using OakChan.DAL.Entities.Base;

namespace OakChan.Identity
{
    public class ApplicationInvitation : Invitation<int>, IHasCreationTime
    {
        public ApplicationUser Publisher { get; set; }

        public int? UsedByID { get; set; }

        public ApplicationUser UsedBy { get; set; }

        public string Token => Id.ToString("n");
    }

}
