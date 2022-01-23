using Moq;
using NUnit.Framework;
using OakChan.Markup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Tests.Markup
{
    public class MarkupTextProcessorTests
    {
        [TestCase("a\nb", "a<br>b")]
        [TestCase("a\r\nb", "a<br>b")]
        [TestCase("a\r\n\nb", "a<br><br>b")]

        [TestCase("<script>abc</script>", "&lt;script&gt;abc&lt;/script&gt;")]
        [TestCase("a&b", "a&amp;b")]
        [TestCase("\"abc\"", "&quot;abc&quot;")]
        public void EscapeChars(string input, string expected)
        {
            MarkupTextProcessor processor = new MarkupTextProcessor(Enumerable.Empty<IMarkupTag>());

            var result = processor.ProcessAsync(input).Result;
            Assert.AreEqual(expected, result);
        }

        [TestCase("abc", 1)]
        [TestCase("abc\n", 1)]
        [TestCase("ab\ncd", 2)]
        [TestCase("ab\n\ncd", 3)]
        public async Task NewLineRequirementConsidered(string input, int callCount)
        {
            var tagMock = new Mock<IMarkupTag>();
            tagMock.Setup(x => x.NewLineRequired).Returns(true);
            tagMock.Setup(x => x.TryOpenAsync(It.IsAny<ReadOnlyMemory<char>>()))
                .ReturnsAsync(TagResult.Fail);

            MarkupTextProcessor processor = new MarkupTextProcessor(new[] { tagMock.Object });

            var result = await processor.ProcessAsync(input);

            tagMock.Verify(x => x.TryOpenAsync(It.IsAny<ReadOnlyMemory<char>>()), Times.Exactly(callCount));
        }

        [Test]
        public async Task RequirementConsidered()
        {
            var testString = "test";

            var outerTagMock = new Mock<IMarkupTag>();
            outerTagMock.Setup(x => x.InnerTagsAllowed).Returns(false);
            outerTagMock.Setup(x => x.TryOpenAsync(It.IsAny<ReadOnlyMemory<char>>()))
                .ReturnsAsync((ReadOnlyMemory<char> mem) => mem.Equals(testString.AsMemory()) ? TagResult.Found(
                  length: testString.Length,
                  content: testString.AsMemory()[1..],
                  opening: "",
                  closing: "") : TagResult.Fail);


            var innerTagMock = new Mock<IMarkupTag>();
            innerTagMock.Setup(x => x.TryOpenAsync(It.IsAny<ReadOnlyMemory<char>>()))
                .ReturnsAsync(TagResult.Fail);

            MarkupTextProcessor processor = new MarkupTextProcessor(new[] { outerTagMock.Object, innerTagMock.Object });

            var result = await processor.ProcessAsync(testString);

            innerTagMock.Verify(x => x.TryOpenAsync(It.IsAny<ReadOnlyMemory<char>>()), Times.Never);
        }
    }
}