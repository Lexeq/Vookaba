using System;

namespace OakChan.DAL.Entities.Base
{
    interface ICreatedByAnonymous
    {
        public Guid AuthorToken { get; set; }
    }
}
