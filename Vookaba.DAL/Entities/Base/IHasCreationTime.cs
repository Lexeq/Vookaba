using System;

namespace Vookaba.DAL.Entities.Base
{
    interface IHasCreationTime
    {
        public DateTime Created { get; set; }
    }
}
