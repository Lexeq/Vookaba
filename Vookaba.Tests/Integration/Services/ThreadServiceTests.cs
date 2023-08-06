using Moq;
using NUnit.Framework;
using Vookaba.Common.Exceptions;
using Vookaba.DAL;
using Vookaba.DAL.Entities;
using Vookaba.DAL.Entities.Base;
using Vookaba.Services.DbServices;
using Vookaba.Services.DTO;
using Vookaba.Tests.Integration.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vookaba.Services.Abstractions;
using Vookaba.DAL.MediaStorage;

namespace Vookaba.Tests.Integration.Services
{
    public class ThreadServiceTests : ServiceTestsBase
    {
        private Mock<IHashService> _hashMock;
        private Mock<IAttachmentsStorage> storageMock;

        [OneTimeSetUp]
        public void Setup()
        {
            ConfigureMocks();
        }

        private DbThreadService CreateThreadService()
        {
            return new DbThreadService(
                GetDbContext(),
                storageMock.Object,
                _hashMock.Object,
                new[] { new FakePostProcessor() },
                ServiceDtoMapper);
        }

        private void ConfigureMocks()
        {
            _hashMock = new Mock<IHashService>();
            _hashMock.Setup(h => h.ComputeHash((byte[])null)).Returns(new byte[] { 100 });
            _hashMock.Setup(h => h.ComputeHash((Stream)null)).Returns(new byte[] { 100 });
            storageMock = new Mock<IAttachmentsStorage>();
            storageMock.Setup(x => x.AddImageAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(new ImageSavingResult
                {
                    Image = new ImageInfo
                    {
                        Height = 100,
                        Name = "xx",
                        Width = 100
                    }
                });
        }


        [TestCase("b", 100, false)]
        [TestCase("b", 700, true)]
        [TestCase("a", 100, true)]
        public async Task GetThread(string boardKey, int threadId, bool isNull)
        {
            var board = new Board
            {
                Key = "b",
                Name = "b",
                Threads = new[]
                {
                    new Thread{ Id = 100, Subject = "t", Posts = new[] {new DefaultOpPost() } }
                }
            };
            SeedData.AddTokens(SeedData.DefaultToken).AddBoards(board);
            var threadService = CreateThreadService();

            var thread = await threadService.GetThreadAsync(boardKey, threadId);

            Assert.AreEqual(isNull, thread == null);
        }

        [Test]
        public async Task CreateThread()
        {
            var threadData = new ThreadCreationDto
            {
                Subject = "test_thread",
                OpPost = new PostCreationDto
                {
                    Message = "test_message",
                }
            };
            SeedData.AddDefaults();
            var service = CreateThreadService();


            var t = await service.CreateThreadAsync("b", threadData);
            var createdThread = await service.GetThreadAsync("b", t.ThreadId);

            Assert.NotNull(createdThread);
            Assert.AreEqual(threadData.Subject, createdThread.Subject);
            Assert.AreEqual(1, createdThread.Posts.Count());
            Assert.NotNull(createdThread.Posts.First());
            Assert.AreEqual(threadData.OpPost.Message, createdThread.Posts.First().Message);
        }

        [Test]
        public async Task CreatePost()
        {
            var postData = new PostCreationDto
            {
                Message = "test_message",
            };

            SeedData.AddDefaults()
                .AddThreads(new Thread
                {
                    BoardKey = "b",
                    Id = 1,
                    Posts = new[]
                    {
                        new DefaultOpPost()
                        {
                            PlainMessageText = "x",
                        }
                    }
                });

            var service = CreateThreadService();

            var postDto = await service.AddPostToThreadAsync(1, postData);
            var thread = await service.GetThreadAsync("b", 1);

            Assert.NotNull(postDto);
            Assert.NotNull(thread);
            Assert.AreEqual(1, postDto.ThreadId);
            Assert.AreEqual(postDto.Message, thread.Posts.Last().Message);
        }

        [Test]
        public async Task PostsNumbering()
        {
            var board1 = new Board
            {
                Key = "a",
                Name = "a"

            };

            var board2 = new Board
            {
                Key = "b",
                Name = "b"
            };

            SeedData.AddTokens(SeedData.DefaultToken).AddBoards(board1, board2);

            var service = CreateThreadService();


            var aThread = await service.CreateThreadAsync("a", new ThreadCreationDto
            {
                Subject = "a_thread",
                OpPost = new PostCreationDto
                {
                    Message = "a_1"
                }
            });

            var bThread = await service.CreateThreadAsync("b", new ThreadCreationDto
            {
                Subject = "b_thread",
                OpPost = new PostCreationDto
                {
                    Message = "b_1"
                }
            });

            var a2 = await service.AddPostToThreadAsync(aThread.ThreadId, new PostCreationDto
            {
                Message = "a_2"
            });


            Assert.AreEqual(1, aThread.Posts.First().PostNumber);
            Assert.AreEqual(1, bThread.Posts.First().PostNumber);
            Assert.AreEqual(2, a2.PostNumber);
        }

        [Test]
        public async Task BumpThread()
        {
            var board = new Board
            {
                BumpLimit = 3,
                Key = "a",
                Name = "a",
                Threads = new Thread[]
                {
                    new Thread
                    {
                        Id = 10,
                        Board = SeedData.DefaultBoard,
                        Posts = new Post[]
                        {
                              new DefaultOpPost { PlainMessageText = "OpPost", Created = DateTime.UtcNow.AddDays(-20) },
                              new DefaultPost { PlainMessageText = "Reply", Created = DateTime.UtcNow.AddDays(-10) }
                        }
                    }
                }
            };

            SeedData.AddTokens(SeedData.DefaultToken).AddBoards(board);


            var service = CreateThreadService();

            await service.AddPostToThreadAsync(10, new PostCreationDto());

            var thread = await service.GetThreadAsync("a", 10);
            Assert.AreEqual(DateTime.UtcNow.Date, thread.LastBump.Date);
        }

        [Test]
        public async Task DoNotBumpThreadInBumpLimit()
        {
            var board = new Board
            {
                BumpLimit = 4,
                Key = "a",
                Name = "a",
                Threads = new Thread[]
    {
                    new Thread
                    {
                        Id = 10,
                        Board = SeedData.DefaultBoard,
                        Posts = new Post[]
                        {
                              new DefaultOpPost { PlainMessageText = "OpPost", Created = DateTime.UtcNow.AddDays(-40) },
                              new DefaultPost { PlainMessageText = "Reply", Created = DateTime.UtcNow.AddDays(-30) },
                              new DefaultPost { PlainMessageText = "Reply", Created = DateTime.UtcNow.AddDays(-20) },
                              new DefaultPost { PlainMessageText = "Reply", Created = DateTime.UtcNow.AddDays(-10) },
                        }
                    }
    }
            };

            SeedData.AddTokens(SeedData.DefaultToken).AddBoards(board);


            var service = CreateThreadService();

            await service.AddPostToThreadAsync(10, new PostCreationDto
            {
            });

            var thread = await service.GetThreadAsync("a", 10);
            Assert.AreNotEqual(DateTime.UtcNow.Date, thread.LastBump.Date);
        }

        [Test]
        public async Task DoNotBumpThreadIfPostWithSage()
        {
            var board = new Board
            {
                BumpLimit = 10,
                Key = "a",
                Name = "a",
                Threads = new Thread[]
                {
                    new Thread
                    {
                        Id = 10,
                        Board = SeedData.DefaultBoard,
                        Posts = new Post[]
                        {
                              new DefaultOpPost { PlainMessageText = "OpPost", Created = DateTime.UtcNow.AddDays(-20) },
                              new DefaultPost { PlainMessageText = "Reply", Created = DateTime.UtcNow.AddDays(-10) }
                        }
                    }
                }
            };

            SeedData.AddTokens(SeedData.DefaultToken).AddBoards(board);


            var service = CreateThreadService();

            await service.AddPostToThreadAsync(10, new PostCreationDto
            {
                IsSaged = true
            });

            var thread = await service.GetThreadAsync("a", 10);
            Assert.AreNotEqual(DateTime.UtcNow.Date, thread.LastBump.Date);
        }

        [Test]
        public async Task CountsIsCorrect()
        {
            var threadData = new Thread
            {
                Subject = "test_thread",
                Board = SeedData.DefaultBoard,
                Posts = new List<Post>
                {
                    new DefaultOpPost() { Attachments = new List<Attachment> { new DefaultImage() } },
                    new DefaultPost(),
                    new DefaultPost(),
                    new DefaultPost { Attachments = new List<Attachment> { new DefaultImage()} },
                    new DefaultPost { Attachments = new List<Attachment> { new DefaultImage()} }
                }
            };
            SeedData.AddDefaults().AddThreads(threadData);
            var service = CreateThreadService();

            var context = GetDbContext();
            var thread = await context.Threads.FindAsync(threadData.Id);

            Assert.NotNull(thread);
            Assert.AreEqual(5, thread.PostsCount);
            Assert.AreEqual(3, thread.PostsWithAttachmentnsCount);
        }
    }
}
