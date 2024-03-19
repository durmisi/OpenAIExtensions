using Microsoft.Extensions.Configuration;
using OpenAIExtensions.Services;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AITranslationServiceTests : IntegrationTestBase
{
    private readonly AITranslationService _translationService;

    public AITranslationServiceTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<AITranslationService>();

        var endpoint = Configuration.GetValue<string>("OpenAI:TranslationService:Endpoint")!;
        var key = Configuration.GetValue<string>("OpenAI:TranslationService:Key")!;

        var kernel = SematicKernelBuilder.Create()
            .AddAIChatCompletion(endpoint: endpoint, apiKey: key)
            .Build();

        _translationService = new AITranslationService(kernel, logger);
    }

    [Fact]
    public async Task AITranslationService_Translate_tes()
    {
        //Act
        var translationResult = await _translationService.TranslateAsync(
            "Нешто на Македонски",
            "Macedonian",
            "English");

        //Assert
        Assert.NotNull(translationResult);
        Assert.NotEmpty(translationResult);
        Assert.Equal("Something in Macedonian", translationResult);

        WriteToConsole(translationResult);
    }

    [Fact]
    public async Task AITranslationService_Translate_test_to_Many_languages()
    {
        //Act
        var translationResult = await _translationService.TranslateToManyAsync(
            "Нешто на Македонски",
            "Macedonian",
            ["English", "Italian"]);

        //Assert
        Assert.NotNull(translationResult);

        foreach (var key in translationResult.Keys)
        {
            Assert.NotEmpty(translationResult[key]);
            WriteToConsole(translationResult[key]);
        }
    }
}