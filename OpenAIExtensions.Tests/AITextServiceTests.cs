using Microsoft.Extensions.Configuration;
using OpenAIExtensions.Services;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AITextServiceTests : IntegrationTestBase
{
    private readonly AITextService _translationService;

    public AITextServiceTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<AITextService>();

        var endpoint = Configuration.GetValue<string>("OpenAI:Endpoint")!;
        var key = Configuration.GetValue<string>("OpenAI:Key")!;

        var kernel = SematicKernelBuilder.Create()
            .AddAIChatCompletion(endpoint: endpoint, apiKey: key)
            .Build();

        _translationService = new AITextService(kernel, logger);
    }

    [Fact]
    public async Task AITextService_Brainstorm_generates_an_idea()
    {
        //Arrange
        var userIdea = @"

            I have this concept for a mobile app that helps people track their daily water intake and reminds them to stay hydrated throughout the day. 
            Can you brainstorm various aspects of this idea, like potential challenges, opportunities for growth, innovative features, 
            and how to differentiate it from existing hydration apps? 
            I'm eager to explore this idea from different angles and get detailed suggestions.
            ";


        //Act
        var answer = await _translationService.Brainstorm(userIdea);

        //Assert
        Assert.NotNull(answer);
        Assert.NotEmpty(answer);

        WriteToConsole(userIdea);
        WriteToConsole(answer);
    }

}