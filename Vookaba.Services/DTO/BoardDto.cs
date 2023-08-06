namespace Vookaba.Services.DTO
{
    public class BoardDto
    {
        public string Key { get; set; } = null!;

        public string Name { get; set; } =  null!;

        public bool IsHidden { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsReadOnly { get; set; }

        public int BumpLimit { get; set; }
    }
}
