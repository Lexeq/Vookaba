using AutoMapper;
using OakChan.Services.Mapping;

namespace OakChan.Tests.Base
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
