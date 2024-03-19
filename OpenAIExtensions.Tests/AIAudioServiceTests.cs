using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenAIExtensions.Services;
using System.Text;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AIAudioServiceTests : IntegrationTestBase
{
    private readonly AIAudioService _audioService;

    public AIAudioServiceTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<AIAudioService>();

        var kernel = Kernel.CreateBuilder()
          .AddAzureOpenAIAudioToText(
              deploymentName: "whisper-001",
              endpoint: Configuration.GetValue<string>("OpenAI:AudioService:Endpoint")!,
              apiKey: Configuration.GetValue<string>("OpenAI:AudioService:Key")!)
          .AddAzureOpenAITextToAudio(
              deploymentName: "tts-1",
              endpoint: Configuration.GetValue<string>("OpenAI:ImageService:Endpoint")!,
              apiKey: Configuration.GetValue<string>("OpenAI:ImageService:Key")!)
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

    [Fact]
    public async Task AIAudioService_TextToAudio_works_as_expected()
    {
        string sampleText = "Hello, my name is John. I am a software engineer. I am working on a project to convert text to audio.";

        //Act
        var response = await _audioService.TextToAudioAsync(sampleText);

        //Assert
        Assert.NotNull(response);
        
        //Save audio content to a file
        await File.WriteAllBytesAsync(Path.GetTempFileName(), response.Data!.Value.ToArray());

        WriteToConsole(Encoding.UTF8.GetString(response.Data!.Value.ToArray()));

    }
}