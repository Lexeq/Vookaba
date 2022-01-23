using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using OakChan.Tests.Unit.Base;
using OakChan.ViewModels;

namespace OakChan.Tests.Unit.ViewModels
{
    public class ThreadFormViewModelTests : ViewModelValidationTestsBase
    {
        [Test]
        public void ModelValidation()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.SetupGet(f => f.FileName).Returns("pic.jpg");
            var vm = new ThreadFormViewModel
            {
                Image = fileMock.Object,
                Text = "not empty",
                Subject = null,
                Name = null
            };

            var isValid = ValidateViewModel(vm, out _);

            Assert.IsTrue(isValid);
        }

        [Test]
        public void ModelValidationNoImage()
        {
            var vm = new ThreadFormViewModel
            {
                Image = null,
                Text = "not empty",
            };
            var isValid = ValidateViewModel(vm, out _);

            Assert.IsFalse(isValid);
        }

        [TestCase(null)]
        [TestCase("")]
        public void ModelValidationBadText(string text)
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.SetupGet(f => f.FileName).Returns("pic.jpg");
            var vm = new ThreadFormViewModel
            {
                Image = fileMock.Object,
                Text = text
            };
            var isValid = ValidateViewModel(vm, out _);

            Assert.IsFalse(isValid);
        }
    }
}
