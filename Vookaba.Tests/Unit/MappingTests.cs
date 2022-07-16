using AutoMapper;
using NUnit.Framework;
using Vookaba.Mapping;
using Vookaba.Services.Mapping;

namespace Vookaba.Tests.Unit
{
    public class MappingTests
    {
        [Test]
        public void ValidateServiceConfigurations()
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile<ServicesMapProfile>()).CreateMapper();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Test]
        public void ValidateViewModelsMappingConfigurations()
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile<ViewModelsMapProfile>()).CreateMapper();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
