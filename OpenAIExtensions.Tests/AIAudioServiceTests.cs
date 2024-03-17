using Microsoft.Extensions.Configuration;
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

        _audioService = new AIAudioService(new AIBroker(endpoint, key), logger);
    }

    [Fact]
    public async Task AIAudioService_Transcribe_mp4_files()
    {
        //Act
        var response = await _audioService.TranscribeAsync("Content/18-13-52.m4a");

        //Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response.Language);
        Assert.NotEmpty(response.Text);
    }

    [Fact]
    public async Task AIAudioService_Translate_mp4_files()
    {
        //Act
        var audioTranslation = await _audioService.TranslateAsync("Content/18-13-52.m4a");

        //Assert
        Assert.NotNull(audioTranslation?.Language);
        Assert.NotNull(audioTranslation?.Text);
    }
}