namespace OakChan.Services.DTO
{
    public class BoardInfoDto
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public bool IsHidden { get; set; }

        public bool IsDisabled { get; set; }

        public int BumpLimit { get; set; }

        public int ThreadsCount { get; set; }
    }
}
