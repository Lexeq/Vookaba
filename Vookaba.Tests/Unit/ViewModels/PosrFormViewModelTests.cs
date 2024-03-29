﻿using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Vookaba.Tests.Unit.Base;
using Vookaba.ViewModels;
using Vookaba.ViewModels.Post;

namespace Vookaba.Tests.Unit.ViewModels
{
    public class PostFormViewModelTests : ViewModelValidationTestsBase
    {
        [Test]
        public void ModelValidationValid()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.SetupGet(f => f.FileName).Returns("pic.jpg");
            var vm = new PostFormViewModel
            {
                Image = fileMock.Object,
                Text = "not empty",
                Name = "A"
            };

            var isValid = ValidateViewModel(vm, out _);

            Assert.IsTrue(isValid);
        }

        [Test]
        public void ModelValidationTextNoFile()
        {
            var vm = new PostFormViewModel
            {
                Image = null,
                Text = "message",
            };
            var isValid = ValidateViewModel(vm, out _);

            Assert.IsTrue(isValid);
        }

        [Test]
        public void ModelValidationFileNoText()
        {
            var vm = new PostFormViewModel
            {
                Image = null,
                Text = "message",
            };
            var isValid = ValidateViewModel(vm, out _);

            Assert.IsTrue(isValid);
        }

        [TestCase(null)]
        [TestCase("")]
        public void ModelValidationNoFileNoText(string msg)
        {
            var vm = new PostFormViewModel
            {
                Image = null,
                Text = msg,
            };
            var isValid = ValidateViewModel(vm, out _);

            Assert.IsFalse(isValid);
        }
    }
}
