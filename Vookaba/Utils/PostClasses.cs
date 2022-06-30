using Vookaba.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vookaba.Utils
{
    public static class PostClasses
    {
        public static IEnumerable<string> GetClasses(PostViewModel post, Guid currentUserToken)
        {
            yield return "post";
            if (post.HasImage) yield return "post-withimage";
            if (post.IsOpening) yield return "oppost";
            if (post.AuthorId == currentUserToken) yield return "post-owned";
            if (post.IsSaged) yield return "saged";
        }
    }
}
