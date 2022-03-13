using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OakChan.Common.Exceptions;
using OakChan.DAL;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using OakChan.Services;
using OakChan.Services.DbServices;
using OakChan.Services.DTO;
using OakChan.Services.Mapping;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Tests.Unit.Services
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
        [TestCase("", "123456789_123456789_123456789_12messagelongerthensubjectsizelimit", "123456789_123456789_123456789...")]
        [TestCase("", "123456789_123456789_123456789_12", "123456789_123456789_123456789_12")]
        public async Task SetSubject(string subject, string message, string expected)
        {
            string subj = null;
            var postSet = new Mock<DbSet<Post>>();
            postSet.Setup(x => x.Add(It.IsAny<Post>()))
                    .Callback<Post>(x => subj = x.Thread.Subject);

            var ct = new Mock<OakDbContext>();
            ct.SetupGet(x => x.Posts).Returns(postSet.Object);


            var service = new DbThreadService(
                ct.Object,
                Mock.Of<IAttachmentsStorage>(),
                Mock.Of<IHashService>(),
                Enumerable.Empty<IPostProcessor>(),
                Mapper,
                Mock.Of<ThrowHelper>());

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
