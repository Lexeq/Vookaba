using OakChan.DAL.Entities;
using System.Collections.Generic;

namespace OakChan.DAL.Database
{
    public class SeedData
    {
        public string AdminRoleName { get; set; } = "Administrator";

        public string AdministratorUserName { get; set; } = "Administrator";

        public string AdministratorPassword { get; set; }

        public string AdministratorEmail { get; set; }

        public IEnumerable<Board> Boards { get; set; }
    }

}
