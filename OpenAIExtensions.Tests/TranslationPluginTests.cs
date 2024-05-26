using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenAIExtensions.Plugins.Translation;
using OpenAIExtensions.Tests.Base;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class TranslationPluginTests : IntegrationTestBase
{
    private Kernel _kernel;

    public TranslationPluginTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<TranslationPluginTests>();

        var endpoint = Configuration.GetValue<string>("OpenAI:TranslationService:Endpoint")!;
        var key = Configuration.GetValue<string>("OpenAI:TranslationService:Key")!;

        _kernel = Kernel.CreateBuilder()
            .AddAzureAIChatCompletion(endpoint: endpoint, apiKey: key)
            .AddPlugin<TranslationPlugin>()
            .Build();
    }

    [Fact]
    public async Task AITranslationService_Translate_tes()
    {
        //Act
        var functionResult = await _kernel.InvokeAsync(nameof(TranslationPlugin), "Translate",  new KernelArguments()
        {
            {"text","Нешто на Македонски" },
            {"fromLanguage","Macedonian" },
            {"toLanguage","English" },
        });

        //Assert
        Assert.NotNull(functionResult);
        var response = functionResult.GetValue<string>();
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Equal("Something in Macedonian", response);

        WriteToConsole(response);
    }

    [Fact]
    public async Task AITranslationService_Translate_test_to_Many_languages()
    {
        //Act
        var functionResult = await _kernel.InvokeAsync(nameof(TranslationPlugin), "TranslateToMany", new KernelArguments()
        {
            {"text","Нешто на Македонски" },
            {"fromLanguage","Macedonian" },
            {"toLanguages", new List<string>(){"English", "Italian" }.ToArray() },
        });

        //Assert
        Assert.NotNull(functionResult);
        var translationResult = functionResult.GetValue<Dictionary<string, string>>();
        foreach (var key in translationResult.Keys)
        {
            Assert.NotEmpty(translationResult[key]);
            WriteToConsole(translationResult[key]);
        }
    }
}