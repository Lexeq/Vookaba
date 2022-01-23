using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OakChan.Markup;
using OakChan.Markup.Tags;
using OakChan.Services;
using OakChan.Utils;

namespace OakChan.Extensions
{
    public static class MarkupDependecyInjection
    {
        public static void AddPostMarkup(this IServiceCollection services)
        {
            services.AddSingleton<IMarkupTagsFactory, MarkupTagsFactory>();
            services.AddScoped<IHtmlFormatter, MarkupFormatter>();
        }
    }
}
