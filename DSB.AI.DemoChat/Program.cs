using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAIExtensions;
using OpenAIExtensions.Chats;
using OpenAIExtensions.Plugins.WebSearch;

var configuration = new ConfigurationBuilder()
         .AddJsonFile($"appsettings.json")
         .AddJsonFile($"appsettings.Development.json", optional: true)
         .AddEnvironmentVariables(prefix: "")
         .Build();

var loggerFactory = LoggerFactory
    .Create(x => x.AddConsole());

var logger = loggerFactory.CreateLogger<AIConversationManager>();

var endpoint = configuration.GetValue<string>("OpenAI:Endpoint");
var key = configuration.GetValue<string>("OpenAI:Key");

var kernel = SematicKernelBuilder.Create()
    .AddAIChatCompletion(endpoint: endpoint, apiKey: key)
    .AddCorePlugins()
    .AddPlugin<DsbWebsiteSearchPlugin>()
    .AddPlugin<DsbTrainSchedulePlugin>()
    .Build();

var aiConversationManager = new AIConversationManager(
    kernel,
    logger);


// Create the chat history
var systemMessage = """
You are an AI bot that only knows how to answer questions about DSB (Danske Statsbaner) 
and search dsb.dk website to find the answers.

Always respond with text.
""";

ChatHistory chatMessages = new();

chatMessages.AddSystemMessage("""
         Under no circumstances should you attempt to call functions / tools that are not available to you.
         Any functions / tools you do call must have the name satisfy the following regex: ^[a-zA-Z0-9_-]{1,64}$
        """);

while (true)
{
    // Get user input
    Console.Write("User > ");

    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
    {
        continue;
    }

    chatMessages.AddUserMessage(input);

    // Get the chat completions
    var fullMessage = await aiConversationManager
        .ProcessConversationAsync(chatMessages, systemMessage);

    if (!string.IsNullOrEmpty(fullMessage))
    {
        System.Console.Write("Assistant > ");
        System.Console.Write(fullMessage);
        System.Console.WriteLine();

        // Add the message from the agent to the chat history
        chatMessages.AddAssistantMessage(fullMessage);
    }
}