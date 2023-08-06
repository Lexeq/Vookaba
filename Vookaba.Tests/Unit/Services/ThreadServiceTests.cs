using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Vookaba.Common.Exceptions;
using Vookaba.DAL.Database;
using Vookaba.DAL.Entities;
using Vookaba.Services.DbServices;
using Vookaba.Services.DTO;
using Vookaba.Services.Mapping;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vookaba.Services.Abstractions;
using Vookaba.DAL.MediaStorage;

namespace Vookaba.Tests.Unit.Services
{
    public class ThreadServiceTests
    {
        public IMapper Mapper { get; private set; }

        [OneTimeSetUp]
        public void Setup()
        {
            var sw = Stopwatch.StartNew();
            Mapper = new MapperConfiguration(x => x.AddProfile<ServicesMapProfile>()).CreateMapper();

            Debug.WriteLine("Profiles" + sw.Elapsed);
            Console.WriteLine("Profiles" + sw.Elapsed);
        }

        [TestCase("subj", "", "subj")]
        [TestCase("subj", "123", "subj")]
        [TestCase("", "subj", "subj")]
        [TestCase("", "<span>subj</span>subj", "subjsubj")]
        [TestCase("", "<span>subj</span>", "subj")]
        [TestCase("", "<i>subj</i>", "subj")]
        [TestCase("", "123456789_123456789_123456789_123456789_12messagelongerthensubjectsizelimit", "123456789_123456789_123456789_123456789...")]
        [TestCase("", "123456789_123456789_123456789_123456789_12", "123456789_123456789_123456789_123456789_12")]
        public async Task SetSubject(string subject, string message, string expected)
        {
            string subj = null;
            var postSet = new Mock<DbSet<Post>>();
            postSet.Setup(x => x.Add(It.IsAny<Post>()))
                    .Callback<Post>(x => subj = x.Thread.Subject);

            var ct = new Mock<VookabaDbContext>();
            ct.SetupGet(x => x.Posts).Returns(postSet.Object);


            var service = new DbThreadService(
                ct.Object,
                Mock.Of<IAttachmentsStorage>(),
                Mock.Of<IHashService>(),
                Enumerable.Empty<IPostProcessor>(),
                Mapper);

            await service.CreateThreadAsync("b", new ThreadCreationDto
            {
                Subject = subject,
                OpPost = new PostCreationDto
                {
                    Message = message
                }
            });

            Assert.AreEqual(expected, subj);
        }
    }
}
