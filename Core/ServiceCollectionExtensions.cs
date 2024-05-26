using Microsoft.Extensions.DependencyInjection;
using OpenAIExtensions.Managers;

namespace OpenAIExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAIConversationManager(this IServiceCollection services)
        {
            services.AddScoped<IAIConversationManager, AIConversationManager>();
        }
    }
}