using System;
using System.Globalization;
using System.Net;
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
using OakChan.Common;
using OakChan.Common.Exceptions;
using OakChan.DAL;
using OakChan.DAL.Database;
using OakChan.Deanon;
using OakChan.Identity;
using OakChan.Mapping;
using OakChan.Policies;
using OakChan.Services;
using OakChan.Services.DbServices;
using OakChan.Services.Mapping;
using OakChan.Utils;

namespace OakChan
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
            services.AddDbContext<OakDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Postgre"))
#if DEBUG
                .UseLoggerFactory(EfLoggerFactory)
#endif
                );
            services.AddSingleton<IAttachmentsStorage>(
                svc => new MediaStorage(svc.GetRequiredService<IWebHostEnvironment>().WebRootPath, svc.GetRequiredService<ILogger<MediaStorage>>()));

            services.AddScoped<IBoardService, DbBoardService>();
            services.AddScoped<IThreadService, DbThreadService>();
            services.AddScoped<IPostService, DbPostService>();
            services.AddScoped<IStaffAggregationService, DbStaffAggregationService>();
            services.AddScoped<IModLogService, DbModLogService>();
            services.AddScoped<ITopThreadsService, TopThreadsService>();
            services.AddSingleton<IHashService>(new HashService());
            services.AddSingleton<ThrowHelper>();
            services.AddSingleton<ModLogDescriber>();

            services.AddDeanon();
            services.AddSingleton<IValidationAttributeAdapterProvider, OakValidatiomAttributeAdapterProvider>();

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
                .AddUserManager<ApplicationUserManager>()
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
                .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();
            services.AddScoped(s => (ApplicationUserStore)s.GetRequiredService<IUserStore<ApplicationUser>>());
            services.AddScoped<IInvitationStore<ApplicationInvitation>>(s => s.GetRequiredService<ApplicationUserStore>());
            services.AddScoped<IUserInvitationStore<ApplicationUser>>(s => s.GetRequiredService<ApplicationUserStore>());
            services.AddScoped<InvitationManager<ApplicationInvitation>>();

            services.AddScoped<AuthorTokenManager>();
            services.AddScoped<IAuthorTokenFactory>(s => s.GetRequiredService<AuthorTokenManager>());
            services.AddScoped<IAuthorTokenManager, AuthorTokenManager>(s => s.GetRequiredService<AuthorTokenManager>());

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Administration/Account/AccessDenied";
                options.LoginPath = "/Administration/Account/Login";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(OakConstants.Identity.CookieExpireInDays);
                options.Cookie.Name = "passport";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.MaxAge = TimeSpan.FromDays(OakConstants.Identity.CookieMaxAgeInDays);
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;

                options.EventsType = typeof(CookieValidator);
            });

            services.AddScoped<CookieValidator>();

            services.Configure<IdentityOptions>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.User.AllowedUserNameCharacters = OakConstants.Identity.AllowedUserNameCharacters;

                o.Password.RequireDigit = true;
                o.Password.RequiredLength = OakConstants.Identity.MinPasswordLength;
                o.Password.RequiredUniqueChars = 2;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
            });

            services.AddChanPolicies();
            services.AddScoped<HttpStatusCodeDescriber>();

            services.Configure<ChanOptions>(o => o.PublicRegistrationEnabled = false);
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
            app.UseStatusCodePagesWithReExecute("/error/HandleHttpStatusCode/{0}");
            app.UseRequestLocalization();
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
