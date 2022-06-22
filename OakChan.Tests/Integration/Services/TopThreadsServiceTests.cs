using NUnit.Framework;
using OakChan.DAL.Entities;
using OakChan.Services.DbServices;
using OakChan.Services.DTO;
using OakChan.Tests.Integration.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Tests.Integration.Services
{
    public class TopThreadsServiceTests : ServiceTestsBase
    {
        #region ByLastBump
        [Test]
        public async Task FavoriteThreadsByLastBump()
        {
            var board = new Board { Key = "qq", Name = "qq" };

            var thread = new[]
            {
                new Thread{
                    BoardKey = "b",
                    Subject = "2",
                    Posts = new[] {
                        new DefaultOpPost(),
                        new DefaultPost { Created = DateTime.SpecifyKind( new DateTime(2020, 1, 10), DateTimeKind.Utc) } } },
                new Thread{
                    BoardKey = "b",
                    Subject = "1",
                    Posts = new[] {
                        new DefaultOpPost(),
                        new DefaultPost { Created = DateTime.SpecifyKind(new DateTime(2020, 1, 20 ), DateTimeKind.Utc) } } },
                new Thread{
                    Board = board,
                    Subject = "4",
                    Posts = new[] {
                        new DefaultOpPost(),
                        new DefaultPost { Created = DateTime.SpecifyKind(new DateTime(2020, 1, 5), DateTimeKind.Utc) } } }
            };

            SeedData.AddDefaults().AddThreads(thread);

            TopThreadsService service = CreateService();

            var x = (await service.GetLastRepliedThreadsAsync(3)).ToList();

            Assert.IsNotNull(x);
            Assert.AreEqual(2, x.Count);
            Assert.AreEqual("1", x[0].Subject);
            Assert.AreEqual("4", x[1].Subject);
        }

        [Test]
        public async Task DoNotShowThreadsWithoutReplies()
        {
            var board = new Board { Key = "qq", Name = "qq" };

            var thread = new[]
            {
                new Thread{
                    BoardKey = "b",
                    Subject = "2",
                    Posts = new[] { new DefaultOpPost { Created = DateTime.SpecifyKind(new DateTime(2020, 1, 10), DateTimeKind.Utc) } } },
                new Thread{
                    Board = board,
                    Subject = "4",
                    Posts = new[] { new DefaultOpPost { Created = DateTime.SpecifyKind(new DateTime(2020, 1, 5), DateTimeKind.Utc) } } }
            };

            SeedData.AddDefaults().AddThreads(thread);

            TopThreadsService service = CreateService();

            var x = (await service.GetLastRepliedThreadsAsync(3)).ToList();

            Assert.IsNotNull(x);
            Assert.AreEqual(0, x.Count);
        }

        [TestCase(2, "2")]
        [TestCase(3, "3")]
        public async Task FavoriteThreadsByLast_Bumplimit(int bumplimit, string subj)
        {

            var board = new Board { Key = "qq", Name = "qq", BumpLimit = bumplimit };

            var threads = new[]
            {
                new Thread{
                    Board = board,
                    Subject = "3",
                    Posts = new[] {
                        new DefaultOpPost {Created = DateTime.UtcNow },
                        new DefaultPost {Created = DateTime.UtcNow.AddDays(1), IsSaged = true },
                        new DefaultPost {Created = DateTime.UtcNow.AddDays(10) }
                    }
                },
                new Thread{
                    Board = board,
                    Subject = "2",
                    Posts = new[] {
                        new DefaultOpPost {Created = DateTime.UtcNow.AddDays(2) },
                        new DefaultPost {Created = DateTime.UtcNow.AddDays(5) }
                         }
                }
            };

            SeedData.AddTokens(SeedData.DefaultToken).AddThreads(threads);
            TopThreadsService service = CreateService();

            var result = (await service.GetLastRepliedThreadsAsync(2)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(subj, result[0].Subject);
        }

        #endregion

        #region ByCreated
        [Test]
        public async Task FavoriteThreadsByCreation()
        {
            var board1 = new Board { Key = "qq", Name = "qq" };
            var board2 = new Board { Key = "ww", Name = "ww" };

            var threads = new[]
            {
                new Thread{
                    Board = board1,
                    Subject = "2",
                    Created = DateTime.UtcNow.AddDays(2),
                    Posts = new[] { new DefaultOpPost { Created = DateTime.UtcNow.AddDays(2) } } },
                new Thread{
                    Board = board1,
                    Subject = "1",
                    Created = DateTime.UtcNow,
                    Posts = new[] { new DefaultOpPost { Created = DateTime.UtcNow, } } },

                new Thread{
                    Board = board2,
                    Subject = "3",
                    Created = DateTime.UtcNow.AddDays(3),
                    Posts = new[] { new DefaultOpPost { Created = DateTime.UtcNow.AddDays(3) } } }
            };

            SeedData.AddTokens(SeedData.DefaultToken).AddThreads(threads);

            TopThreadsService service = CreateService();

            var x = (await service.GetLastCreatedThreadsAsync(3)).ToList();

            Assert.IsNotNull(x);
            Assert.AreEqual(2, x.Count);
            Assert.AreEqual("3", x[0].Subject);
            Assert.AreEqual("2", x[1].Subject);
        }
        #endregion

        #region For All
        [TestCaseSource(typeof(DataSources), nameof(DataSources.Foo), new object[] { 3 })]
        public async Task EmptyThread(MethodCaller method)
        {
            var thread = new Thread { Board = SeedData.DefaultBoard };
            SeedData.AddThreads(thread);
            TopThreadsService service = CreateService();

            var result = (await method(service)).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestCaseSource(typeof(DataSources), nameof(DataSources.Foo), new object[] { 3 })]
        public async Task EmptyBoard(MethodCaller method)
        {
            SeedData.AddBoards(SeedData.DefaultBoard);
            TopThreadsService service = CreateService();

            var result = (await method(service)).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        #endregion

        #region Helpers
        private TopThreadsService CreateService() =>
            new TopThreadsService(GetDbContext(), ServiceDtoMapper);

        class DataSources
        {
            public static IEnumerable<MethodCaller> Foo(int limit)
            {
                yield return serv => serv.GetLastRepliedThreadsAsync(limit);
                yield return serv => serv.GetLastCreatedThreadsAsync(limit);
            }
        }

        public delegate Task<IEnumerable<ThreadPreviewDto>> MethodCaller(TopThreadsService service);
        #endregion
    }
}
