using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;

namespace BetaSharp.Launcher.Features.Home;

internal sealed class SkinService(IHttpClientFactory clientFactory)
{
    public async Task<CroppedBitmap> GetFaceAsync(string url)
    {
        var client = clientFactory.CreateClient(nameof(SkinService));

        var response = await client.GetAsync(url);

        await using var stream = await response.Content.ReadAsStreamAsync();

        return new CroppedBitmap(new Bitmap(stream), new PixelRect(8, 8, 8, 8));
    }
}
