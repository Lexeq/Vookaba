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
using OakChan.Tests.Integration.Base;
using OakChan.ViewModels;
using System.Threading.Tasks;

namespace OakChan.Tests.Unit.Controllers
{
    public class ThreadControllerTests : ControllerTestsBase
    {
        private ThreadController CreateContorller(IThreadService threadService = null,
            IBoardService boardService = null,
            IStringLocalizer<ThreadController> stringLocalizer = null,
            IMapper mapper = null,
            ILogger<ThreadController> logger = null)
        {
            var controller = new ThreadController(
                threadService ?? Mock.Of<IThreadService>(),
                boardService ?? Mock.Of<IBoardService>(),
                stringLocalizer ?? Mock.Of<IStringLocalizer<ThreadController>>(),
                mapper ?? Mapper,
                logger ?? Mock.Of<ILogger<ThreadController>>());
            SetHttpContext(controller);
            return controller;
        }

        [Test]
        public async Task RequestThread()
        {
            var threadServiceMock = new Mock<IThreadService>(MockBehavior.Strict);
            threadServiceMock.Setup(x => x.GetThreadAsync("b", 42))
                .ReturnsAsync(new ThreadDto
                {
                    ThreadId = 42,
                    Subject = "subj",
                    BoardKey = "b",
                    Posts = new[] { new PostDto() }
                });

            var boardServiceMock = new Mock<IBoardService>(MockBehavior.Strict);
            boardServiceMock.Setup(x => x.GetBoardAsync("b"))
                .ReturnsAsync(new BoardDto { Key = "b", Name = "b" });
            var controller = CreateContorller(threadServiceMock.Object, boardServiceMock.Object);

            var result = await controller.Index("b", 42) as ViewResult;
            var threadVm = result?.ViewData.Model as ThreadViewModel;

            Assert.IsNotNull(threadVm);
            StringAssert.AreEqualIgnoringCase("b", threadVm.BoardKey);
            Assert.AreEqual("subj", threadVm.Subject);
            threadServiceMock.Verify(x => x.GetThreadAsync("b", 42), Times.AtMostOnce());
            boardServiceMock.Verify(x => x.GetBoardAsync("b"), Times.AtMostOnce());
        }

        [Test]
        public async Task RequestNotExistingThread()
        {
            var threadServiceMock = new Mock<IThreadService>();
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(x => x.GetBoardAsync("b"))
                .ReturnsAsync(new BoardDto { Key = "b", Name = "b" });

            var controller = CreateContorller(threadServiceMock.Object, boardServiceMock.Object);
            var id = await controller.Index("b", 42) as ViewResult;
            var errorVm = id?.ViewData.Model as ErrorViewModel;

            Assert.IsNotNull(errorVm);
            Assert.AreEqual(404, errorVm.Code);
            threadServiceMock.Verify(x => x.GetThreadAsync("b", 1), Times.AtMostOnce());
            boardServiceMock.Verify(x => x.GetBoardAsync("b"), Times.AtMostOnce());
        }

        [Test]
        public async Task CreatePost()
        {
            var board = new BoardDto { Key = "b", Name = "b" };
            var thread = new ThreadInfoDto { Subject = "subj", BoardKey = board.Key, ThreadId = 42 };

            var threadServiceMock = new Mock<IThreadService>();
            threadServiceMock.Setup(t => t.GetThreadInfoAsync(board.Key, thread.ThreadId))
                .ReturnsAsync(thread);
            threadServiceMock.Setup(t => t.AddPostToThreadAsync(thread.ThreadId, It.IsAny<PostCreationDto>()))
                .ReturnsAsync(new PostDto { ThreadId = thread.ThreadId });

            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(x => x.GetBoardAsync("b"))
                .ReturnsAsync(board);

            var controller = CreateContorller(threadServiceMock.Object, boardServiceMock.Object);

            var result = await controller.AddPost(board.Key, thread.ThreadId, new PostFormViewModel
            {
                Text = "Hello, world!"
            }) as ViewResult;

            boardServiceMock.Verify(x => x.GetBoardAsync(It.IsAny<string>()), Times.AtMostOnce());
            threadServiceMock.Verify(m => m.AddPostToThreadAsync(thread.ThreadId, It.Is<PostCreationDto>(p => p.Message == "Hello, world!")), Times.Once());
            threadServiceMock.Verify(m => m.GetThreadInfoAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task CreatePostWithInvalidModel()
        {
            var threadServiceMock = new Mock<IThreadService>();
            var controller = CreateContorller();
            controller.ModelState.AddModelError(string.Empty, "test error");

            var res = await controller.AddPost("x", 42, new PostFormViewModel()) as ViewResult;

            threadServiceMock.Verify(m => m.AddPostToThreadAsync(It.IsAny<int>(), It.IsAny<PostCreationDto>()), Times.Never());
            threadServiceMock.Verify(m => m.GetThreadAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task CreatePostOnReadOnlyThread()
        {
            var board = new BoardDto { Key = "x", Name = "x" };
            var thread = new ThreadInfoDto { Subject = "x", BoardKey = board.Key, ThreadId = 42, IsReadOnly = true };

            var threadServiceMock = new Mock<IThreadService>();
            threadServiceMock.Setup(t => t.GetThreadInfoAsync(board.Key, thread.ThreadId))
                .ReturnsAsync(thread);

            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(x => x.GetBoardAsync(board.Key))
                .ReturnsAsync(board);

            var controller = CreateContorller(threadServiceMock.Object, boardServiceMock.Object);

            var res = await controller.AddPost(board.Key, thread.ThreadId, new PostFormViewModel
            {
                Text = "Hello, world!"
            });

            Assert.IsInstanceOf<BadRequestResult>(res);

            threadServiceMock.Verify(m => m.AddPostToThreadAsync(thread.ThreadId, It.Is<PostCreationDto>(p => p.Message == "Hello, world!")), Times.Never());
            threadServiceMock.Verify(m => m.GetThreadInfoAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task CreatePostOnDisabledBoard()
        {
            var board = new BoardDto { Key = "x", IsDisabled = true };
            var thread = new ThreadDto { Subject = "x", BoardKey = board.Key, ThreadId = 42 };
            var threadServiceMock = new Mock<IThreadService>();
            threadServiceMock.Setup(t => t.GetThreadAsync(board.Key, thread.ThreadId))
                .ReturnsAsync(thread);

            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(x => x.GetBoardAsync(board.Key))
                .ReturnsAsync(board);

            var controller = CreateContorller(threadServiceMock.Object, boardServiceMock.Object);

            var res = await controller.AddPost(board.Key, thread.ThreadId, new PostFormViewModel
            {
                Text = "Hello, world!"
            });

            Assert.IsInstanceOf<BadRequestResult>(res);

            threadServiceMock.Verify(m => m.AddPostToThreadAsync(It.IsAny<int>(), It.IsAny<PostCreationDto>()), Times.Never());
        }


        [Test]
        public async Task CreateThread()
        {
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock
                 .Setup(m => m.GetBoardAsync("b"))
                 .ReturnsAsync(new BoardDto
                 {
                     Key = "b"
                 });

            var threadServiceMock = new Mock<IThreadService>();
            threadServiceMock
                 .Setup(m => m.CreateThreadAsync("b", It.Is<ThreadCreationDto>(d => d.Subject == "test thread")))
                 .ReturnsAsync(new ThreadDto
                 {
                     ThreadId = 100,
                     BoardKey = "b"
                 });

            var controller = CreateContorller(threadServiceMock.Object, boardServiceMock.Object);
            const string subj = "test thread";


            var result = await controller.Create("b", new ThreadFormViewModel
            {
                Subject = subj,
                Text = "Hello, world!"
            }) as ViewResult;

            threadServiceMock.Verify(m => m.CreateThreadAsync("b", It.Is<ThreadCreationDto>(d => d.Subject == subj)), Times.Once());
            boardServiceMock.Verify(m => m.GetBoardAsync("b"), Times.Once());
        }

        [Test]
        public async Task CreateThreadWithInvalidModel()
        {
            var boardServiceMock = new Mock<IBoardService>();
            var threadServiceMock = new Mock<IThreadService>();
            var controller = CreateContorller(threadServiceMock.Object, boardServiceMock.Object);
            controller.ModelState.AddModelError(string.Empty, "test error");

            var result = await controller.Create("b", new ThreadFormViewModel()) as ViewResult;

            threadServiceMock.Verify(m => m.CreateThreadAsync(It.IsAny<string>(), It.IsAny<ThreadCreationDto>()), Times.Never());
            boardServiceMock.Verify(m => m.GetBoardAsync(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task CreateThreadOnNotExistingBoard()
        {
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock
                .Setup(x => x.GetBoardAsync(It.IsAny<string>()))
                .ReturnsAsync((BoardDto)null);
            var threadServiceMock = new Mock<IThreadService>();
            var controller = CreateContorller(threadServiceMock.Object, boardServiceMock.Object);

            var result = await controller.Create("b", new ThreadFormViewModel
            {
                Subject = "test thread",
                Text = "hello, world",
                Image = Mock.Of<IFormFile>()
            }) as BadRequestResult;

            Assert.NotNull(result);
            threadServiceMock.Verify(m => m.CreateThreadAsync(It.IsAny<string>(), It.IsAny<ThreadCreationDto>()), Times.Never);
            boardServiceMock.Verify(m => m.GetBoardAsync(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task CreateThreadOnDeletedBoard()
        {
            var threadServiceMock = new Mock<IThreadService>();
            var boardServiceMock = new Mock<IBoardService>();
            boardServiceMock
                .Setup(x => x.GetBoardAsync(It.IsAny<string>()))
                .ReturnsAsync(new BoardDto { IsDisabled = true });
            var controller = CreateContorller(threadServiceMock.Object, boardServiceMock.Object);

            var result = await controller.Create("b", new ThreadFormViewModel
            {
                Subject = "test thread",
                Text = "hello, world",
                Image = Mock.Of<IFormFile>()
            });

            Assert.NotNull(result);
            threadServiceMock.Verify(m => m.CreateThreadAsync(It.IsAny<string>(), It.IsAny<ThreadCreationDto>()), Times.Never);
            boardServiceMock.Verify(m => m.GetBoardAsync(It.IsAny<string>()), Times.Once());
        }
    }
}
