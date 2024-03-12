using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAIExtensions.Services;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AIImageServiceTests
{
    private readonly AIImageService _imageService;
    private readonly ITestOutputHelper _outputHelper;

    public AIImageServiceTests(ITestOutputHelper outputHelper)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .Build();

        var logger = LoggerFactory
                .Create(x => x.AddConsole())
                .CreateLogger<AIImageService>();


        var endpoint = configuration.GetValue<string>("OpenAI:ImageService:Endpoint")!;
        var key = configuration.GetValue<string>("OpenAI:ImageService:Key")!;

        _imageService = new AIImageService(new AIBroker(endpoint, key), logger);
        this._outputHelper = outputHelper;
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

        _outputHelper.WriteLine(response);
    }

}