using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OakChan.Deanon;
using OakChan.Mapping;
using System;

namespace OakChan.Tests.Base
{
    public class ControllerTestsBase
    {
        private readonly MapperConfiguration mapperConfiguration;

        protected IMapper Mapper => mapperConfiguration.CreateMapper();

        public ControllerTestsBase()
        {
            mapperConfiguration = new MapperConfiguration(o => o.AddProfile<ViewModelsMapProfile>());
        }

        protected void SetDeanonFeature(Controller controller)
        {
            var deanonMock = new Mock<IDeanonFeature>();
            deanonMock
                .Setup(x => x.IPAddress)
                .Returns(new System.Net.IPAddress(0));
            deanonMock
                .Setup(x => x.UserAgent)
                .Returns("UA-Tests");
            deanonMock
                .Setup(x => x.UserToken)
                .Returns(new Guid());
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Features.Set(deanonMock.Object);
        }
    }
}
