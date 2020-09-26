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
    public class MockService : IBoardService, IUserService, IThreadService
    {
        private Random rnd = new Random();
        public List<Thread> threads = new List<Thread>();
        public Task<Thread> CreateThreadAsync(string boardId, PostCreationData data)
        {

            var id = rnd.Next();
            var ms = new MemoryStream();
            data.Image.Source.CopyTo(ms);
            var ext = Path.GetExtension(data.Image.Name);
            File.WriteAllBytes($"wwwroot/res/img/{id}.{ext}", ms.ToArray());
            Image im = new Image { Hash = new byte[] { 10, 20, 30 }, Id = id, OriginalName = data.Image.Name, Type = ext, UploadDate = DateTime.Now };

            var tid = rnd.Next();
            var t = new Thread()
            {
                BoardId = boardId,
                Id = tid,
                Posts = new List<Post>
                {
                    new Post{
                        Id = rnd.Next(),
                        CreationTime = DateTime.Now,
                        Message = data.Text,
                        Name = data.Name,
                        Subject = data.Subject,
                        ThreadId = tid,
                        UserId =  new Guid(),
                        Image = im}
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
                            BoardId = b.Key,
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

        public Task<Anonymous> CreateAnonymousAsync(string ip)
        {
            return Task.FromResult(new Anonymous() { Id = new Guid(), Created = DateTime.Now, IP = ip});
        }

        public Task<Thread> GetThreadAsync(string board, int thread)
        {
            return Task.FromResult(threads.FirstOrDefault(t => t.Id == thread));
        }

        public async Task<Post> CreatePostAsync(string board, int thread, PostCreationData post)
        {
            var t = await GetThreadAsync(board, thread);
            Image im = null;

            if (post.Image != null)
            {
                var id = rnd.Next();
                var ms = new MemoryStream();
                post.Image.Source.CopyTo(ms);
                var ext = Path.GetExtension(post.Image.Name);
                File.WriteAllBytes($"wwwroot/res/img/{id}.{ext}", ms.ToArray());
                im = new Image { Hash = new byte[] { 10, 20, 30 }, Id = id, OriginalName = post.Image.Name, Type = ext, UploadDate = DateTime.Now };
            }
            var p = new Post
            {
                Id = rnd.Next(),
                CreationTime = DateTime.Now,
                Message = post.Text,
                Name = post.Name,
                Subject = post.Subject,
                ThreadId = t.Id,
                UserId = new Guid(),
                Image = im
            };

            t.Posts.Add(p);
            return t.Posts.Last();
        }
    }
}
