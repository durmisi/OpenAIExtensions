﻿using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAIExtensions.Chats;
using OpenAIExtensions.Plugins.Demo;
using OpenAIExtensions.Plugins.WebSearch;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AIConversationManagerTests : IntegrationTestBase
{
    private readonly AIConversationManager _aiConversationManager;
    private readonly ITestOutputHelper _outputHelper;

    public AIConversationManagerTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<AIConversationManager>();

        var endpoint = Configuration.GetValue<string>("OpenAI:Endpoint");
        var key = Configuration.GetValue<string>("OpenAI:Key");

        var kernel = SematicKernelBuilder.Create()
            .AddAIChatCompletion(endpoint: endpoint, apiKey: key)
            .AddCorePlugins()
            .AddPlugin<GetCurrentWeatherPlugin>()
            .AddPlugin<WikipediaPlugin>()
            .Build();

        _aiConversationManager = new AIConversationManager(
            kernel,
            logger);

        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task AIConversationManager_ProcessConversation_and_run_all_tools()
    {
        //Arrange

        var history = new ChatHistory();
        history.AddUserMessage("What`s the weather like today in Skopje, Berlin and Munich?");

        var systemMessage = @"
            You are an AI bot that only knows how to answer questions about weather or forward user questions to wikipedia and return the answers.
            Always respond with text.
        ";

        //Act
        var response = await _aiConversationManager.ProcessConversationAsync(history, systemMessage);

        //Asert
        Assert.NotNull(response);
        Assert.NotEmpty(response);

        _outputHelper.WriteLine(response);
    }

    [Fact]
    public async Task AIConversationManager_ProcessConversation_can_search_in_wikipedia()
    {
        //Arrange

        var history = new ChatHistory();

        var systemMessage = @"
            You are an AI bot that only knows how to answer questions about weather or forward user questions to wikipedia and return the answers.
            Always respond with text.
        ";

        history.AddUserMessage("Show some facts about North Macedonia, search in wikipedia");

        //Act
        var response = await _aiConversationManager.ProcessConversationAsync(history, systemMessage);

        //Asert
        Assert.NotNull(response);
        Assert.NotEmpty(response);

        _outputHelper.WriteLine(response);
    }
}