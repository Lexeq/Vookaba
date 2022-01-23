using System.Collections.Generic;

namespace OakChan.Markup
{
    public interface IMarkupTagsFactory
    {
        IEnumerable<IMarkupTag> GetTags();
    }
}
