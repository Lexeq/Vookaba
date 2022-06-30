using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Vookaba.Common.Exceptions;
using Vookaba.DAL;
using Vookaba.DAL.Entities;
using Vookaba.DAL.Entities.Base;
using Vookaba.Identity;
using Vookaba.Services;
using Vookaba.Services.DbServices;
using Vookaba.Tests.Integration.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Vookaba.Tests.Integration.Services
{
    public class PostServiceTests : ServiceTestsBase
    {
        private Mock<ThrowHelper> _throwHelperMock;
        private Mock<IAttachmentsStorage> _storage;
        private Mock<ILogger<DbPostService>> _logger;

        [OneTimeSetUp]
        public void Setup()
        {
            ConfigureMocks();
        }

        private void ConfigureMocks()
        {
            _throwHelperMock = new Mock<ThrowHelper>();
        }

        private DbPostService CreatePostService()
        {
            _storage = new Mock<IAttachmentsStorage>();
            _logger = new Mock<ILogger<DbPostService>>();

            return new DbPostService(GetDbContext(),
                                      _storage.Object,
                                      ServiceDtoMapper,
                                      _throwHelperMock.Object,
                                      _logger.Object);
        }

        [Test]
        public async Task GetPostByNumber()
        {
            SeedData.AddDefaults().AddThreads(
                new Thread
                {
                    Board = SeedData.DefaultBoard,
                    Posts = new Post[]
                    {
                        new DefaultOpPost(),
                        new DefaultPost(),
                        new DefaultPost{ HtmlEncodedMessage = "ok" }, //Number 3
                        new DefaultPost()
                    }
                });
            var posts = CreatePostService();

            var p = await posts.GetByNumberAsync(SeedData.DefaultBoard.Key, 3);
            Assert.AreEqual("ok", p.Message);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task DeletePostById(bool hasImage)
        {
            SeedData.AddDefaults().AddThreads(
                new Thread
                {
                    Board = SeedData.DefaultBoard,
                    Posts = new Post[]
                    {
                                    new DefaultOpPost(),
                                    new DefaultPost(),
                                    new DefaultPost{ Id = 42 },
                                    new DefaultPost()
                    }
                });

            if (hasImage)
            {
                SeedData.AddImages(new Image
                {
                    Extension = "a",
                    Hash = "0",
                    Name = "img",
                    OriginalName = "bbb",
                    PostId = 42,
                });
            }


            var posts = CreatePostService();
            await posts.DeleteByIdAsync(42);

            var post = GetDbContext().Posts.Find(42);

            Assert.AreEqual(3, GetDbContext().Posts.Count());
            Assert.AreEqual(1, GetDbContext().Threads.Count());
            Assert.IsNull(post);
            _storage.Verify(x => x.DeleteImageAsync(It.IsAny<string>()), hasImage ? Times.Once : Times.Never);
            _storage.Verify(x => x.DeleteImageAsync("img"), hasImage ? Times.Once : Times.Never);
        }

        [Test]
        public async Task DeleteOpPostById()
        {
            SeedData.AddDefaults().AddThreads(
                new Thread
                {
                    Board = SeedData.DefaultBoard,
                    Posts = new Post[]
                    {
                        new DefaultOpPost
                        {
                            Id = 42,
                            Attachments = new List<Attachment>
                            {
                                new Image
                                {
                                    Extension = "a",
                                    Hash = "0",
                                    Name = "img",
                                    OriginalName = "bbb",
                                }
                            }
                        },
                        new DefaultPost
                        {
                            Attachments = new List<Attachment>
                            {
                                new Image
                                {
                                    Extension = "c",
                                    Hash = "0",
                                    Name = "img2",
                                    OriginalName = "dd",
                                }
                            }
                        },
                        new DefaultPost()
                    }
                });

            var posts = CreatePostService();
            await posts.DeleteByIdAsync(42);

            var count = GetDbContext().Posts.Count();
            var post = GetDbContext().Posts.Find(42);

            Assert.AreEqual(0, GetDbContext().Posts.Count());
            Assert.AreEqual(0, GetDbContext().Threads.Count());
            Assert.IsNull(post);
            _storage.Verify(x => x.DeleteImageAsync(It.IsAny<string>()), Times.Exactly(2));
            _storage.Verify(x => x.DeleteImageAsync("img"), Times.Once);
            _storage.Verify(x => x.DeleteImageAsync("img2"), Times.Once);
        }

        [Test]
        public async Task DecreacePostsCountWhenPostDeleted()
        {
            var threadData = new Thread
            {
                Subject = "test_thread",
                Board = SeedData.DefaultBoard,
                Posts = new List<Post>
                {
                    new DefaultOpPost() { Attachments = new List<Attachment> { new DefaultImage() } },
                    new DefaultPost() { Id = 33 },
                    new DefaultPost()
                }
            };
            SeedData.AddDefaults().AddThreads(threadData);
            var service = CreatePostService();
            await service.DeleteByIdAsync(33);
            var context = GetDbContext();
            var thread = await context.Threads.FindAsync(threadData.Id);

            Assert.NotNull(thread);
            Assert.AreEqual(2, thread.PostsCount);
            Assert.AreEqual(1, thread.PostsWithAttachmentnsCount);
        }


        [TestCaseSource(typeof(DataSource), nameof(DataSource.MassDelete))]
        public async Task DeleteByIp(Mode mode, SearchArea area, int postsLeft, int imagesDeleted)
        {
            var violatorIP = IPAddress.Parse("55.55.55.55");
            var violatorToken = Guid.Parse("55555555-5555-5555-5555-555555555555");

            SeedData.AddDefaults();
            SeedData.AddTokens(new AuthorToken { Token = violatorToken });

            void FillWithThreads(Board board)
            {
                board.Threads = new List<Thread> {
                    new Thread {
                        Posts = new Post[] {
                            new DefaultOpPost(),
                            new DefaultPost {
                                IP = violatorIP,
                                AuthorToken = violatorToken,
                                Attachments = new List<Attachment> {
                                    new Image {
                                        Extension = "c",
                                        Hash = "0",
                                        Name = "img",
                                        OriginalName = "dd",
                                    }
                                }
                            },
                            new DefaultPost{ AuthorToken = violatorToken },
                            new DefaultPost{ IP = violatorIP }
                        }
                    },
                    //thread creaed by violator
                    new Thread {
                        Board = SeedData.DefaultBoard,
                        Posts = new Post[] {
                            new DefaultOpPost { IP = violatorIP, AuthorToken = violatorToken },
                            new DefaultPost() {
                                Attachments = new List<Attachment> {
                                    new Image {
                                        Extension = "c",
                                        Hash = "0",
                                        Name = "img",
                                        OriginalName = "dd",
                                    }
                                }
                            }
                        }
                    }
                };
            }

            FillWithThreads(SeedData.DefaultBoard);
            SeedData.DefaultBoard.Threads.First().Posts.Skip(1).First().Id = 456;
            var secondBoard = new Board
            {
                Key = "w",
                Name = "w"
            };
            FillWithThreads(secondBoard);
            SeedData.AddBoards(secondBoard);

            var service = CreatePostService();
            await service.DeleteManyAsync(456, mode, area);
            var context = GetDbContext();

            Assert.AreEqual(postsLeft, context.Posts.Count());
            _storage.Verify(s => s.DeleteImageAsync(It.IsAny<string>()), Times.Exactly(imagesDeleted));
        }

        public class DataSource
        {
            public static IEnumerable<object[]> MassDelete()
            {
                yield return new object[] { Mode.IP, SearchArea.All, 4, 4 };
                yield return new object[] { Mode.IP, SearchArea.Board, 8, 2 };
                yield return new object[] { Mode.IP, SearchArea.Thread, 10, 1 };
                yield return new object[] { Mode.Token, SearchArea.All, 4, 4 };
                yield return new object[] { Mode.Token, SearchArea.Board, 8, 2 };
                yield return new object[] { Mode.Token, SearchArea.Thread, 10, 1 };
            }
        }
    }
}
