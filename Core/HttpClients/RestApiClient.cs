using System.Text.Json;

namespace OpenAIExtensions.HttpClients
{
    public class RestApiClient : IRestApiClient
    {
        private readonly HttpClient _httpClient;

        public RestApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T?> SendAsync<T>(string url,
            HttpMethod method,
            Dictionary<string, string>? headers = null)
            where T : class
        {
            var request = new HttpRequestMessage(method, url);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            if (typeof(T) == typeof(string))
            {
                return content as T;
            }

            return JsonSerializer.Deserialize<T>(content);
        }
    }
}