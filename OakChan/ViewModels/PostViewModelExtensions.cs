using OakChan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.ViewModels
{
    public static class PostViewModelExtensions
    {
        public static PostCreationData ToPostCreationData(this IPostViewModel post, Guid anonId)
        {
            return new PostCreationData(anonId)
            {
                Name = post.Name,
                Subject = post.Subject,
                Text = post.Text,
                Image = post.Image == null ? null : new ImageData
                {
                    Name = post.Image?.FileName,
                    Source = post.Image?.OpenReadStream()
                }
            };
        }
    }
}
