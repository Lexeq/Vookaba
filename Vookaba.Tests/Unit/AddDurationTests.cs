using NUnit.Framework;
using System;
using Vookaba.Common.Extensions;

namespace Vookaba.Tests.Unit
{
    public class AddDurationTests
    {
        [TestCaseSource(nameof(Cases))]
        public void AddTest(DateTime original, string input, DateTime expected)
        {
            var isSccessfull = original.TryAddFromString(input ,out var result);
            Assert.IsTrue(isSccessfull);
            Assert.AreEqual(expected, result);
        }

        static readonly object[] Cases =
        {
            new object[] { new DateTime(2000, 1, 1), "2d", new DateTime(2000, 1, 3) },
            new object[] { new DateTime(2000, 1, 1), "2d2m", new DateTime(2000, 1, 3,0,2,0) },
            new object[] { new DateTime(2000, 1, 1), "2d  2m", new DateTime(2000, 1, 3,0,2,0) },
            new object[] { new DateTime(2000, 1, 1), "2d2d", new DateTime(2000, 1, 5) }
        };
    }
}
