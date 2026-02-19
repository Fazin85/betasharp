using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace BetaSharp.Launcher;

public class MicrosoftAuthService
{
    private static readonly HttpClient _httpClient = new();

    //Prism Launcher client id
    private const string ClientId = "c36a9fb6-4f2a-41ff-90bd-ae7cc92031eb";
    private const string RedirectUri = "http://localhost:8080";

    public async Task<Session?> AuthenticateAsync()
    {
        try
        {
            var msToken = await GetMicrosoftTokenAuthCodeFlowAsync();
            if (msToken == null) return null;

            var xblToken = await AuthenticateXboxLiveAsync(msToken);
            if (xblToken == null) return null;

            var xstsData = await GetXSTSTokenAsync(xblToken);
            if (xstsData == null) return null;

            var mcToken = await AuthenticateMinecraftAsync(xstsData.Value.uhs, xstsData.Value.token);
            if (mcToken == null) return null;

            var ownsGame = await CheckGameOwnershipAsync(mcToken);
            if (!ownsGame)
            {
                throw new Exception("You don't own Minecraft Java Edition!");
            }

            var profile = await GetMinecraftProfileAsync(mcToken);
            if (profile == null) return null;

            return new Session
            {
                Username = profile.name,
                AccessToken = mcToken,
                Uuid = profile.id
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }

    private async Task<string?> GetMicrosoftTokenAuthCodeFlowAsync()
    {
        try
        {
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            var state = Guid.NewGuid().ToString("N");

            var authUrl = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize" +
                          $"?client_id={ClientId}" +
                          $"&response_type=code" +
                          $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                          $"&scope={Uri.EscapeDataString("XboxLive.signin offline_access")}" +
                          $"&state={state}" +
                          $"&code_challenge={codeChallenge}" +
                          $"&code_challenge_method=S256" +
                          "&prompt=select_account";


            // Start local HTTP listener
            using var listener = new HttpListener();
            listener.Prefixes.Add($"{RedirectUri}/");
            listener.Start();

            Process.Start(new ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });

            var context = await listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            var responseString = "<html><body><h1>Authentication successful!</h1><p>You can close this window now.</p></body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer);
            response.Close();

            var query = request.Url?.Query;
            if (string.IsNullOrEmpty(query))
            {
                throw new Exception("No query parameters received");
            }

            var queryParams = HttpUtility.ParseQueryString(query);
            var code = queryParams["code"];
            var returnedState = queryParams["state"];
            var error = queryParams["error"];

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Authentication error: {error}");
            }

            if (returnedState != state)
            {
                throw new Exception("State mismatch - possible CSRF attack");
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("No authorization code received");
            }

            var tokenRequest = new Dictionary<string, string>
            {
                ["client_id"] = ClientId,
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = RedirectUri,
                ["code_verifier"] = codeVerifier
            };

            var tokenResponse = await _httpClient.PostAsync(
                "https://login.microsoftonline.com/consumers/oauth2/v2.0/token",
                new FormUrlEncodedContent(tokenRequest));

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Token exchange failed: {tokenContent}");
            }

            var tokenJson = JsonDocument.Parse(tokenContent);
            return tokenJson.RootElement.GetProperty("access_token").GetString();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Base64UrlEncode(bytes);
    }

    private string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        return Base64UrlEncode(challengeBytes);
    }

    private string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    private static async Task<string?> AuthenticateXboxLiveAsync(string msToken)
    {
        try
        {
            var request = new
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = $"d={msToken}"
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            };

            var requestJson = JsonSerializer.Serialize(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://user.auth.xboxlive.com/user/authenticate");
            httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(httpRequest);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Xbox Live auth failed (Status: {response.StatusCode}): {content}");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("Xbox Live returned empty response");
            }

            var json = JsonDocument.Parse(content);
            return json.RootElement.GetProperty("Token").GetString();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task<(string token, string uhs)?> GetXSTSTokenAsync(string xblToken)
    {
        try
        {
            var request = new
            {
                Properties = new
                {
                    SandboxId = "RETAIL",
                    UserTokens = new[] { xblToken }
                },
                RelyingParty = "rp://api.minecraftservices.com/",
                TokenType = "JWT"
            };

            var requestJson = JsonSerializer.Serialize(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://xsts.auth.xboxlive.com/xsts/authorize");
            httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(httpRequest);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"XSTS auth failed (Status: {response.StatusCode}): {content}");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("XSTS returned empty response");
            }

            var json = JsonDocument.Parse(content);
            var token = json.RootElement.GetProperty("Token").GetString();
            var uhs = json.RootElement.GetProperty("DisplayClaims")
                .GetProperty("xui")[0]
                .GetProperty("uhs").GetString();

            return (token!, uhs!);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task<string?> AuthenticateMinecraftAsync(string uhs, string xstsToken)
    {
        try
        {
            var request = new
            {
                identityToken = $"XBL3.0 x={uhs};{xstsToken}"
            };

            var response = await _httpClient.PostAsJsonAsync(
                "https://api.minecraftservices.com/authentication/login_with_xbox", request);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Minecraft auth failed: {content}");
            }

            var json = JsonDocument.Parse(content);
            return json.RootElement.GetProperty("access_token").GetString();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task<bool> CheckGameOwnershipAsync(string mcToken)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {mcToken}");

        var response = await _httpClient.GetAsync(
            "https://api.minecraftservices.com/entitlements/mcstore");

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var items = json.GetProperty("items");

        foreach (var item in items.EnumerateArray())
        {
            var name = item.GetProperty("name").GetString();
            if (name == "product_minecraft" || name == "game_minecraft")
            {
                return true;
            }
        }

        return false;
    }

    private static async Task<MinecraftProfile?> GetMinecraftProfileAsync(string mcToken)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {mcToken}");

        var response = await _httpClient.GetAsync(
            "https://api.minecraftservices.com/minecraft/profile");

        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<MinecraftProfile>();
    }

    private class MinecraftProfile
    {
        public string id { get; set; } = "";
        public string name { get; set; } = "";
    }
}