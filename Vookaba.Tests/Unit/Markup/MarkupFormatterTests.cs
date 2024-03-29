﻿using Moq;
using NUnit.Framework;
using Vookaba.Markup;
using System.Linq;
using System.Threading.Tasks;

namespace Vookaba.Tests.Unit.Markup
{
    public class MarkupFormatterTests
    {
        [Test]
        public async Task SetEncodedMessage()
        {
            var factory = new Mock<IMarkupTagsFactory>();
            factory.Setup(x => x.GetTags())
                .Returns(Enumerable.Empty<IMarkupTag>());

            var post = new Vookaba.Services.DTO.PostCreationDto { Message = "message" };
            var mf = new MarkupFormatter(factory.Object);
            await mf.ProcessAsync(post);

            Assert.False(string.IsNullOrEmpty(post.EncodedMessage));
        }
    }
}
