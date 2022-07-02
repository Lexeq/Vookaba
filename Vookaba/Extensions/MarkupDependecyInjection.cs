using Microsoft.Extensions.DependencyInjection;
using Vookaba.Markup;
using Vookaba.Services.Abstractions;
using Vookaba.Utils;

namespace Vookaba.Extensions
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
