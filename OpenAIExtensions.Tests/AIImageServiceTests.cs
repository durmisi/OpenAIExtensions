using Microsoft.Extensions.Configuration;
using OpenAIExtensions.Services;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AIImageServiceTests : IntegrationTestBase
{
    private readonly AIImageService _imageService;
    private readonly ITestOutputHelper _outputHelper;

    public AIImageServiceTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<AIImageService>();

        var endpoint = Configuration.GetValue<string>("OpenAI:ImageService:Endpoint")!;
        var key = Configuration.GetValue<string>("OpenAI:ImageService:Key")!;


        var kernel = SematicKernelBuilder.Create()
            .AddAIChatCompletion(endpoint: endpoint, apiKey: key, deploymentName: "gpt-4-vision-preview")
            .Build();

        _imageService = new AIImageService(kernel, logger);
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task AIImageService_Describes_images()
    {
        //Act
        var response = await _imageService.DescribeAsync("https://dynamic-media-cdn.tripadvisor.com/media/photo-o/0d/ac/47/0b/macedonia-stone-bridge.jpg");

        //Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Contains("Stone bridge".ToLower(), response.ToLower());

        WriteToConsole(response);
    }
}