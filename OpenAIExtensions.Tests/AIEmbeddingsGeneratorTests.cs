using Microsoft.Extensions.Configuration;
using OpenAIExtensions.Services;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class AIEmbeddingsGeneratorTests : IntegrationTestBase
{
    private readonly AIEmbeddingsGenerator _embeddingsGenerator;
    private readonly ITestOutputHelper _outputHelper;

    public AIEmbeddingsGeneratorTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        var logger = CreateLogger<AIEmbeddingsGenerator>();

        var endpoint = Configuration.GetValue<string>("OpenAI:Endpoint")!;
        var key = Configuration.GetValue<string>("OpenAI:Key")!;

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