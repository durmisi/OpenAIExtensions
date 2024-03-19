using HtmlAgilityPack;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using System.Text.RegularExpressions;

namespace OpenAIExtensions.Tools
{
    public abstract partial class WebSearchPlugin
    {
        protected async Task<string?> GetHtml(Kernel kernel, string url)
        {
            var html = await kernel.InvokeAsync<string>(pluginName: nameof(HttpPlugin),
                functionName: "Get",
                new KernelArguments()
                {
                    {"uri", url }
                });

            return html;
        }

        protected static string GetPlainTextFromHtml(string htmlString)
        {
            string htmlTagPattern = "<.*?>";

            htmlString = RegexCss().Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = RegexNewLine().Replace(htmlString, "");
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            return htmlString;
        }

        [GeneratedRegex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
        private static partial Regex RegexCss();

        protected static HtmlDocument LoadHtml(string html)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(html);

            return doc;
        }

        [GeneratedRegex(@"^\s+$[\r\n]*", RegexOptions.Multiline)]
        private static partial Regex RegexNewLine();
    }
}