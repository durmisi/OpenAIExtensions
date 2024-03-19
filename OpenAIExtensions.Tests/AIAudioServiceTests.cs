using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenAIExtensions.Services;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AIAudioServiceTests : IntegrationTestBase
{
    private readonly AIAudioService _audioService;

    public AIAudioServiceTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<AIAudioService>();

        var endpoint = Configuration.GetValue<string>("OpenAI:AudioService:Endpoint")!;
        var key = Configuration.GetValue<string>("OpenAI:AudioService:Key")!;

        var kernel = Kernel.CreateBuilder()
          .AddAzureOpenAIAudioToText(
              deploymentName: "whisper-001",
              endpoint: endpoint,
              apiKey: key)
          .Build();

        _audioService = new AIAudioService(kernel, logger);
    }

    [Fact]
    public async Task AIAudioService_AudioToText_works_for_mp4_files()
    {
        //Act
        var response = await _audioService.AudioToTextAsync("Content/18-13-52.m4a");

        //Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response);

        WriteToConsole(response);
    }
}