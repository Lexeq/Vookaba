using OakChan.Models.DB;
using OakChan.Models.DB.Entities;
using OakChan.Models.Interfces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models
{
    public class MockService : IBoardService, IUserService
    {
        private Random rnd = new Random();
        public List<Thread> threads = new List<Thread>();
        public Task<Thread> CreateThreadAsync(string boardId, PostCreationData data)
        {
            var tid = rnd.Next();
            var t = new Thread()
            {
                BoardId = boardId,
                Id = tid,
                Posts = new List<Post>
                {
                    new Post{
                        Id =rnd.Next(),
                        CreationTime = DateTime.Now,
                        Message = data.Text,
                        Name = data.Name,
                        Subject = data.Subject,
                        ThreadId = tid,
                        UserId = 42 }
                }
            };


            threads.Add(t);
            return Task.FromResult(t);

        }

        public Task<Board> GetBoardPreviewAsync(string boardId, int page, int threadsPerPage)
        {
            var b = GetBoardsAsync().Result.FirstOrDefault(b => b.Key == boardId);

            if (b == null)
            {
                return Task.FromResult<Board>(null);
            }

            return Task.FromResult(
                new Board
                {
                    Key = b.Key,
                    Name = b.Name,
                    Threads = new[] {
                        new Thread { 
                            Id = 8,
                            Posts = new[] {
                                new Post { 
                                    Message = "Oppost",
                                    Image = new Image { Id = 0, Type = "jpg" } ,
                                    Subject = "Test thread", Id = 32,
                                    Name="OP", 
                                    CreationTime = DateTime.Parse("12.02.2019 13:14"),
                                    ThreadId = 8 },
                                new Post { 
                                    Message = "reply post",
                                    Name ="Anon",
                                    CreationTime = DateTime.Parse("12.02.2019 14:00"),
                                    Id = 40,
                                    ThreadId = 8 }
                        }},
                        new Thread { Posts = new[] { new Post { Message = "222" } } },
                        new Thread { Posts = new[] { new Post { Message = "333" } } }}
                    .Union(threads.Where(t => t.BoardId == boardId))
                    .Take(threadsPerPage)
                    .OrderByDescending(t => t.Posts.OrderBy(p => p.CreationTime).Last().CreationTime)
                    .ToArray()
                });
        }

        public Task<IEnumerable<Board>> GetBoardsAsync()
            => Task.FromResult<IEnumerable<Board>>(new[] {
                new Board() {Key = "b", Name = "Random"},
                new Board() {Key = "pic", Name = "Pictures" },
                new Board() {Key = "sky", Name = "Buy Skyrim" }});

        public User CreateAnonymous()
        {
            return new User() { Id = rnd.Next() };
        }
    }
}
