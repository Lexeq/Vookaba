using OakChan.Services;
using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Tests.Integration.Base
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
