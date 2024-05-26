using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenAIExtensions.Plugins.Audio;
using OpenAIExtensions.Tests.Base;
using System.Text;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AudioPluginTests : IntegrationTestBase
{
    private Kernel _kernel;

    public AudioPluginTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<AudioPluginTests>();

        _kernel = Kernel.CreateBuilder()
            .AddPlugin<AudioPlugin>()
          .AddAzureOpenAIAudioToText(
              deploymentName: "whisper-001",
              endpoint: Configuration.GetValue<string>("OpenAI:AudioService:Endpoint")!,
              apiKey: Configuration.GetValue<string>("OpenAI:AudioService:Key")!)
          .AddAzureOpenAITextToAudio(
              deploymentName: "tts-1",
              endpoint: Configuration.GetValue<string>("OpenAI:ImageService:Endpoint")!,
              apiKey: Configuration.GetValue<string>("OpenAI:ImageService:Key")!)
          .Build();

    }

    [Fact]
    public async Task AIAudioService_AudioToText_works_for_mp4_files()
    {
        //Act
        var funtionResult = await _kernel.InvokeAsync(nameof(AudioPlugin), "AudioFileToText",
             new KernelArguments()
             {
                 {
                     "path", "Content/18-13-52.m4a"
                 }
        });

        //Assert
        Assert.NotNull(funtionResult);

        var response = funtionResult.GetValue<string>();

        Assert.NotNull(response);
        Assert.NotEmpty(response);

        WriteToConsole(response);
    }

    [Fact]
    public async Task AIAudioService_TextToAudio_works_as_expected()
    {
        string sampleText = "Hello, my name is John. I am a software engineer. I am working on a project to convert text to audio.";

        //Act
        var funtionResult = await _kernel.InvokeAsync(nameof(AudioPlugin),
            "TextToAudio",
          new KernelArguments()
          {
                 {
                     "text", sampleText
                 }
        });

        //Assert
        Assert.NotNull(funtionResult);
        var response = funtionResult.GetValue<AudioContent>();
        Assert.NotNull(response);

        //Save audio content to a file
        await File.WriteAllBytesAsync(Path.GetTempFileName(), response.Data!.Value.ToArray());

        WriteToConsole(Encoding.UTF8.GetString(response.Data!.Value.ToArray()));

    }
}