using System;
using System.Threading.Tasks;

namespace Vookaba.Markup.Tags
{
    public class QuoteTag : IMarkupTag
    {
        private readonly string _openingHtml = "<span class=\"quote" +
            "\">&gt;";
        private readonly string _closingHtml = "</span>";

        public bool NewLineRequired => true;

        public bool InnerTagsAllowed => true;

        public ValueTask<TagResult> TryOpenAsync(ReadOnlyMemory<char> input)
        {
            var span = input.Span;

            if (span[0] != '>')
            {
                return ValueTask.FromResult(TagResult.Fail);
            }

            var lineEndsIdx = span.IndexOf("\n");

            TagResult x;
            if (lineEndsIdx < 0)
            {
                x = TagResult.Found(
                    span.Length,
                    input[1..],
                   _openingHtml,
                   _closingHtml);
            }
            else
            {
                x = TagResult.Found(
                    lineEndsIdx,
                    input.Slice(1, lineEndsIdx - 1),
                    _openingHtml,
                    _closingHtml);
            }
            return ValueTask.FromResult(x);
        }
    }
}
