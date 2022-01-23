using OakChan.Services;
using System.Threading.Tasks;

namespace OakChan.Markup
{
    public class MarkupFormatter : IHtmlFormatter
    {
        private readonly IMarkupTagsFactory tagsFactory;

        public MarkupFormatter(IMarkupTagsFactory tagsFactory)
        {
            this.tagsFactory = tagsFactory;
        }

        public Task<string> FormatAsync(string text)
        {
            var markupProc = new MarkupTextProcessor(tagsFactory.GetTags());
            return markupProc.ProcessAsync(text);
        }
    }
}
