using AutoMapper;
using NUnit.Framework;
using OakChan.Mapping;
using OakChan.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakChan.Tests
{
    public class MappingTests
    {
        [Test]
        public void ValidateServiceConfigurations()
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile(new ServicesMapProfile())).CreateMapper();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Test]
        public void ValidateViewModelsMappingConfigurations()
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile(new ViewModelsMapProfile())).CreateMapper();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
