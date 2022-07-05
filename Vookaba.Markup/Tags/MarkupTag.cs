using System;
using System.Threading.Tasks;

namespace Vookaba.Markup.Tags
{
    public class MarkupTag : IMarkupTag
    {
        private readonly StringComparison _comparisonType;
        private readonly string _openingHtml;
        private readonly string _closingHtml;
        private readonly string _tag;

        public virtual bool InnerTagsAllowed => true;

        public virtual bool NewLineRequired => false;

        public MarkupTag(string tag, string htmlTag) :
            this(tag, htmlTag, null, true)
        { }

        public MarkupTag(string tag, string htmlTag, string? htmlClass = null, bool caseSensetive = true)
        {
            if (string.IsNullOrWhiteSpace(htmlTag)) { throw new ArgumentException("Value can not be empty or null.", nameof(htmlTag)); }
            _tag = !string.IsNullOrWhiteSpace(tag) ? tag : throw new ArgumentException("Value can not be empty or null.");
            _openingHtml = $"<{htmlTag}{(htmlClass != null ? $" class=\"{htmlClass}\"" : "")}>";
            _closingHtml = $"</{htmlTag}>";
            _comparisonType = caseSensetive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        public ValueTask<TagResult> TryOpenAsync(ReadOnlyMemory<char> input)
        {
            var span = input.Span;
            if (!span.StartsWith(_tag, _comparisonType))
            {
                return ValueTask.FromResult(TagResult.Fail);
            }

            var closingIndex = span[_tag.Length..].IndexOf(_tag, _comparisonType);

            if (closingIndex < 0)
            {
                return ValueTask.FromResult(TagResult.Fail);
            }

            return ValueTask.FromResult(TagResult.Found(
                length: _tag.Length + closingIndex + _tag.Length,
                content: input.Slice(_tag.Length, closingIndex),
                opening: _openingHtml,
                closing: _closingHtml));
        }
    }
}
