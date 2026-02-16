using System.Net.Http;
using System.Threading.Tasks;

namespace BetaSharp.Launcher.Features.Splash;

internal sealed class GitHubService(IHttpClientFactory httpClientFactory)
{
    public async Task GetUpdatesAsync()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://api.github.com/repos/Fazin85/betasharp/releases");
    }
}
