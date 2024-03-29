﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vookaba.Mapping;

namespace Vookaba.Tests.Integration.Base
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
