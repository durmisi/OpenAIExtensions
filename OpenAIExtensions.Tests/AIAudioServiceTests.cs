using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAIExtensions.Services;

namespace OpenAIExtensions.Tests;

public class AIAudioServiceTests
{
    private readonly AIAudioService _audioService;

    public AIAudioServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .AddEnvironmentVariables(prefix: "ASPNETCORE")
            .Build();

        var logger = LoggerFactory
                .Create(x => x.AddConsole())
                .CreateLogger<AIAudioService>();


        var endpoint = configuration.GetValue<string>("OpenAI:AudioService:Endpoint")!;
        var key = configuration.GetValue<string>("OpenAI:AudioService:Key")!;

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