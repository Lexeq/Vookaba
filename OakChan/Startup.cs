using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OakChan.Attributes;
using OakChan.Common.Exceptions;
using OakChan.DAL;
using OakChan.DAL.Database;
using OakChan.Deanon;
using OakChan.Identity;
using OakChan.Mapping;
using OakChan.Services;
using OakChan.Services.Mapping;
using OakChan.Utils;

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
            services.AddSingleton<IAttachmentsStorage>(
                svc => new MediaStorage(svc.GetRequiredService<IWebHostEnvironment>().WebRootPath, svc.GetRequiredService<ILogger<MediaStorage>>()));

            services.AddScoped<IBoardService, DbBoardService>();
            services.AddScoped<IUserService, DbUserService>();
            services.AddScoped<IThreadService, DbThreadService>();
            services.AddScoped<IPostService, DbPostService>();
            services.AddScoped<FavoriteThreadsService>();
            services.AddSingleton<IHashService>(new HashService());
            services.AddSingleton<ThrowHelper>();

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

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<OakDbContext>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>()
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Administration/AccessDenied";
                options.Cookie.Name = "passport";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Administration/Account/Login";
                options.Cookie.IsEssential = true;
                options.SlidingExpiration = true;
            });

            services.Configure<IdentityOptions>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.User.AllowedUserNameCharacters = OakConstants.AllowedUserNameCharacters;

                o.Password.RequireDigit = true;
                o.Password.RequiredLength = OakConstants.MinPasswordLength;
                o.Password.RequiredUniqueChars = 2;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
            });

            services.AddOptions();
            services.AddScoped<DatabaseSeeder>();
            services.Configure<SeedData>(Configuration.GetSection(nameof(SeedData)));
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
            app.UseStatusCodePagesWithReExecute("/error/HandleHttpStatusCode/{0}");
            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDeanon();
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
