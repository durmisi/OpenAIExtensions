namespace OpenAIExtensions.HttpClients
{
    public interface IRestApiClient
    {
        Task<T?> SendAsync<T>(string url, HttpMethod method, Dictionary<string, string>? headers = null)
            where T : class;
    }
}