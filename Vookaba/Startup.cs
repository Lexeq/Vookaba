using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vookaba.Attributes;
using Vookaba.Common;
using Vookaba.Common.Exceptions;
using Vookaba.DAL.Database;
using Vookaba.DAL.MediaStorage;
using Vookaba.Infrastructure.Deanon;
using Vookaba.Extensions;
using Vookaba.Extensions.DependencyInjection;
using Vookaba.Mapping;
using Vookaba.Security.DependecyInjection;
using Vookaba.Services;
using Vookaba.Services.Abstractions;
using Vookaba.Services.DbServices;
using Vookaba.Services.Mapping;
using Vookaba.Utils;
using Microsoft.AspNetCore.Authorization;
using Vookaba.Security.AuthorizationHandlers;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vookaba.Security;

namespace Vookaba
{
    public class Startup
    {
#if DEBUG
        public ILoggerFactory EfLoggerFactory = LoggerFactory.Create(o =>
        {
            o.ClearProviders();
            o.AddDebug();
        });
#endif
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //TODO: clean up this mess
            #region DB
            services.AddDbContext<VookabaDbContext>(options =>
                options.UseNpgsql("Host=localhost;Port=5432;Database=BigDataOak2;Username=oak;Password=oakpassword", x =>
                x.MigrationsAssembly("Vookaba.DAL"))
#if DEBUG
                .UseLoggerFactory(EfLoggerFactory)
                .EnableSensitiveDataLogging(true)
#endif
                );

            services.AddScoped<DatabaseSeeder>();
            services.Configure<SeedData>(Configuration.GetSection(nameof(SeedData)));
            #endregion

            services.AddSingleton<IAttachmentsStorage>(
                svc => new MediaStorage(svc.GetRequiredService<IWebHostEnvironment>().WebRootPath, svc.GetRequiredService<ILogger<MediaStorage>>()));

            services.AddScoped<IBoardService, DbBoardService>();
            services.AddScoped<IThreadService, DbThreadService>();
            services.AddScoped<IPostService, DbPostService>();
            services.AddScoped<IStaffAggregationService, DbStaffAggregationService>();
            services.AddScoped<IModLogService, DbModLogService>();
            services.AddScoped<ITopThreadsService, TopThreadsService>();
            services.AddScoped<IBanService, DbBanService>();
            services.AddSingleton<IHashService>(new HashService());
            services.AddSingleton<ModLogDescriber>();
            services.AddSingleton<IPostProcessor, TripcodePostProcessor>();

            services.AddPostMarkup();

            services.AddDeanon();
            services.AddSingleton<IValidationAttributeAdapterProvider, VookabaValidationAttributeAdapterProvider>();

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Environment.WebRootPath));

            var mvcBuilder = services.AddMvc();

            #region Localization

            services.Configure<RequestLocalizationOptions>(o =>
            {
                o.RequestCultureProviders.Clear();
                o.SetDefaultCulture(ApplicationConstants.Culture);
            });

            services.AddLocalization(o => o.ResourcesPath = "resources/localization");
            mvcBuilder.AddMvcLocalization();
            #endregion

            services.Configure<ForwardedHeadersOptions>(o =>
            {
                o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddAutoMapper(cfg =>
            {
                cfg.DisableConstructorMapping();
                cfg.AddProfile<ServicesMapProfile>();
                cfg.AddProfile<ViewModelsMapProfile>();
            });

            #region Identity
            services.AddChanIdentity()
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
                .AddDbStores();

            services.Configure<IdentityOptions>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.User.AllowedUserNameCharacters = ApplicationConstants.Identity.AllowedUserNameCharacters;

                o.Password.RequireDigit = true;
                o.Password.RequiredLength = ApplicationConstants.Identity.MinPasswordLength;
                o.Password.RequiredUniqueChars = 2;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
            });
            #endregion

            services.AddDataProtection(x =>
            {
                x.ApplicationDiscriminator = "37f7095fa7d8419db8248814abf50326b1207665e75c456cabf8cdf051c912a1";// Configuration[nameof(x.ApplicationDiscriminator)];
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Administration/Account/Login";
                options.LogoutPath = "/Administration/Account/Logout";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(ApplicationConstants.Identity.CookieExpireInDays);
                options.Cookie.Name = "passport";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.MaxAge = TimeSpan.FromDays(ApplicationConstants.Identity.CookieMaxAgeInDays);
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                options.EventsType = typeof(Security.AppCookieEvents);
            });

            services.AddChanPolicies();
            services.AddScoped<HttpStatusCodeDescriber>();

            services.AddApiVersioning();

            services.Configure<ApplicationOptions>(Configuration.GetSection("ApplicationOptions"), o => o.BindNonPublicProperties = true);
            services.AddOptions();
            services.AddSingleton<OptionsRewriter>();
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

            app.UseForwardedHeaders();
            app.UseStaticFiles();
            app.Use(async (x, y) =>
            {
                var path = x.Request.Path.Value;
                if (path.StartsWith("/res/") && path.LastIndexOf(".") > 0)
                {
                    x.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else
                {
                    await y.Invoke();
                }
            });
            app.UseRequestLocalization();
            app.UseStatusCodePagesWithReExecute("/error/HandleHttpStatusCode/{0}");
            app.UseRouting();
            app.UseAuthentication();
            app.UseDeanon();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "AdministrationArea",
                    areaName: "Administration",
                    pattern: "Administration/{controller}/{action}");

                endpoints.MapControllerRoute(
                  name: "error",
                  pattern: "error/{action}/{statusCode?}",
                  defaults: new { Controller = "Error" });

                endpoints.MapControllerRoute(
                  name: "home",
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
                    pattern: "boards/{action}/{board:alpha?}",
                    defaults: new { Controller = "Board" });

                endpoints.MapControllerRoute(
                    name: "threadAction",
                    pattern: "threads/{board:alpha}/{thread:int}/{action}",
                    defaults: new { Controller = "Thread" });
            });
        }
    }
}
