using OakChan.DAL.Entities;
using System;
using System.Net;

namespace OakChan.Tests.Integration.Base
{
    class DefaultPost : Post
    {
        public DefaultPost()
        {
            PlainMessageText = "Post";
            Created = DateTime.SpecifyKind(new DateTime(2021, 1, 1), DateTimeKind.Utc);
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
