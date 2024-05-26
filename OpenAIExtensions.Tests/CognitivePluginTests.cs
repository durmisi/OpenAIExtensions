using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenAIExtensions.Plugins.Cognitive;
using OpenAIExtensions.Tests.Base;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class CognitivePluginTests : IntegrationTestBase
{
    private readonly Kernel _kernel;
    private readonly ITestOutputHelper _outputHelper;

    public CognitivePluginTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<CognitivePluginTests>();

        var endpoint = Configuration.GetValue<string>("OpenAI:ImageService:Endpoint")!;
        var key = Configuration.GetValue<string>("OpenAI:ImageService:Key")!;

        _kernel = Kernel.CreateBuilder()
            .AddAzureAIChatCompletion(endpoint: endpoint, apiKey: key, deploymentName: "gpt-4-vision-preview")
            .AddPlugin<CognitivePlugin>()
            .Build();

        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task AIImageService_Describes_images()
    {
        //Act
        var functionResult = await _kernel
            .InvokeAsync(nameof(CognitivePlugin), "Describe", new KernelArguments()
            {
                {"imageUrl",  "https://dynamic-media-cdn.tripadvisor.com/media/photo-o/0d/ac/47/0b/macedonia-stone-bridge.jpg"}
            });

        //Assert
        Assert.NotNull(functionResult);
        var response = functionResult.GetValue<string>();
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Contains("Stone bridge".ToLower(), response.ToLower());

        WriteToConsole(response);
    }
}