using System;

namespace OakChan.DAL.Entities.Base
{
    interface IHasCreationTime
    {
        public DateTime Created { get; set; }
    }
}
