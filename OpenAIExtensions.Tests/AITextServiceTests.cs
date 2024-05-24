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


    [Fact]
    public async Task AITextService_ChangeVoiceToEmotional_generates_an_emotional_response()
    {
        //Arrange
        string productDescription = @"
Our new electric scooter is perfect for city commuters. 
With its lightweight design and long battery life, 
it's the ideal way to navigate busy streets and reach your destination quickly.";

        //Act
        var emotionallyEngagingDescription = await _translationService.ChangeVoiceToEmotional(productDescription);

        //Assert
        Assert.NotNull(emotionallyEngagingDescription);
        Assert.NotEmpty(emotionallyEngagingDescription);

        WriteToConsole(productDescription);
        WriteToConsole("------------------------------------------------");
        WriteToConsole(emotionallyEngagingDescription);
    }


    [Fact]
    public async Task AITextService_Summarize_generates_valid_response()
    {
        //Arrange
        string newsArticle = @"
            The United Nations said the situation in Yemen was deteriorating by the day,
            as it warned the war-ravaged country now risked sliding into famine directly
            attributable to the fact that fighting has moved north, obstructing the delivery
            of humanitarian aid. According to the UN, the situation is dire: 16.2 million
            people - more than half the population - are facing crisis levels of hunger and
            are in need of humanitarian aid, with 5 million people in danger of slipping
            into famine-like conditions, according to a recent Integrated Food Security
            Phase Classification (IPC) report, published on Friday. The war has been raging
            in Yemen since 2014 when Houthi rebels took over the capital Sanaa and much of
            the country's north, forcing the government to flee to the south, and later to
            Saudi Arabia, which has since led a coalition fighting the Iran-backed rebels.";

        //Act
        string summary = await _translationService.Summarize(newsArticle);

        //Assert
        Assert.NotNull(summary);
        Assert.NotEmpty(summary);

        WriteToConsole(newsArticle);
        WriteToConsole(newsArticle.Length.ToString());
        WriteToConsole("------------------------------------------------");
        WriteToConsole(summary);
        WriteToConsole(summary.Length.ToString());
    }

}