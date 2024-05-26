using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

namespace OpenAIExtensions
{
    public static class KernelBuilderExtensions
    {
        public static IKernelBuilder AddAzureAIChatCompletion(
            this IKernelBuilder kernelBuilder,
            string endpoint,
            string apiKey,
            string deploymentName = "gpt-35-turbo-0613")
        {
            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endpoint,
                apiKey: apiKey);

            return kernelBuilder;
        }

        public static IKernelBuilder AddAzureAITextGeneration(
            this IKernelBuilder kernelBuilder,
            string endpoint,
            string apiKey,
            string deploymentName = "gpt-35-turbo-0613")
        {
            kernelBuilder.AddAzureOpenAITextGeneration(
               deploymentName: deploymentName,
               endpoint: endpoint,
               apiKey: apiKey);

            return kernelBuilder;
        }

        public static IKernelBuilder AddAITextEmbeddingGeneration(
            this IKernelBuilder kernelBuilder,
            string endpoint,
            string apiKey,
            string deploymentName = "text-embedding-ada-002")
        {
            kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
               deploymentName: deploymentName,
               endpoint: endpoint,
               apiKey: apiKey);

            return kernelBuilder;
        }

        public static IKernelBuilder AddAIAudioToText(
            this IKernelBuilder kernelBuilder,
            string endpoint,
            string apiKey,
            string deploymentName = "whisper-001")
        {
            kernelBuilder.AddAzureOpenAIAudioToText(
               deploymentName: deploymentName,
               endpoint: endpoint,
               apiKey: apiKey);
            return kernelBuilder;
        }

        public static IKernelBuilder AddAITextToAudio(
             this IKernelBuilder kernelBuilder,
             string endpoint,
             string apiKey,
             string deploymentName = "gpt-35-turbo-0613")
        {
            kernelBuilder.AddAzureOpenAITextToAudio(
               deploymentName: deploymentName,
               endpoint: endpoint,
               apiKey: apiKey);

            return kernelBuilder;
        }

        public static IKernelBuilder AddAIFiles(
            this IKernelBuilder kernelBuilder,
            string apiKey)
        {
            kernelBuilder.AddOpenAIFiles(apiKey: apiKey);
            return kernelBuilder;
        }

        public static IKernelBuilder AddPlugin<TPlugin>(
              this IKernelBuilder kernelBuilder,
              string? pluginName = null)
            where TPlugin : class
        {
            kernelBuilder.Plugins.AddFromType<TPlugin>(pluginName);
            return kernelBuilder;
        }

        public static IKernelBuilder AddConversationSummaryPlugin(
            this IKernelBuilder kernelBuilder
            )
        {
            kernelBuilder.Plugins.AddFromType<ConversationSummaryPlugin>();
            return kernelBuilder;
        }

        public static IKernelBuilder AddMathPlugin(
          this IKernelBuilder kernelBuilder
          )
        {
            kernelBuilder.Plugins.AddFromType<MathPlugin>();
            return kernelBuilder;
        }

        public static IKernelBuilder AddHttpPlugin(
        this IKernelBuilder kernelBuilder
        )
        {
            kernelBuilder.Plugins.AddFromType<HttpPlugin>();
            return kernelBuilder;
        }


        public static IKernelBuilder AddTimePlugin(
            this IKernelBuilder kernelBuilder
            )
        {
            kernelBuilder.Plugins.AddFromType<TimePlugin>();
            return kernelBuilder;
        }

        public static IKernelBuilder AddWaitPlugin(
            this IKernelBuilder kernelBuilder
            )
        {
            kernelBuilder.Plugins.AddFromType<WaitPlugin>();
            return kernelBuilder;
        }


        public static IKernelBuilder AddTextPlugin(
            this IKernelBuilder kernelBuilder
            )
        {
            kernelBuilder.Plugins.AddFromType<TextPlugin>();
            return kernelBuilder;
        }


        public static IKernelBuilder AddFileIOPlugin(
            this IKernelBuilder kernelBuilder
            )
        {
            kernelBuilder.Plugins.AddFromType<FileIOPlugin>();
            return kernelBuilder;
        }

    }
}