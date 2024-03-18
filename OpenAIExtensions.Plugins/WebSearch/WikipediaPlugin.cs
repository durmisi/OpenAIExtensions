using Microsoft.SemanticKernel;
using OpenAIExtensions.Tools;
using System.ComponentModel;
using System.Text;

namespace OpenAIExtensions.Plugins.WebSearch
{
    public class WikipediaPlugin : WebSearchPlugin
    {
        private readonly int ContentLenght = 300;

        [KernelFunction, Description("Search wikipedia, wiki search")]
        public async Task<string> Search(
            Kernel kernel,
            [Description("The query to search for in wikipedia")] string query
        )
        {
            var html = await GetHtml(kernel, $"https://en.wikipedia.org/wiki/{query.Trim()}");
            var doc = LoadHtml(html);

            var sb = new StringBuilder();
            foreach (var node in doc.DocumentNode.SelectNodes("//div[@id=\"mw-content-text\"]"))
            {
                sb.AppendLine(node.InnerHtml);
            }
            string plainText = GetPlainTextFromHtml(sb.ToString());

            return plainText.Substring(0, ContentLenght);
        }
    }
}