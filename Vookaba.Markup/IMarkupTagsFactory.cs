using System.Collections.Generic;

namespace Vookaba.Markup
{
    public interface IMarkupTagsFactory
    {
        IEnumerable<IMarkupTag> GetTags();
    }
}
