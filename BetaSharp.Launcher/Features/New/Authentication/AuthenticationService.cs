using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.New.Authentication.Extensions;

namespace BetaSharp.Launcher.Features.New.Authentication;

// More decoupling and overall cleaning.
internal sealed class AuthenticationService(IHttpClientFactory httpClientFactory, LauncherService launcherService, XboxService xboxService, MinecraftService minecraftService)
{
    public sealed class Session(string name, string token)
    {
        public string Name => name;

        public string Token => token;
    }

    private const string Id = "c36a9fb6-4f2a-41ff-90bd-ae7cc92031eb";
    private const string Scope = "XboxLive.signin offline_access";

    private readonly string _redirect = $"http://localhost:{Socket.GetAvailablePort()}";

    // Store this somewhere else, a file or something.
    private readonly byte[] _response = """
                                        <!DOCTYPE html><html lang="en"><head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>BetaSharp</title><style>body{margin:0;padding:0;background-color:#000;display:flex;justify-content:center;align-items:center;height:100vh;font-family:Arial,sans-serif}p{color:#fff;font-size:0.75rem;font-weight:400;text-align:center;opacity:.5}</style></head><body><p>You can close this tab now</p></body></html>
                                        """u8.ToArray();

    public async Task<Session> AuthenticateAsync()
    {
        string microsoft = await GetMicrosoftTokenAsync();
        var profile = await xboxService.GetProfileAsync(microsoft);
        string xbox = await xboxService.GetTokenAsync(profile.Token);
        string minecraft = await minecraftService.GetTokenAsync(xbox, profile.Hash);

        if (!await minecraftService.GetGameAsync(minecraft))
        {
            throw new InvalidOperationException("You must own a legitimate copy of Minecraft Java edition to use this client.");
        }

        string name = await minecraftService.GetNameAsync(minecraft);
        var session = new Session(name, minecraft);

        return session;
    }

    // This probably needs further refactoring.
    private async Task<string> GetMicrosoftTokenAsync()
    {
        string state = Guid.NewGuid().ToString();

        using var listener = new HttpListener();

        listener.Prefixes.Add($"{_redirect}/");
        listener.Start();

        string url = $"https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize"
                     + $"?client_id={Uri.EscapeDataString(Id)}"
                     + $"&redirect_uri={Uri.EscapeDataString(_redirect)}"
                     + $"&scope={Uri.EscapeDataString(Scope)}"
                     + $"&state={Uri.EscapeDataString(state)}"
                     + "&response_type=code"
                     + "&prompt=select_account";

        await launcherService.LaunchAsync(url);

        var context = await listener.GetContextAsync();
        string? code = context.Request.QueryString["code"];

        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (context.Request.QueryString["state"] != state)
        {
            throw new InvalidOperationException("Context's state did not match the request's state.");
        }

        context.Response.ContentLength64 = _response.Length;
        await context.Response.OutputStream.WriteAsync(_response);
        context.Response.Close();

        listener.Stop();

        return await ExchangeCodeAsync(code);
    }

    private async Task<string> ExchangeCodeAsync(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var client = httpClientFactory.CreateClient();

        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", Id),
            new KeyValuePair<string, string>("scope", Scope),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", _redirect),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        ]);

        var response = await client.PostAsync("https://login.microsoftonline.com/consumers/oauth2/v2.0/token", content);

        return await response.Content.GetValueAsync("access_token");
    }
}
