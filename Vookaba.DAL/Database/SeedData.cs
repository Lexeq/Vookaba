﻿using Vookaba.DAL.Entities;
using System.Collections.Generic;

namespace Vookaba.DAL.Database
{
    public class SeedData
    {
        public string AdministratorUserName { get; set; } = "Administrator";

        public string AdministratorPassword { get; set; }

        public string AdministratorEmail { get; set; }

        public IEnumerable<Board> Boards { get; set; }
    }

}
