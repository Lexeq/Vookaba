using System.Collections.Generic;

namespace OakChan.Services.DTO
{
    public class StaffDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Role { get; init; }

        public List<string> Boards { get; set; }
    }
}
