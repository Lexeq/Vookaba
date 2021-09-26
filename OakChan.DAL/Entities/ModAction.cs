﻿using OakChan.Common;
using OakChan.DAL.Entities.Base;
using OakChan.Identity;
using System;
using System.Net;

namespace OakChan.DAL.Entities
{
    public class ModAction : IHasCreationTime, ICreatedByUser, IHasClientInfo
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ApplicationEvent EventId { get; set; }

        public string EntityId { get; set; }

        public DateTime Created { get; set; }

        public IPAddress IP { get; set; }

        public string UserAgent { get; set; }

        public string Note { get; set; }

    }
}
