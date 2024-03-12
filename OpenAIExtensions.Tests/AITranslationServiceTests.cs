﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAIExtensions.Services;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AITranslationServiceTests
{
    private readonly AITranslationService _translationService;
    private readonly ITestOutputHelper _outputHelper;

    public AITranslationServiceTests(ITestOutputHelper outputHelper)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .Build();

        var logger = LoggerFactory
                .Create(x => x.AddConsole())
                .CreateLogger<AITranslationService>();

        var endpoint = configuration.GetValue<string>("OpenAI:TranslationService:Endpoint")!;
        var key = configuration.GetValue<string>("OpenAI:TranslationService:Key")!;


        _translationService = new AITranslationService(new AIBroker(endpoint, key), logger);
        this._outputHelper = outputHelper;
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

        _outputHelper.WriteLine(translationResult);
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
            _outputHelper.WriteLine(translationResult[key]);
        }
    }

}