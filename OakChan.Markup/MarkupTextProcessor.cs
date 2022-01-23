using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakChan.Markup
{

    public class MarkupTextProcessor
    {
        class Counter
        {
            public int Count { get; set; }
        }

        private readonly IMarkupTag[] tags;

        public int MaxTags { get; set; } = 128;

        public MarkupTextProcessor(IEnumerable<IMarkupTag> tags)
        {
            this.tags = tags.ToArray();
        }

        public async Task<string> ProcessAsync(string message)
        {
            StringBuilder sb = new((int)(message.Length * 1.25));
            await ParseAsync(message.AsMemory().Trim(), sb, new Counter());
            return sb.ToString();
        }

        private async Task ParseAsync(ReadOnlyMemory<char> msg, StringBuilder builder, Counter counter)
        {
            bool newline = counter.Count == 0;

            for (int i = 0; i < msg.Length; i++)
            {
                if (counter.Count == MaxTags)
                {
                    HtmlEncoder.AppendEncoded(msg.Span[i..], builder);
                    break;
                }

                var openedTag = await TryStartAsync(msg[i..], newline);
                newline = msg.Span[i] == '\n';

                if (openedTag != null)
                {
                    counter.Count++;
                    builder.Append(openedTag.Value.Result.OpeningHtml);
                    if (openedTag.Value.Tag.InnerTagsAllowed)
                    {
                        await ParseAsync(openedTag.Value.Result.Content, builder, counter);
                    }
                    else
                    {
                        HtmlEncoder.AppendEncoded(openedTag.Value.Result.Content.Span, builder);
                    }
                    builder.Append(openedTag.Value.Result.ClosingHtml);
                    i += openedTag.Value.Result.Length - 1; // - 1 because 'i' will be increased by 1 on the next loop iteration
                }
                else
                {
                    HtmlEncoder.AppendEncoded(msg.Span.Slice(i, 1), builder);
                }
            }
        }

        private async ValueTask<(TagResult Result, IMarkupTag Tag)?> TryStartAsync(ReadOnlyMemory<char> input, bool newLine)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                var tag = tags[i];

                if (tag.NewLineRequired && !newLine) { continue; }

                var op = await tag.TryOpenAsync(input);
                if (op.Successed)
                {
                    return (op, tag);
                }
            }
            return null;
        }
    }
}
