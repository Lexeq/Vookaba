﻿using NUnit.Framework;
using OakChan.Markup.Tags;
using System;
using System.Threading.Tasks;

namespace OakChan.Tests.Markup
{
    public class QuoteTagTests
    {
        [TestCase(">abc", "abc")]
        [TestCase(">abc\ndef", "abc")]
        [TestCase(">>abc", ">abc")]
        public async Task ContentTest(string input, string expected)
        {
            QuoteTag tag = new QuoteTag();

            var result = await tag.TryOpenAsync(input.AsMemory());

            Assert.AreEqual(expected, result.Content.ToString());
            Assert.AreEqual(expected.Length + 1, result.Length);
        }
    }
}
