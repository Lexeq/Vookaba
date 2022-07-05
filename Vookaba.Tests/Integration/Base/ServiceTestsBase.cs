using AutoMapper;
using Vookaba.Services.Mapping;

namespace Vookaba.Tests.Integration.Base
{
    public class ServiceTestsBase : PostgreContextTestsBase
    {
        protected IMapper ServiceDtoMapper { get; private set; }

        public ServiceTestsBase()
        {
            ServiceDtoMapper = new MapperConfiguration(x => x.AddProfile<ServicesMapProfile>()).CreateMapper();
        }
    }
}
