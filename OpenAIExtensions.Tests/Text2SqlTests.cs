using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAIExtensions.Text2Sql;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests
{
    public class Text2SqlTests
    {
        private readonly AISqlGenerator _aiProcessingService;
        private readonly ITestOutputHelper _outputHelper;

        public Text2SqlTests(ITestOutputHelper outputHelper)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json")
                .Build();

            var aiBroker = new AIBroker(configuration);

            var logger = LoggerFactory
                .Create(x => x.AddConsole())
                .CreateLogger<AISqlGenerator>();

            var endpoint = configuration.GetValue<string>("OpenAI:Endpoint");
            var key = configuration.GetValue<string?>("OpenAI:Key");

            _aiProcessingService = new AISqlGenerator(endpoint, key, logger);

            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task Text2Sql_Generates_valid_sql()
        {
            //Arrange
            var context = GetContext();

            //Act
            string naturalQuery = "Find all Customers where Name starts with Adm, ignore case";
            string sqlQuery = await _aiProcessingService.TranslateToSqlQueryAsync(naturalQuery, context);

            //Assert
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