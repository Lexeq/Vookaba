using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OakChan.Common.Exceptions;
using OakChan.DAL;
using OakChan.DAL.Entities;
using OakChan.Services;
using OakChan.Services.DbServices;
using OakChan.Services.DTO;
using OakChan.Tests.Base;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Tests.Services
{
    public class BoardsServiceTests : ServiceTestsBase
    {
        private Mock<ThrowHelper> _throwHelperMock;
        private Mock<IHashService> _hashMock;
        private Mock<IAttachmentsStorage> _storage;
        private Mock<ILogger<DbBoardService>> _logger;

        [OneTimeSetUp]
        public void Setup()
        {
            ConfigureMocks();
        }

        private void ConfigureMocks()
        {
            _throwHelperMock = new Mock<ThrowHelper>();

            _hashMock = new Mock<IHashService>();
            _hashMock.Setup(h => h.ComputeHash((byte[])null)).Returns(new byte[] { 100 });
            _hashMock.Setup(h => h.ComputeHash((Stream)null)).Returns(new byte[] { 100 });
        }

        private DbBoardService CreateBoardService()
        {
            _storage = new Mock<IAttachmentsStorage>();
            _logger = new Mock<ILogger<DbBoardService>>();

            return new DbBoardService(GetDbContext(),
                                      ServiceDtoMapper,
                                      _storage.Object,
                                      _throwHelperMock.Object,
                                      _logger.Object);
        }


        [TestCase(false, new[] { "b" })]
        [TestCase(true, new[] { "b", "h", "d", "hd" })]
        public async Task GetBoards(bool showAll, string[] keys)
        {
            SeedData.FromResource("boards");
            var service = CreateBoardService();

            var boards = await service.GetBoardsAsync(showAll);

            Assert.AreEqual(keys.Length, boards.Count());
            CollectionAssert.AreEquivalent(keys, boards.Select(x => x.Key));
        }


        [TestCase("b")]
        [TestCase("h")]
        [TestCase("d")]
        [TestCase("D")]
        public async Task GetBoard(string key)
        {
            SeedData.FromResource("boards");
            var service = CreateBoardService();

            var board = await service.GetBoardInfoAsync(key);

            Assert.NotNull(board);
            StringAssert.AreEqualIgnoringCase(key, board.Key);
        }

        [Test]
        public void GetNotExistingBoard()
        {
            SeedData.FromResource("boards");
            var service = CreateBoardService();
            BoardInfoDto result = null;

            Assert.DoesNotThrow(() => result = service.GetBoardInfoAsync("x").Result);
            Assert.IsNull(result);
        }

        [Test]
        public async Task ThreadsOrderOnBoardPage()
        {
            var threadsData = new Thread[]
            {
                new Thread{Id = 1, BoardKey = "b", Subject ="first_created"},
                new Thread{Id = 2, BoardKey = "b", Subject ="pinned", IsPinned = true},
                new Thread{Id = 3, BoardKey = "b", Subject ="last_created"},
            };

            var postsData = new[]
            {
                new DefaultPost{ThreadId = 1, Created = DateTime.Parse("2021-01-01"), Message = "OP1" },
                new DefaultPost{ThreadId = 2, Created = DateTime.Parse("2021-01-02"), Message = "OP_Pinned" },
                new DefaultPost{ThreadId = 3, Created = DateTime.Parse("2021-01-03"), Message = "OP2" },
                new DefaultPost{ThreadId = 3, Created = DateTime.Parse("2021-01-04"), Message = "Rep2" },
                new DefaultPost{ThreadId = 1, Created = DateTime.Parse("2021-01-05"), Message = "Rep1" },
                new DefaultPost{ThreadId = 3, Created = DateTime.Parse("2021-01-06"), Message = "Rep2", IsSaged = true },
            };

            SeedData.AddDefaults().AddThreads(threadsData).AddPosts(postsData);
            var service = CreateBoardService();

            var threads = (await service.GetThreadPreviewsAsync("b", 0, 10, 1)).ToList();

            CollectionAssert.AreEqual(new[] { 2, 1, 3 }, threads.Select(p => p.ThreadId));
            Assert.AreEqual(3, threads.Count);
            Assert.IsTrue(threads[0].IsPinned);
        }

        [Test]
        public async Task FirstPageOnBoardWithoutThreads()
        {
            SeedData.AddDefaults();
            var service = CreateBoardService();

            var result = (await service.GetThreadPreviewsAsync("b", 0, 10, 1)).ToList();

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public async Task BoardPageOffsetOutOfRange()
        {
            var threads = new Thread[]
            {
                new Thread{ Board = SeedData.DefaultBoard, Posts = new[] {new DefaultPost() } },
                new Thread{ Board = SeedData.DefaultBoard, Posts = new[] {new DefaultPost() } }
            };
            SeedData.AddDefaults().AddThreads(threads);
            var service = CreateBoardService();

            var result = (await service.GetThreadPreviewsAsync("b", 10, 10, 1)).ToList();

            CollectionAssert.IsEmpty(result);
        }

        //TODO: Replace Json with TestCaseSource
        [TestCase("postsorder1", 1, new int[] { })]
        [TestCase("postsorder1", 1, new int[] { })]
        [TestCase("postsorder2", 2, new int[] { 42 })]
        [TestCase("postsorder3", 4, new int[] { 77, 100 })]
        [TestCase("postsorder4", 5, new int[] { 600, 100 })]
        public async Task PostsOrder(string src, int postsCount, int[] ids)
        {
            SeedData.AddDefaults().FromResource(src);
            var service = CreateBoardService();

            var threadPreview = (await service.GetThreadPreviewsAsync("b", 0, 10, 2)).First();

            Assert.AreEqual(postsCount, threadPreview.TotalPostsCount);
            Assert.NotNull(threadPreview.Posts.First());
            CollectionAssert.AreEqual(ids, threadPreview.Posts.Skip(1).Select(x => x.PostId));
        }

        [Test]
        public async Task CreateBoardSuccessfully()
        {
            const string key = "z";
            const string name = "zzz";
            const int bumps = 25;

            var service = CreateBoardService();

            await service.CreateBoardAsync(new BoardDto { Key = key, Name = name, BumpLimit = bumps });
            var bi = await service.GetBoardInfoAsync(key);

            Assert.NotNull(bi);
            Assert.AreEqual(0, bi.ThreadsCount);
            Assert.AreEqual(key, bi.Key);
            Assert.AreEqual(name, bi.Name);
            Assert.AreEqual(bumps, bi.BumpLimit);
        }

        [Test]
        public void CreateBoardWithDuplicateKey()
        {
            const string key = "z";
            const string name = "zzz";

            SeedData.AddBoards(new Board { Key = key, Name = name });
            var service = CreateBoardService();

            var task = service.CreateBoardAsync(new BoardDto { Key = key, Name = name });

            Assert.That(async () => await task, Throws.Exception);
        }

        [Test]
        public async Task DeleteBoard()
        {
            const string key = "z";
            const string name = "zzz";

            SeedData
                .AddTokens(SeedData.DefaultToken)
                .AddBoards(new Board { Key = key, Name = name })
                .AddThreads(new Thread { BoardKey = key, Id = 10 })
                .AddPosts(new DefaultOpPost() { ThreadId = 10, Id = 20 })
                .AddImages(new DefaultImage { Name = "img1.png", PostId = 20 },
                           new DefaultImage { Name = "img2.jpg", PostId = 20 });

            var service = CreateBoardService();
            var context = GetDbContext();

            await service.DeleteBoardAsync(key);
            var board = context.Boards.Find(key);
            var threadsCount = context.Threads.Count();
            var postsCount = context.Posts.Count();
            var attachmentsCount = context.Images.Count();


            Assert.IsNull(board);
            Assert.AreEqual(0, threadsCount);
            Assert.AreEqual(0, postsCount);
            Assert.AreEqual(0, attachmentsCount);

            _storage.Verify(s => s.DeleteImageAsync("img1.png"), Times.Once);
            _storage.Verify(s => s.DeleteImageAsync("img2.jpg"), Times.Once);

        }

        [Test]
        public void DeleteNotExistingBoard()
        {
            const string key = "a";
            const string name = "aaa";

            SeedData.AddBoards(new Board { Key = key, Name = name });

            var service = CreateBoardService();
            var context = GetDbContext();

            Assert.That(async () => await service.DeleteBoardAsync(key + "a"), Throws.Exception);
        }

        [Test]
        public async Task EditBoardProperties()
        {

            SeedData.AddBoards(new Board
            {
                Key = "b",
                Name = "name",
                BumpLimit = 25,
                IsDisabled = false,
                IsHidden = true
            });

            var service = CreateBoardService();
            await service.UpdateBoardAsync("b", new BoardDto
            {
                Key = "b",
                Name = "new",
                BumpLimit = 100,
                IsDisabled = true
            });
            var board = await service.GetBoardInfoAsync("b");

            Assert.AreEqual("b", board.Key);
            Assert.AreEqual("new", board.Name);
            Assert.AreEqual(100, board.BumpLimit);
            Assert.AreEqual(true, board.IsDisabled);
            Assert.AreEqual(false, board.IsHidden);
        }

        [Test]
        public async Task EditBoardKey()
        {
            const string oldKey = "b";
            const string newKey = "x";
            const string name = "rnd";

            SeedData
                .AddTokens(SeedData.DefaultToken)
                .AddBoards(new Board { Key = oldKey, Name = name })
                .AddThreads(new Thread { BoardKey = oldKey, Id = 10 })
                .AddPosts(new DefaultOpPost() { ThreadId = 10, Id = 20 })
                .AddImages(new DefaultImage { Name = "img.bmp", PostId = 20 });

            var service = CreateBoardService();
            var context = GetDbContext();

            await service.UpdateBoardAsync("b", new BoardDto
            {
                Key = newKey,
                Name = name
            });
            var oldBoard = context.Boards.Find(oldKey);
            var newBoard = context.Boards.Find(newKey);
            var threadsCount = context.Threads.Count(t => t.BoardKey == newKey);
            var postsCount = context.Posts.Count(p => p.Thread.BoardKey == newKey);
            var attachmentsCount = context.Images.Count(i => i.Post.Thread.BoardKey == newKey);


            Assert.IsNull(oldBoard);
            Assert.IsNotNull(newBoard);
            Assert.AreEqual(1, threadsCount);
            Assert.AreEqual(1, postsCount);
            Assert.AreEqual(1, attachmentsCount);
        }

        [Test]
        public void UpdateNotExistingBoard()
        {
            const string key = "a";
            const string name = "aaa";

            SeedData.AddBoards(new Board { Key = key, Name = name });

            var service = CreateBoardService();
            var context = GetDbContext();

            Assert.That(async () => await service.UpdateBoardAsync(key + "a", new BoardDto { Name = "new_name" }), Throws.Exception);
            Assert.That(async () => await service.UpdateBoardAsync(key + "a", new BoardDto { Name = "new_name", Key = "pp" }), Throws.Exception);
        }
    }
}
