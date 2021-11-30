using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OakChan.Controllers;
using OakChan.Services;
using OakChan.Services.DTO;
using OakChan.Tests.Base;
using OakChan.ViewModels;
using System.Threading.Tasks;

namespace OakChan.Tests.Controllers
{
    public class BoardControllerTests : ControllerTestsBase
    {
        private BoardController CreateContorller(
            IBoardService boardService = null,
            IStringLocalizer<BoardController> stringLocalizer = null,
            IMapper mapper = null,
            IModLogService modLogService = null,
            ILogger<BoardController> logger = null)
        {
            var controller = new BoardController(
                boardService ?? Mock.Of<IBoardService>(),
                stringLocalizer ?? Mock.Of<IStringLocalizer<BoardController>>(),
                mapper ?? Mapper,
                modLogService ?? Mock.Of<IModLogService>(),
                logger ?? Mock.Of<ILogger<BoardController>>());
            SetHttpContext(controller);
            return controller;
        }

        [TestCase("b", "random")]
        public async Task RequestBoardPage(string key, string name)
        {
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock
                .Setup(x => x.GetBoardInfoAsync(key))
                .ReturnsAsync(new BoardInfoDto { Key = key, Name = name });
            var controller = CreateContorller(boardServiceMock.Object);

            var id = await controller.Index(key) as ViewResult;
            var page = id?.ViewData.Model as BoardPageViewModel;

            boardServiceMock.Verify(m => m.GetBoardInfoAsync(key), Times.Once);
            Assert.IsNotNull(page);
            StringAssert.AreEqualIgnoringCase(key, page.Key);
            StringAssert.AreEqualIgnoringCase(name, page.Name);
            Assert.AreEqual(1, page.PagesInfo.PageNumber);
        }

        [Test]
        public async Task RequestDeletedBoard()
        {
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock
                .Setup(x => x.GetBoardInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(new BoardInfoDto { IsDisabled = true });
            var controller = CreateContorller(boardServiceMock.Object);

            var result = await controller.Index("b") as ViewResult;

            Assert.NotNull(result);
            Assert.IsInstanceOf<ErrorViewModel>(result.Model);
            boardServiceMock.Verify(m => m.GetBoardInfoAsync(It.IsAny<string>()), Times.Once());
            boardServiceMock.Verify(m => m.GetThreadPreviewsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }


        [TestCase(2)]
        [TestCase(-2)]
        [TestCase(0)]
        public async Task RequestNonExistingPage(int page)
        {
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock
                .Setup(x => x.GetBoardInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(new BoardInfoDto { Key = "b", Name = "Random" });
            var controller = CreateContorller(boardServiceMock.Object);

            var result = await controller.Index("b", page) as ViewResult;

            boardServiceMock.Verify(m => m.GetThreadPreviewsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            Assert.IsInstanceOf<ErrorViewModel>(result.ViewData.Model);
        }

        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(1, 10)]
        [TestCase(2, 11)]
        public async Task PagesCountCalculation(int expected, int source)
        {
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(x => x.GetBoardInfoAsync("b"))
             .ReturnsAsync(new BoardInfoDto { Key = "b", Name = "Random", ThreadsCount = source });
            var controller = CreateContorller(boardServiceMock.Object);

            var viewResult = await controller.Index("b", 1, 10) as ViewResult;
            var viewModel = viewResult.Model as BoardPageViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expected, viewModel.PagesInfo.TotalPages);
        }
    }
}
