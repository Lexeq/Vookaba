using Vookaba.Services;
using Vookaba.Services.DTO;
using System.Threading.Tasks;

namespace Vookaba.Tests.Integration.Base
{
    internal class FakePostProcessor : IPostProcessor
    {
        public Task ProcessAsync(PostCreationDto post)
        {
            post.EncodedMessage = post.Message;
            return Task.CompletedTask;
        }
    }
}
