using System.Net;

namespace Vookaba.DAL.Entities.Base
{
    interface IHasClientInfo
    {
        public IPAddress IP { get; set; }

        public string UserAgent { get; set; }
    }
}
