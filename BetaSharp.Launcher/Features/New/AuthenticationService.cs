using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BetaSharp.Launcher.Features.New;

internal sealed class AuthenticationService(IHttpClientFactory httpClientFactory, LauncherService launcherService)
{
    private const string ID = "c36a9fb6-4f2a-41ff-90bd-ae7cc92031eb";
    private const string Redirect = "http://localhost:8080";
    private const string Scope = "XboxLive.signin offline_access";

    public async Task<bool> OwnsMinecraftAsync()
    {
        var microsoft = await RequestAccessTokenAsync();
        var xbox = await RequestXboxLiveTokenAsync(microsoft);
        var token = await RequestXstsTokenAsync(xbox.Token);
        var minecraft = await AuthenticateMinecraftAsync(token, xbox.Hash);

        var client = httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {minecraft}");

        var response = await client.GetAsync("https://api.minecraftservices.com/entitlements/mcstore");

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();

        var node = await JsonNode.ParseAsync(stream);
        var items = node?["items"]?.AsArray() ?? [];

        return items.Any(item => item?["name"]?.GetValue<string>() is "game_minecraft" or "product_minecraft");
    }

    private async Task<string> RequestAccessTokenAsync()
    {
        var state = Guid.NewGuid().ToString();

        using var listener = new HttpListener();

        listener.Prefixes.Add($"{Redirect}/");
        listener.Start();

        var url = $"https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize"
                  + $"?client_id={Uri.EscapeDataString(ID)}"
                  + $"&redirect_uri={Uri.EscapeDataString(Redirect)}"
                  + $"&scope={Uri.EscapeDataString(Scope)}"
                  + $"&state={Uri.EscapeDataString(state)}"
                  + $"&response_type=code";

        await launcherService.LaunchAsync(url);

        var context = await listener.GetContextAsync();

        if (context.Request.QueryString["state"] != state)
        {
            throw new InvalidOperationException("Context's state did not match the request's state.");
        }

        listener.Stop();

        return await ExchangeCodeAsync(context.Request.QueryString["code"]);
    }

    private async Task<(string Token, string Hash)> RequestXboxLiveTokenAsync(string accessToken)
    {
        var client = httpClientFactory.CreateClient();

        var xblRequest = new
        {
            Properties = new
            {
                AuthMethod = "RPS",
                SiteName = "user.auth.xboxlive.com",
                RpsTicket = $"d={accessToken}"
            },
            RelyingParty = "http://auth.xboxlive.com",
            TokenType = "JWT"
        };

        var response = await client.PostAsync("https://user.auth.xboxlive.com/user/authenticate", xblRequest);

        await using var stream = await response.Content.ReadAsStreamAsync();

        var node = await JsonNode.ParseAsync(stream);
        var token = node?["Token"]?.GetValue<string>();

        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hash = node?["DisplayClaims"]?["xui"]?[0]?["uhs"]?.GetValue<string>();

        ArgumentException.ThrowIfNullOrWhiteSpace(hash);

        return (token, hash);
    }

    private async Task<string> RequestXstsTokenAsync(string xblToken)
    {
        var client = httpClientFactory.CreateClient();

        var xstsRequest = new
        {
            Properties = new
            {
                SandboxId = "RETAIL",
                UserTokens = new[] { xblToken }
            },
            RelyingParty = "rp://api.minecraftservices.com/",
            TokenType = "JWT"
        };

        var response = await client.PostAsync("https://xsts.auth.xboxlive.com/xsts/authorize", xstsRequest);

        response.EnsureSuccessStatusCode();

        return await response.Content.GetValueAsync("Token");
    }

    private async Task<string> AuthenticateMinecraftAsync(string token, string hash)
    {
        var request = new
        {
            identityToken = $"XBL3.0 x={hash};{token}"
        };

        var client = httpClientFactory.CreateClient();
        var response = await client.PostAsync("https://api.minecraftservices.com/authentication/login_with_xbox", request);

        response.EnsureSuccessStatusCode();

        return await response.Content.GetValueAsync("access_token");
    }

    private async Task<string> ExchangeCodeAsync(string? code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var client = httpClientFactory.CreateClient();

        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", ID),
            new KeyValuePair<string, string>("scope", Scope),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", $"{Redirect}"),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        ]);

        var response = await client.PostAsync("https://login.microsoftonline.com/consumers/oauth2/v2.0/token", content);

        return await response.Content.GetValueAsync("access_token");
    }
}

internal static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, string uri, T instance) where T : class
    {
        var content = new StringContent(JsonSerializer.Serialize(instance), Encoding.UTF8, "application/json");
        return await client.PostAsync(uri, content);
    }
}

internal static class HttpContentExtensions
{
    public static async Task<string> GetValueAsync(this HttpContent content, string key)
    {
        await using var stream = await content.ReadAsStreamAsync();

        var node = await JsonNode.ParseAsync(stream);
        var value = node?[key]?.GetValue<string>();

        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return value;
    }
}