using Microsoft.Extensions.DependencyInjection;
using OpenAIExtensions.Chats;
using OpenAIExtensions.Services;
using OpenAIExtensions.Text2Sql;

namespace OpenAIExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddOpenAI(this IServiceCollection services)
        {
            services.AddScoped<IAIAudioService,AIAudioService>();
            services.AddScoped<IAIConversationManager, AIConversationManager>();
            services.AddScoped<IAIImageService, AIImageService>();
            services.AddScoped<AITranslationService, AITranslationService>();
            services.AddScoped<IAISqlGenerator, AISqlGenerator>();
        }
    }
}