namespace OakChan.Services.DTO
{
    public class BoardDto
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public bool IsHidden { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsReadOnly { get; set; }

        public int BumpLimit { get; set; }
    }
}
