# About OpenAIExtensions
OpenAIExtensions is a cutting-edge library designed to enhance and streamline the integration of Azure OpenAI services into your applications. 
Leveraging the advanced capabilities of Azure OpenAI & developed on top of SematicKernel, my goal is to provide developers with a comprehensive toolkit that simplifies the process of building AI-powered solutions.


# Using OpenAIExtensions in Your Project

In this guide, i'll walk you through the process of integrating OpenAIExtensions into your project and showcase how to use it effectively.

## Installation and Setup

To get started, you'll need to install OpenAIExtensions via NuGet Package Manager or by cloning the repository from GitHub.

```bash
dotnet add package OpenAIExtensions
```

## AIConversationManager

Now, let's create an instance of the AIConversationManager class in your test or application code. 
This manager orchestrates the conversation between users and the AI system.

```bash
// Instantiate AIConversationManager with necessary dependencies
var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json")
    .Build();

var logger = LoggerFactory
        .Create(x => x.AddConsole())
        .CreateLogger<AIConversationManager>();

var endpoint = configuration.GetValue<string>("OpenAI:Endpoint")!;
var key = configuration.GetValue<string>("OpenAI:Key")!;

var kernel = SematicKernelBuilder.Create()
    .AddAIChatCompletion(endpoint: endpoint, apiKey: key)
    .AddPlugin<GetCurrentWeatherPlugin>()
    .Build();

var aiConversationManager = new AIConversationManager(
    kernel,
    logger);

var history = new ChatHistory();
history.AddUserMessage("What`s the weather like today in Skopje?");

var systemMessage = @"
    You are an AI bot that only knows how to answer questions about weather.
    Always respond with text.
";

var response = await aiConversationManager.ProcessConversationAsync(history, systemMessage);

response => The weather in Skopje today is partly cloudy with a high of 15°C.

```