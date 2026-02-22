using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace BetaSharp.Launcher.Features;

internal static class HttpClientExtensions
{
    public static async Task<TResponse> PostAsync<TRequest, TResponse>(
        this HttpClient client,
        string uri,
        TRequest instance,
        JsonTypeInfo<TRequest> requestType,
        JsonTypeInfo<TResponse> responseType)
    {
        var request = await client.PostAsync(uri, new StringContent(
            JsonSerializer.Serialize(instance, requestType),
            Encoding.UTF8,
            "application/json"));

        var response = await request.Content.ReadFromJsonAsync(responseType);

        ArgumentNullException.ThrowIfNull(response);

        return response;
    }
}
