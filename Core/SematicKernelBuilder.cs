using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using OpenAIExtensions.HttpClients;

namespace OpenAIExtensions
{
    public class SematicKernelBuilder
    {
        private readonly IKernelBuilder _kernelBuilder;

        public string? DefaultEndpoint { get; private set; }
        public string? DefaultKey { get; private set; }

        private SematicKernelBuilder()
        {
            _kernelBuilder = Kernel.CreateBuilder();

            _kernelBuilder.Services.AddHttpClient<IRestApiClient, RestApiClient>();
        }

        public static SematicKernelBuilder Create(string defaultEndpoint, string defaultKey)
        {
            if (string.IsNullOrEmpty(defaultEndpoint))
            {
                throw new ArgumentException($"'{nameof(defaultEndpoint)}' cannot be null or empty.", nameof(defaultEndpoint));
            }

            if (string.IsNullOrEmpty(defaultKey))
            {
                throw new ArgumentException($"'{nameof(defaultKey)}' cannot be null or empty.", nameof(defaultKey));
            }


            return new SematicKernelBuilder()
            {
                DefaultEndpoint = defaultEndpoint,
                DefaultKey = defaultKey,
            };
        }

        public static SematicKernelBuilder Create()
        {
            return new SematicKernelBuilder();
        }

        public SematicKernelBuilder AddAIChatCompletion(
            string? endpoint = null,
            string? apiKey = null,
            string deploymentName = "gpt-35-turbo-0613")
        {
            _kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: GetEndpoint(endpoint),
                apiKey: GetApiKey(apiKey));
            return this;
        }

        public SematicKernelBuilder AddAITextGeneration(
            string? endpoint = null,
            string? apiKey = null,
            string deploymentName = "gpt-35-turbo-0613")
        {
#pragma warning disable SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _kernelBuilder.AddAzureOpenAITextGeneration(
               deploymentName: deploymentName,
               endpoint: GetEndpoint(endpoint),
               apiKey: GetApiKey(apiKey));
#pragma warning restore SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            return this;
        }

        public SematicKernelBuilder AddAITextEmbeddingGeneration(
            string? endpoint = null,
            string? apiKey = null,
            string deploymentName = "text-embedding-ada-002")
        {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
               deploymentName: deploymentName,
               endpoint: GetEndpoint(endpoint),
               apiKey: GetApiKey(apiKey));
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            return this;
        }

        public SematicKernelBuilder AddAIAudioToText(
            string? endpoint = null,
            string? apiKey = null,
            string deploymentName = "whisper-001")
        {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _kernelBuilder.AddAzureOpenAIAudioToText(
               deploymentName: deploymentName,
               endpoint: GetEndpoint(endpoint),
               apiKey: GetApiKey(apiKey));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            return this;
        }

        public SematicKernelBuilder AddAITextToAudio(
    string? endpoint = null,
    string? apiKey = null,
    string deploymentName = "gpt-35-turbo-0613")
        {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _kernelBuilder.AddAzureOpenAITextToAudio(
               deploymentName: deploymentName,
               endpoint: GetEndpoint(endpoint),
               apiKey: GetApiKey(apiKey));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            return this;
        }

        public SematicKernelBuilder AddAIFiles(string? apiKey = null)
        {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _kernelBuilder.AddOpenAIFiles(apiKey: GetApiKey(apiKey));
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            return this;
        }


        private string GetApiKey(string? apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                return apiKey;
            }

            return DefaultKey
                ?? throw new ArgumentNullException("Please provide a valid api key.");
        }

        private string GetEndpoint(string? endpoint)
        {
            if (!string.IsNullOrEmpty(endpoint))
            {
                return endpoint;
            }

            return DefaultEndpoint
                ?? throw new ArgumentNullException("Please provide a valid endpoint.");
        }

        public Kernel Build()
        {
            return _kernelBuilder.Build();
        }


        public SematicKernelBuilder AddPlugin<TPlugin>(string? pluginName = null)
            where TPlugin : class
        {
            _kernelBuilder.Plugins.AddFromType<TPlugin>(pluginName);
            return this;
        }
    }
}
