using OakChan.DAL.Entities;
using System;
using System.IO;

namespace OakChan.ViewModels
{
    public class PostViewModel
    {
        public int Number { get; set; }

        public DateTime Date { get; set; }

        public string Board { get; set; }

        public string Thread { get; set; }

        public Guid AuthorId { get; set; }

        public string Subject { get; set; }

        public string AuthorName { get; set; }

        public string Message { get; set; }

        public bool HasImage => Image != null;

        public ImageViewModel Image { get; set; }

        public static PostViewModel CreatePostViewModel(Post post, string board) =>
            new PostViewModel
            {
                AuthorId = post.UserId,
                AuthorName = post.Name,
                Board = board,
                Date = post.CreationTime,
                Message = post.Message,
                Number = post.Id,
                Subject = post.Subject,
                Thread = post.ThreadId.ToString(),
                Image = post.Image == null ? null :
                new ImageViewModel
                {
                    Id = post.Image.Id,
                    Height = post.Image.Height,
                    Width = post.Image.Width,
                    Name = post.Image.Name,
                    Path = "/res/img/" + post.Image.Name,
                    ThumbPath = "/res/img/" + Path.GetFileNameWithoutExtension(post.Image.Name) + "-min" + Path.GetExtension(post.Image.Name),
                    OriginalName = post.Image.OriginalName,
                    Size = post.Image.Size
                }
            };

    }

}
