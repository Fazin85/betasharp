using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace BetaSharp.Launcher;

internal static class HttpClientExtensions
{
    public static async Task<T?> GetAsync<T>(this HttpClient client, string url, JsonTypeInfo<T> typeInfo)
    {
        var response = await client.GetAsync(url);

        await using var stream = await response.Content.ReadAsStreamAsync();

        var instance = JsonSerializer.Deserialize(stream, typeInfo);

        return instance;
    }

    public static async Task<TResponse?> PostAsync<TResponse, TRequest>(
        this HttpClient client,
        string url,
        TRequest request,
        JsonTypeInfo<TRequest> requestInfo,
        JsonTypeInfo<TResponse> responseInfo)
    {
        var response = await client.PostAsync(url, JsonContent.Create(request, requestInfo));

        await using var stream = await response.Content.ReadAsStreamAsync();

        var instance = JsonSerializer.Deserialize(stream, responseInfo);

        return instance;
    }
}
