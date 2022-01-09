using OakChan.DAL.Entities;
using System;
using System.Net;

namespace OakChan.Tests.Base
{
    class DefaultPost : Post
    {
        public DefaultPost()
        {
            Message = "Post";
            Created = new DateTime(2021, 1, 1);
            AuthorToken = new Guid("11111111-1111-1111-1111-111111111111");
            IP = IPAddress.Loopback;
            UserAgent = "tester";
        }
    }

    class DefaultOpPost : DefaultPost
    {
        public DefaultOpPost()
        {
            IsOP = true;
        }
    }

}
