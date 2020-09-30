using Newtonsoft.Json.Converters;
using OakChan.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.ViewModels
{
    public static class PostViewModelExtensions
    {
        public static async Task<PostCreationData> ToPostCreationData(this IPostViewModel post, Guid anonId)
        {
            byte[] imageBytes = null;
            if (post.Image != null)
            {
                using var ms = new MemoryStream();
                await post.Image.OpenReadStream().CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            return new PostCreationData(anonId)
            {
                Name = post.Name,
                Subject = post.Subject,
                Text = post.Text,
                Image = post.Image == null ? null : new UploadImageData
                {
                    Name = post.Image?.FileName,
                    Data = imageBytes
                }
            };
        }
    }
}
