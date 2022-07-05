using System;
using System.Threading.Tasks;

namespace Vookaba.Markup
{
    public interface IMarkupTag
    {
        bool InnerTagsAllowed { get; }

        bool NewLineRequired { get; }

        ValueTask<TagResult> TryOpenAsync(ReadOnlyMemory<char> msg);
    }
}
