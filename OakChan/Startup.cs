using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OakChan.Attributes;
using OakChan.Deanon;
using OakChan.Models;
using OakChan.Models.DB;
using OakChan.Models.Interfces;

namespace OakChan
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OakDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("Postgre")));
            services.AddSingleton<MediaStorage>();

            services.AddScoped<PostCreator>();
            services.AddScoped<IBoardService, DbBoardService>();
            services.AddScoped<IUserService, DbUserService>();
            services.AddScoped<IThreadService, DbThreadService>();

            services.AddSingleton<IValidationAttributeAdapterProvider, OakValidatiomAttributeAdapterProvider>();
            
            services.AddAuthentication()
                .AddDeanonCookie();

            services.AddAuthorization(options =>
            {
                options.AddDeanonPolicy();
            });
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Environment.WebRootPath));

            var mvcBuilder = services.AddMvc();

            #region Localization

            var supportedCultures = new[] { new CultureInfo("ru-ru") };
            services.Configure<RequestLocalizationOptions>(o =>
            {
                o.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                o.SupportedCultures = supportedCultures;
                o.SupportedUICultures = supportedCultures;
            });

            services.AddLocalization(o => o.ResourcesPath = "Resources\\Localization");
            mvcBuilder.AddMvcLocalization();
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/HandleException/");
            }

            app.UseStatusCodePagesWithReExecute("/error/HandleHttpStatusCode/{0}");
            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDeanon();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "error",
                  pattern: "error/{action}/{statusCode?}",
                  defaults: new { Controller = "Error" });

                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "/",
                  defaults: new { Controller = "Home", Action = "Index" });

                endpoints.MapControllerRoute(
                    name: "board",
                    pattern: "{board:alpha}",
                    defaults: new { Controller = "Board", Action = "Index" });

                endpoints.MapControllerRoute(
                   name: "thread",
                   pattern: "{board:alpha}/{thread:int}",
                   defaults: new { Controller = "Thread", Action = "Index" });

                endpoints.MapControllerRoute(
                    name: "boardAction",
                    pattern: "{board:alpha}/{action}",
                    defaults: new { Controller = "Board" });

                endpoints.MapControllerRoute(
                    name: "threadAction",
                    pattern: "{board:alpha}/{thread:int}/{action}",
                    defaults: new { Controller = "Thread" });
            });
        }
    }
}
