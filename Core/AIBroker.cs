using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace OpenAIExtensions
{

    public interface IAIBroker
    {
        public OpenAIClient GetClient();
        public OpenAIClient GetClient(string endpoint, string? key);

    }

    public class AIBroker : IAIBroker
    {

        private readonly IConfiguration? _configuration;
        private readonly string? _endpoint;
        private readonly string? _key;

        public AIBroker(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public AIBroker(string endpoint, string key)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException($"'{nameof(endpoint)}' cannot be null or empty.", nameof(endpoint));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            _endpoint = endpoint;
            _key = key;
        }


        public OpenAIClient GetClient()
        {

            if (_configuration != null)
            {

                var endpoint = _configuration.GetValue<string>("OpenAI:Endpoint");

                if (string.IsNullOrEmpty(endpoint))
                {
                    throw new ArgumentNullException(nameof(endpoint));
                }

                var key = _configuration.GetValue<string?>("OpenAI:Key");

                return GetClient(endpoint, key);
            }

            return GetClient(_endpoint!, _key);
        }

        public OpenAIClient GetClient(string endpoint, string? key)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            if (!string.IsNullOrEmpty(key))
            {
                return new OpenAIClient(
                  new Uri(endpoint),
                  new AzureKeyCredential(key));
            }

            return new OpenAIClient(
                  new Uri(endpoint),
                  new DefaultAzureCredential());
        }

    }
}
