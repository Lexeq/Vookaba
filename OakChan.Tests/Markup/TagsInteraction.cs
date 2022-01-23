using NUnit.Framework;
using OakChan.Markup;
using OakChan.Markup.Tags;

namespace OakChan.Tests.Markup
{
    public class TagsInteractions
    {
        [TestCase("**__a__**", "<strong><em>a</em></strong>")]
        [TestCase("**____**", "<strong><em></em></strong>")]
        public void NestedTags(string input, string expected)
        {
            MarkupTextProcessor processor = new MarkupTextProcessor(new IMarkupTag[] { new StrongTag(), new EmphasizeTag() });

            var result = processor.ProcessAsync(input).Result;

            Assert.AreEqual(expected, result);
        }


        [Test]
        public void OverlappedTags()
        {
            MarkupTextProcessor processor = new MarkupTextProcessor(new IMarkupTag[] { new StrongTag(), new EmphasizeTag() });

            var result = processor.ProcessAsync("**a__b**c__").Result;

            Assert.AreEqual("<strong>a__b</strong>c__", result);
        }
    }
}
