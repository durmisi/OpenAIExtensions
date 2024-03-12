using Microsoft.Extensions.DependencyInjection;
using OpenAIExtensions.Services;
using OpenAIExtensions.Text2Sql;

namespace OpenAIExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddOpenAI(this IServiceCollection services)
        {
            services.AddScoped<IAIBroker, AIBroker>();

            services.AddScoped<IAISqlGenerator, AISqlGenerator>();
            services.AddScoped<IAIAudioService, IAIAudioService>();
            services.AddScoped<IAIEmbeddingsGenerator, AIEmbeddingsGenerator>();
          
        }
    }
}
