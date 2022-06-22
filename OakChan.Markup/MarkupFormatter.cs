using OakChan.Services;
using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Markup
{
    public class MarkupFormatter : IPostProcessor
    {
        private readonly IMarkupTagsFactory tagsFactory;

        public MarkupFormatter(IMarkupTagsFactory tagsFactory)
        {
            this.tagsFactory = tagsFactory;
        }

        public async Task ProcessAsync(PostCreationDto post)
        {
            if (post.Message != null)
            {
                var markupProc = new MarkupTextProcessor(tagsFactory.GetTags());
                post.EncodedMessage = await markupProc.ProcessAsync(post.Message);
            }
        }
    }
}
