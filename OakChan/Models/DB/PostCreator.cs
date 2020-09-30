using OakChan.Models.DB.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OakChan.Models.DB
{
    public class PostCreator
    {
        private readonly MediaStorage media;

        public PostCreator(MediaStorage media)
        {
            this.media = media;
        }

        public async Task<Post> AddPostToThread(PostCreationData postData, Thread thread)
        {
            Image postImage = null;
            if (postData.Image != null)
            {
                using var sha = System.Security.Cryptography.SHA1.Create();
                var hash = sha.ComputeHash(postData.Image.Data);
                var extension = Path.GetExtension(postData.Image.Name).TrimStart('.');
                string imageNameForSaving = $"{DateTime.Now.ToUniversalTime().Ticks}.{extension}";
                postImage = new Image
                {
                    Name = imageNameForSaving,
                    Hash = hash,
                    OriginalName = postData.Image.Name,
                    Type = extension,
                    UploadDate = DateTime.Now,
                };

                var image = await media.AddImage(postData.Image.Data, imageNameForSaving);
                postImage.Width = image.Width;
                postImage.Height = image.Height;
                postImage.Size = postData.Image.Data.Length;
            }

            return new Post
            {
                CreationTime = DateTime.Now,
                Image = postImage,
                Message = postData.Text,
                Name = postData.Name,
                Subject = postData.Subject,
                Thread = thread,
                UserId = postData.Author
            };
        }
    }
}
