using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace OpenAIExtensions.Plugins.Demo
{
    public sealed class GetCurrentWeatherPlugin
    {
        [KernelFunction, Description("Get the current weather in a given location")]
        public static int GetCurrentWeather(
            [Description("The city and state, e.g. San Francisco, CA")] string location,
            [Description("Temperature unit, by default is celsius, but it can be fahrenheit as well")] string? unit
        )
        {
            return new Random().Next(1, 27);
        }
    }
}