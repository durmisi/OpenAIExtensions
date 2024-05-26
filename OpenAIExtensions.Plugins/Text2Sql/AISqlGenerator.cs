using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;

namespace OpenAIExtensions.Plugins.Text2Sql
{

    public class Text2SqlPlugin
    {

        [KernelFunction, Description("Translates a native query to SQL code.")]

        public async ValueTask<string> TranslateToSqlQueryAsync(
            Kernel kernel,
         [Description("The native query to be translated.")] string naturalQuery,
         [Description("Information about the context in which the translation is requested.")]   ContextInformation context)
        {
            try
            {
                ValidateQuery(naturalQuery);
                ValidateContext(context);

                string contextDescription = GetContextDescription(context);

                string naturalQueryInput = @$"
                    Given a SQL db with the following tables:
                    {contextDescription} Translate the following request into SQL query: {naturalQuery}.";

                return await PromptQueryAsync(
                    kernel,
                    naturalQueryInput, @$"
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

        private async ValueTask<string> PromptQueryAsync(
            Kernel kernel,
            string message,
            string? assistantMessage = null)
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

                var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
                var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, openAIPromptExecutionSettings);

                return result.Content!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}