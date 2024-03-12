using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace OpenAIExtensions.Text2Sql
{
    public interface IAISqlGenerator
    {
        ValueTask<string> TranslateToSqlQueryAsync(
            string naturalQuery,
            ContextInformation context);
    }

    public class AISqlGenerator : IAISqlGenerator
    {

        private readonly IChatCompletionService _chatCompletionService;

        private readonly ILogger<AISqlGenerator> _logger;

        private readonly string _deploymentName = "gpt-35-turbo-0613";

        public AISqlGenerator(
            string endpoint,
            string apiKey,
            ILogger<AISqlGenerator> logger,
            string? deploymentName = null)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException($"'{nameof(endpoint)}' cannot be null or empty.", nameof(endpoint));
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException($"'{nameof(apiKey)}' cannot be null or empty.", nameof(apiKey));
            }

            ArgumentNullException.ThrowIfNull(logger);

            _deploymentName = !string.IsNullOrEmpty(deploymentName) 
                ? deploymentName 
                : _deploymentName;

            var kernel = SematicKernelBuilder.Create()
                .AddAIChatCompletion(endpoint, apiKey, _deploymentName)
                .Build();

            _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            _logger = logger;
        }

        public AISqlGenerator(
            Kernel kernel,
            ILogger<AISqlGenerator> logger)
        {
            ArgumentNullException.ThrowIfNull(kernel);
            ArgumentNullException.ThrowIfNull(logger);

            _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            _logger = logger;
        }


        public async ValueTask<string> TranslateToSqlQueryAsync(
            string naturalQuery,
            ContextInformation context)
        {
            try
            {

                ValidateQuery(naturalQuery);
                ValidateContext(context);

                string contextDescription = GetContextDescription(context);


                string naturalQueryInput = @$"
                    Given a SQL db with the following tables:
                    {contextDescription} Translate the following request into SQL query: {naturalQuery}.";

                return await PromptQueryAsync(naturalQueryInput, @$"
                    You are an AI assistant that helping people to translate natural queries to SQL.
                    Always use only the tables and columns specified in the SQL db {contextDescription}.
                    Always select all columns, use database schemas and aliases when possible.
                    Respond ONLY with code.
                    ");
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void ValidateQuery(string naturalQuery)
        {
            if (string.IsNullOrWhiteSpace(naturalQuery))
            {
                throw new ArgumentException("Please provide a valid query");
            }

        }

        private static void ValidateContext(ContextInformation context)
        {
            if (context is null || context.Tables is null || context.Tables.Count == 0)
            {
                throw new ArgumentException("Please provide a valid context.");
            }


            foreach (var table in context.Tables)
            {
                if (string.IsNullOrEmpty(table.Name))
                {
                    throw new ArgumentException($"Please provide a valid context. Cannot find table information.");
                }

                if (table.Columns == null || !table.Columns.Any())
                {
                    throw new ArgumentException($"Please provide a valid context. Cannot find column information for table with name {table.Name}.");
                }

                foreach (var column in table.Columns)
                {
                    if (string.IsNullOrWhiteSpace(column.Name) || string.IsNullOrWhiteSpace(column.Type))
                    {
                        throw new ArgumentException($"Please provide a valid context. Cannot find valid column information for table with name {table.Name}.");
                    }
                }
            }
        }

        private static string GetContextDescription(ContextInformation context)
        {
            var contextDescription = string.Empty;

            foreach (var table in context.Tables)
            {

                if (!string.IsNullOrEmpty(table.Schema))
                {
                    contextDescription += $"Entity with Name = {table.Name} and Schema = {table.Schema} has the following properties:";
                }
                else
                {
                    contextDescription += $"Entity with Name = {table.Name} and Schema = dbo has the following properties:";
                }


                foreach (var column in table.Columns)
                {
                    contextDescription += $" {column.Name} as {column.Type}";
                }
            }

            return contextDescription;
        }

        private async ValueTask<string> PromptQueryAsync(string message, string? assistantMessage = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("Please provide a valid message.");
            }

            try
            {
                var chatHistory = new ChatHistory();

                if (!string.IsNullOrEmpty(assistantMessage))
                {
                    chatHistory.AddSystemMessage(assistantMessage);
                    chatHistory.AddUserMessage(message);
                }
                else
                {
                    chatHistory.AddUserMessage(message);
                }

                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    Temperature = 0
                };

                var result = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, openAIPromptExecutionSettings);

                return result.Content!;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
