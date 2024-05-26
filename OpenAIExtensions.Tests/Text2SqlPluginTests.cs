using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenAIExtensions.Plugins.Text2Sql;
using OpenAIExtensions.Tests.Base;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests
{
    public class Text2SqlPluginTests : IntegrationTestBase
    {
        private readonly Kernel _kernel;
        private readonly ITestOutputHelper _outputHelper;

        public Text2SqlPluginTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            var logger = CreateLogger<Text2SqlPluginTests>();

            var endpoint = Configuration.GetValue<string>("OpenAI:Endpoint")!;
            var key = Configuration.GetValue<string?>("OpenAI:Key")!;

             _kernel = Kernel.CreateBuilder()
                .AddAzureAIChatCompletion(endpoint, key)
                .AddPlugin<Text2SqlPlugin>()
                .Build();


            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task Text2Sql_Generates_valid_sql()
        {
            //Arrange
            var context = GetContext();

            //Act
            string naturalQuery = "Find all Customers where Name starts with Adm, ignore case";
            var functionResult = await _kernel.InvokeAsync(nameof(Text2SqlPlugin), "TranslateToSqlQuery", new KernelArguments()
            {
                {"naturalQuery", naturalQuery},
                {"context", context},
            });


            //Assert
            Assert.NotNull(functionResult);

            var sqlQuery = functionResult.GetValue<string>();

            _outputHelper.WriteLine(naturalQuery);
            _outputHelper.WriteLine(sqlQuery);

            Assert.Equal("SELECT * FROM dbo.Customers WHERE LOWER(Name) LIKE 'adm%'", sqlQuery);
        }

        private static ContextInformation GetContext()
        {
            return new ContextInformation()
            {
                Tables = [
                   new()
                   {
                       Name = "Customers",
                       Schema = "dbo",
                       Columns =
                       [
                            new() {
                                Name = "Id",
                                Type = "int"
                            },
                            new() {
                                Name = "Name",
                                Type = "string"
                            },
                            new() {
                                Name = "Email",
                                Type = "string"
                            }
                       ],
                   },
                    new()
                    {
                        Name = "Orders",
                        Schema = "dbo",
                        Columns =
                        [
                            new() {
                                Name = "OrderNumber",
                                Type = "string"
                            },
                            new() {
                                Name = "CustomerId",
                                Type = "int"
                            }
                        ],
                    }]
            };
        }
    }
}