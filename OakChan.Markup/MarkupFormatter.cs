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
            if (!string.IsNullOrEmpty(post.Message))
            {
                var markupProc = new MarkupTextProcessor(tagsFactory.GetTags());
                post.Message = await markupProc.ProcessAsync(post.Message);
            }
        }
    }
}
