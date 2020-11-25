using OakChan.DAL.Entities;
using OakChan.Models.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.Interfaces
{
    public interface IThreadService
    {
        public Task<Thread> GetThreadAsync(string board, int thread);

        public Task<Post> CreatePostAsync(string board, int thread, PostCreationData post);
    }
}
