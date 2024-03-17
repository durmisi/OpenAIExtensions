using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAIExtensions.Chats;
using Testcontainers.MsSql;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests;

public class KernelMemoryTests : IntegrationTestBase, IAsyncLifetime
{
    private readonly ITestOutputHelper _outputHelper;

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    private string _connectionString;

    public KernelMemoryTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        _outputHelper = outputHelper;
    }

    //[Fact]
    //public async Task AskAsync_return_valid_result()
    //{
    //    //Arrange
    //    var memory = KernelMemoryFactory.Create(new CreateKernelMemoryRequest()
    //    {
    //        Endpoint = _endpoint,
    //        ApiKey = _apiKey,
    //        ConnectionString = _connectionString,
    //    });

    //    var documentId = await memory.ImportTextAsync("Моменталната температура во центарот на градот е 4 степени.");

    //    //Act
    //    var answer = await memory.AskAsync("Колкава е моменталната температура во центарот на градот?");

    //    //Assert
    //    Assert.NotEmpty(documentId);
    //    Assert.NotNull(answer);
    //    Assert.NotEmpty(answer.Result);
    //    Assert.Equal("Моменталната температура во центарот на градот е 4 степени.", answer.Result);

    //    _outputHelper.WriteLine(answer.Result);
    //}

    [Fact]
    public async Task AIConversationManager_ask_memory_return_valid_result()
    {
        //Arrange
        var logger = CreateLogger<AIConversationManager>();

        var endpoint = Configuration.GetValue<string>("OpenAI:Endpoint")!;
        var apiKey = Configuration.GetValue<string>("OpenAI:Key")!;

        var kernel = SematicKernelBuilder.Create()
            .AddAIChatCompletion(endpoint: endpoint, apiKey: apiKey)
            .AddAITextEmbeddingGeneration(endpoint: endpoint, apiKey: apiKey)
            .Build();

        var kernelMemory = KernelMemoryFactory.Create(new CreateKernelMemoryRequest()
        {
            Endpoint = endpoint,
            ApiKey = apiKey,
            ConnectionString = _connectionString,
            Schema = "ai2"
        });

        var plugin = new MemoryPlugin(kernelMemory, waitForIngestionToComplete: true);
        kernel.ImportPluginFromObject(plugin, "memory");

        kernel.ImportTextMemoryPlugin(_connectionString, "ai");

        var aiConversationManager = new AIConversationManager(
            kernel,
            logger);

        _ = await kernelMemory.ImportTextAsync("Моменталната температура во центарот на градот е 4 степени.");


        var systemMessage = @"
            You are an AI bot that only knows how to answer questions about weather and return the answers.
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

        _outputHelper.WriteLine(response);
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        _connectionString = _msSqlContainer.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync().AsTask();
    }
}