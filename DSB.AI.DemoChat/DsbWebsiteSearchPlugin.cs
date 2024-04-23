using Microsoft.SemanticKernel;
using OpenAIExtensions.Tools;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.ComponentModel;
using System.Text.Json;

namespace OpenAIExtensions.Plugins.WebSearch
{
    public class DsbWebsiteSearchPlugin : WebSearchPlugin
    {
        [KernelFunction, Description("Search dsb.dk, dsb website search")]
        public async Task<string> Search(
            Kernel kernel,
            [Description("The query to search for in dsb.dk website. Ex: what is dsb plus?")] string query
        )
        {
            var searchUrl = @$"https://www.dsb.dk/en/sogning/#?cludoquery={query.Trim()}&cludopage=1&cludoinputtype=standard";

            var browser = SearchWithChrome(searchUrl);

            var searchResults = browser.FindElements(By.XPath("//li[contains(@class,'search-results-item')]"));

            var results = new List<KeyValuePair<string, string>>();
            foreach (IWebElement node in searchResults)
            {
                var link = node.FindElements(By.TagName("a")).FirstOrDefault();

                if (link == null) continue;

                var href = link.GetAttribute("href");

                results.Add(new KeyValuePair<string, string>(node.Text, href.ToString()));
            }

            var result = JsonSerializer.Serialize(results);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Search result:" + result);
            Console.ForegroundColor = ConsoleColor.White;

            return result;
        }

        private static ChromeDriver SearchWithChrome(string searchUrl)
        {
            var options = new ChromeOptions()
            {
                BinaryLocation = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"
            };

            options.AddArguments(new List<string>() { "headless", "disable-gpu" });

            options.AddArguments("--ignore-certificate-errors");
            options.AddArguments("--allow-running-insecure-content");
            options.AddArgument("--disable-extensions");

            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            var browser = new ChromeDriver(service, options);
            browser.Navigate().GoToUrl(searchUrl);

            return browser;
        }
    }

}