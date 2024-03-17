using HtmlAgilityPack;
using OpenAIExtensions.HttpClients;
using System.Text.RegularExpressions;

namespace OpenAIExtensions.Tools
{
    public abstract partial class WebSearchPlugin
    {
        private readonly IRestApiClient _restApiClient;

        public WebSearchPlugin(IRestApiClient restApiClient)
        {
            _restApiClient = restApiClient;
        }

        protected async Task<string> GetHtml(string url)
        {
            var html = await _restApiClient.SendAsync<string>(url, HttpMethod.Get);
            return html;
        }

        protected static string GetPlainTextFromHtml(string htmlString)
        {
            string htmlTagPattern = "<.*?>";

            htmlString = RegexCss().Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            return htmlString;
        }

        [GeneratedRegex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
        private static partial Regex RegexCss();

        protected HtmlDocument LoadHtml(string html)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(html);

            return doc;
        }
    }
}