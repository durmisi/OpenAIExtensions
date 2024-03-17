using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAIExtensions.Chats;
using Testcontainers.MsSql;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class KernelMemoryTests : IntegrationTestBase, IAsyncLifetime
{
    private readonly ILogger<AIConversationManager> _logger;

    private readonly ITestOutputHelper _outputHelper;

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
    private Kernel _kernel;
    private IKernelMemory _kernelMemory;

    public KernelMemoryTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        _logger = CreateLogger<AIConversationManager>();

        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task AskAsync_return_valid_result()
    {
        //Arrange
        var documentId = await _kernelMemory.ImportTextAsync("Моменталната температура во центарот на градот е 4 степени.");

        //Act
        var answer = await _kernelMemory.AskAsync("Колкава е моменталната температура во центарот на градот?");

        //Assert
        Assert.NotEmpty(documentId);
        Assert.NotNull(answer);
        Assert.NotEmpty(answer.Result);
        Assert.Equal("Моменталната температура во центарот на градот е 4 степени.", answer.Result);

        _outputHelper.WriteLine(answer.Result);
    }

    [Fact]
    public async Task AIConversationManager_ask_memory_return_valid_result()
    {
        //Arrange
        var aiConversationManager = new AIConversationManager(
            _kernel,
            _logger);

        var temp = new Random().Next(-10, 28);

        _ = await _kernelMemory.ImportTextAsync($"Моменталната температура во центарот на градот е {temp} степени.");

        var systemMessage = @"
            You are an AI bot that only knows how to answer user questions based on your memory and return the answers.
            Always respond with text.
        ";

        //Act
        var question = "Колкава е моменталната температура во центарот на градот?";

        var history = new ChatHistory();

        var prompt = $@"
           Question to Kernel Memory: {question}

           Kernel Memory Answer: {{memory.ask}}

           If the answer is empty say 'I don't know', otherwise reply with the answer.
           ";

        history.AddMessage(AuthorRole.User, prompt);

        var response = await aiConversationManager.ProcessConversationAsync(history, systemMessage);

        //Asert
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Contains(temp.ToString(), response);

        _outputHelper.WriteLine(response);
    }

    public async Task InitializeAsync()
    {

        await _msSqlContainer.StartAsync();
        var connectionString = _msSqlContainer.GetConnectionString();
        var schema = "ai" + new Random().Next(1, 100);

        var endpoint = Configuration.GetValue<string>("OpenAI:Endpoint")!;
        var apiKey = Configuration.GetValue<string>("OpenAI:Key")!;

        _kernel = SematicKernelBuilder.Create()
            .AddAIChatCompletion(endpoint: endpoint, apiKey: apiKey)
            .AddAITextEmbeddingGeneration(endpoint: endpoint, apiKey: apiKey)
            .Build();

        _kernelMemory = await _kernel.AddSqlServerKernelMemory(
            endpoint,
            apiKey,
            connectionString,
            schema);

    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync().AsTask();
    }
}