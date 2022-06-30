using System;

namespace Vookaba.DAL.Entities.Base
{
    interface ICreatedByAnonymous
    {
        public Guid AuthorToken { get; set; }
    }
}
