using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OpenAIExtensions.Managers;
using OpenAIExtensions;
using Microsoft.SemanticKernel.ChatCompletion;

var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json")
    .AddJsonFile($"appsettings.Development.json", optional: true)
    .Build();

var logger = LoggerFactory
        .Create(x => x.AddConsole())
        .CreateLogger<AIConversationManager>();

var endpoint = configuration.GetValue<string>("OpenAI:Endpoint")!;
var apiKey = configuration.GetValue<string>("OpenAI:Key")!;

var kernel = Kernel.CreateBuilder()
    .AddAzureAIChatCompletion(endpoint: endpoint, apiKey: apiKey)
.Build();


var kernelMemory = await kernel.AddSimpleVectorDb(
endpoint,
apiKey);


var documentId = "bitcoin.pdf";
await kernelMemory.ImportDocumentAsync("bitcoin.pdf", documentId);

var isReady = false;
while (!isReady)
{
    isReady = await kernelMemory.IsDocumentReadyAsync(documentId);
    await Task.Delay(100);
}


var skPrompt = """
Question to Memory: {{$input}}

Answer from Memory: {{memory.ask $input}}

If the answer is empty say 'I don't know' otherwise reply with an answer. 
""";

var myFunction = kernel.CreateFunctionFromPrompt(skPrompt);

var aiConversationManager = new AIConversationManager(
    kernel,
    logger);

var history = new ChatHistory();

Console.WriteLine("This assistant is here to help you with answering questions from you bitcoin.pdf document.");

while (true)
{
    Console.Write(">");

    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
    {
        continue;
    }

    if (input.Contains("quit"))
    {
        Environment.Exit(0);
    }

    var answer = await myFunction.InvokeAsync(kernel, input);
    Console.WriteLine("Assistant>" + answer);

}