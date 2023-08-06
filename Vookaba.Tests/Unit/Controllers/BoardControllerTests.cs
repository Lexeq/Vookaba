using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Vookaba.Controllers;
using Vookaba.Services.DTO;
using Vookaba.Tests.Integration.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vookaba.Services.Abstractions;
using Vookaba.ViewModels.Board;
using Vookaba.ViewModels.Error;

namespace Vookaba.Tests.Unit.Controllers
{
    public class BoardControllerTests : ControllerTestsBase
    {
        private BoardController CreateContorller(
            IBoardService boardService = null,
            IPostService postService = null,
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
                .Setup(x => x.GetBoardAsync(key))
                .ReturnsAsync(new BoardDto { Key = key, Name = name });
            boardServiceMock
                .Setup(x => x.GetThreadPreviewsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PartialList<ThreadPreviewDto>(0, new List<ThreadPreviewDto>()));
            var controller = CreateContorller(boardServiceMock.Object);

            var id = await controller.Index(key) as ViewResult;
            var page = id?.ViewData.Model as BoardPageViewModel;

            boardServiceMock.Verify(m => m.GetBoardAsync(key), Times.Once);
            Assert.IsNotNull(page);
            StringAssert.AreEqualIgnoringCase(key, page.Key);
            StringAssert.AreEqualIgnoringCase(name, page.Name);
            Assert.AreEqual(1, page.PagesInfo.PageNumber);
        }

        [Test]
        public async Task RequestDisabledBoard()
        {
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock
                .Setup(x => x.GetBoardAsync(It.IsAny<string>()))
                .ReturnsAsync(new BoardDto { IsDisabled = true });
            var controller = CreateContorller(boardServiceMock.Object);

            var result = await controller.Index("b") as ViewResult;

            Assert.NotNull(result);
            Assert.IsInstanceOf<ErrorViewModel>(result.Model);
            boardServiceMock.Verify(m => m.GetBoardAsync(It.IsAny<string>()), Times.Once());
            boardServiceMock.Verify(m => m.GetThreadPreviewsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }


        [TestCase(2)]
        [TestCase(-2)]
        [TestCase(0)]
        public async Task RequestNonExistingPage(int page)
        {
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock
                .Setup(x => x.GetBoardAsync(It.IsAny<string>()))
                .ReturnsAsync(new BoardDto { Key = "b", Name = "Random" });
            boardServiceMock.Setup(x => x.GetThreadPreviewsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PartialList<ThreadPreviewDto>(0, new List<ThreadPreviewDto>()));
            var controller = CreateContorller(boardServiceMock.Object);

            var result = await controller.Index("b", page) as ViewResult;

            Assert.IsInstanceOf<ErrorViewModel>(result.ViewData.Model);
        }

        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(1, Common.ApplicationConstants.BoardConstants.PageSize)]
        [TestCase(2, Common.ApplicationConstants.BoardConstants.PageSize + 1)]
        [TestCase(5, Common.ApplicationConstants.BoardConstants.PageSize * 5)]
        public async Task PagesCountCalculation(int expected, int source)
        {
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(x => x.GetBoardAsync("b"))
             .ReturnsAsync(new BoardDto { Key = "b", Name = "Random" });
            boardServiceMock.Setup(x => x.GetThreadPreviewsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PartialList<ThreadPreviewDto>(source, Array.Empty<ThreadPreviewDto>()));
            var controller = CreateContorller(boardServiceMock.Object);

            var viewResult = await controller.Index("b", 1) as ViewResult;
            var viewModel = viewResult.Model as BoardPageViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expected, viewModel.PagesInfo.TotalPages);
        }
    }
}
