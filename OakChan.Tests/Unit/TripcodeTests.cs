﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using OakChan.Services.DTO;
using OakChan.Utils;
using System;
using System.Security.Claims;

namespace OakChan.Tests.Unit
{
    public class TripcodeTests
    {
        [TestCase("", "", false)]
        [TestCase("abc", "abc", false)]
        [TestCase(null, null, false)]
        [TestCase("name##", "name", true)]
        [TestCase("name##123", "name", true)]
        [TestCase("##", "", true)]
        [TestCase("##123", "", true)]
        public void TripcodeDetectingTest(string name, string newName, bool hasTripcode)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(Common.OakConstants.ClaimTypes.AuthorToken, new Guid().ToString()) }
                ));

            var httpAccessorMock = new Mock<IHttpContextAccessor>();
            httpAccessorMock.SetupGet(x => x.HttpContext).Returns(new DefaultHttpContext()
            {
                User = user
            });

            var optionsMock = new Mock<IOptions<DataProtectionOptions>>();
            optionsMock.SetupGet(x => x.Value)
                .Returns(new DataProtectionOptions()
                {
                    ApplicationDiscriminator = "1234"
                });

            var tripProc = new TripcodePostProcessor(httpAccessorMock.Object, optionsMock.Object);
            var pdo = new PostCreationDto
            {
                AuthorName = name
            };
            tripProc.ProcessAsync(pdo).Wait();
            Assert.AreEqual(newName, pdo.AuthorName);
            Assert.AreNotEqual(hasTripcode, string.IsNullOrEmpty(pdo.Tripcode));
        }
    }
}