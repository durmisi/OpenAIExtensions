using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAIExtensions.Services;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AIEmbeddingsGeneratorTests
{
    private readonly AIEmbeddingsGenerator _embeddingsGenerator;
    private readonly ITestOutputHelper _outputHelper;

    public AIEmbeddingsGeneratorTests(ITestOutputHelper outputHelper)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .Build();

        var logger = LoggerFactory
                .Create(x => x.AddConsole())
                .CreateLogger<AIEmbeddingsGenerator>();

        var endpoint = configuration.GetValue<string>("OpenAI:Endpoint")!;
        var key = configuration.GetValue<string>("OpenAI:Key")!;

        _embeddingsGenerator = new AIEmbeddingsGenerator(endpoint, key, logger);
        _outputHelper = outputHelper;
    }


    [Fact]
    public async Task AIEmbeddingsGenerator_Create_embeddings_from_text()
    {
        //Act
        var embeddings = await _embeddingsGenerator.GetEmbeddingsAsync("Some content goes here...");

        //Assert
        Assert.NotNull(embeddings);
        Assert.NotEmpty(embeddings);

        _outputHelper.WriteLine($"Vector {string.Join(", ", embeddings!.First().Span.ToArray())}...");
    }

}