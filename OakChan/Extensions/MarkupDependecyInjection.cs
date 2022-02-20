using Microsoft.Extensions.DependencyInjection;
using OakChan.Markup;
using OakChan.Services;
using OakChan.Utils;

namespace OakChan.Extensions
{
    public static class MarkupDependecyInjection
    {
        public static void AddPostMarkup(this IServiceCollection services)
        {
            services.AddSingleton<IMarkupTagsFactory, MarkupTagsFactory>();
            services.AddSingleton<IPostProcessor, MarkupFormatter>();
        }
    }
}
