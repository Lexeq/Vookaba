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

        protected void SetHttpContext(Controller controller)
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }
    }
}
