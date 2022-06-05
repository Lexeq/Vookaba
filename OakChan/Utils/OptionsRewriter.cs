﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OakChan.Utils
{
    public class OptionsRewriter
    {
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        private readonly IFileProvider files;
        private readonly IConfigurationRoot configuration;
        private readonly IServiceProvider services;

        public OptionsRewriter(IWebHostEnvironment environment,
                               IConfiguration configuration,
                               IServiceProvider services)
        {
            files = environment.ContentRootFileProvider;
            this.configuration = (IConfigurationRoot)configuration;
            this.services = services;
        }

        public void Write<T>(T options, string sectionName = null, bool reloadOptions = false) where T : class
        {
            sectionName ??= typeof(T).Name;
            var fi = files.GetFileInfo(Common.OakConstants.UserOptionsFileName);

            using (var stream = File.Open(fi.PhysicalPath, FileMode.OpenOrCreate))
            {
                var currentOptions = stream.Length != 0 ?
                    JsonSerializer.Deserialize<Dictionary<string, object>>(stream) :
                    new Dictionary<string, object>();
                currentOptions[sectionName] = options;
                stream.Position = 0;
                stream.SetLength(0);
                JsonSerializer.Serialize(stream, currentOptions, jsonSerializerOptions);
            }

            if (reloadOptions)
            {
                configuration.Reload();
                var optionsObject = services.GetRequiredService<IOptions<T>>();
                configuration.GetRequiredSection(sectionName).Bind(optionsObject.Value, x => x.BindNonPublicProperties = true);
            }
        }
    }
}
