namespace OakChan.Deanon
{
    public class DeanonOptions
    {
        public bool SignOutIfUserAuthentificated { get; set; } = false;

        public string IdTokenClaimType { get; set; } = "uid";

    }
}
