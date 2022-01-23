using NUnit.Framework;
using OakChan.Markup;
using OakChan.Markup.Tags;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OakChan.Tests.Markup
{

    public class MarkupTagTests
    {
        [TestCase("a++b++c", false)]
        [TestCase("++b++", true)]
        [TestCase("++++", true)]
        [TestCase("++b", false)]
        [TestCase("++b+", false)]
        [TestCase("++a\nb++", true)]
        public async Task OpenTag(string input, bool success)
        {
            var tag = new MarkupTag("++", "t");

            var result = await tag.TryOpenAsync(input.AsMemory());

            Assert.AreEqual(success, result.Successed);
        }


        [TestCase("[T]a[T]", true, true)]
        [TestCase("[t]a[t]", true, false)]
        [TestCase("[T]a[t]", true, false)]
        [TestCase("[T]a[T]", false, true)]
        [TestCase("[t]a[t]", false, true)]
        [TestCase("[T]a[t]", false, true)]
        public async Task CaseSensitivity(string input, bool caseSensetive, bool success)
        {
            var tag = new MarkupTag("[T]", "t", caseSensetive: caseSensetive);

            var result = await tag.TryOpenAsync(input.AsMemory());

            Assert.AreEqual(success, result.Successed);
        }


        [Test]
        public async Task SetClass()
        {
            var tag = new MarkupTag("++", "t", "test");

            var result = await tag.TryOpenAsync("++a++".AsMemory());
            var hasClass = Regex.IsMatch(result.OpeningHtml, @"<t.*class="".*test.*"">");

            Assert.IsTrue(hasClass);
        }


        [TestCase("++123++45", "123")]
        [TestCase("++++45", "")]
        [TestCase("++1\n23++45", "1\n23")]
        public async Task TagInnerContent(string input, string content)
        {
            var tag = new MarkupTag("++", "t", "test");

            var result = await tag.TryOpenAsync(input.AsMemory());

            Assert.AreEqual(content, result.Content.ToString());
        }
    }
}
