//using Azure.AI.OpenAI;
//using Microsoft.Extensions.Configuration;
//using OpenAIExtensions.Chats;

//namespace OpenAIExtensions.Tools
//{


//    public class ChatWithYourDataTool : IChatExtension
//    {
//        private readonly IConfiguration _configuration;

//        public ChatWithYourDataTool(
//           IConfiguration configuration
//            )
//        {

//            _configuration = configuration;
//        }

//        public string Name => "search_hotels_database";

//        public bool IsActive
//        {
//            get
//            {
//                /// <summary>
//                /// https://github.com/Azure/azure-sdk-for-net/issues/41138
//                /// </summary>
//                return false;
//            }
//        }

//        public AzureChatExtensionConfiguration GetChatExtensionConfiguration()
//        {
//            var searchEndpoint = _configuration.GetValue<string>("OpenAI:SearchService:Endpoint");


//            if (string.IsNullOrEmpty(searchEndpoint))
//            {

//                throw new InvalidOperationException("Invalid configuration. Please check and try again.");

//            }

//            var indexName = _configuration.GetValue<string>("OpenAI:SearchService:IndexName")!;
//            var apiKey = _configuration.GetValue<string>("OpenAI:SearchService:ApiKey")!;

//            return new AzureCognitiveSearchChatExtensionConfiguration()
//            {
//                SearchEndpoint = new Uri(searchEndpoint),
//                Authentication = new OnYourDataApiKeyAuthenticationOptions(apiKey),
//                IndexName = indexName
//            };
//        }

//    }
//}
