using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetaSharp.Client.Rendering.Core.Textures;

public class SkinManager : IDisposable
{
    private readonly ILogger _logger = Log.Instance.For<SkinManager>();
    private readonly TextureManager _textureManager;
    private readonly HttpClient _httpClient;

    private readonly ConcurrentDictionary<string, Image<Rgba32>> _downloadedImages = new();
    private readonly ConcurrentDictionary<string, TextureHandle> _textureHandles = new();
    private readonly ConcurrentDictionary<string, bool> _downloading = new();

    public SkinManager(TextureManager textureManager)
    {
        _textureManager = textureManager;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
    }

    public void RequestDownload(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        if (_textureHandles.ContainsKey(url) || _downloadedImages.ContainsKey(url)) return;
        if (!_downloading.TryAdd(url, true)) return;

        Task.Run(async () =>
        {
            try
            {
                _logger.LogInformation("Downloading skin from {Url}", url);
                byte[] data = await _httpClient.GetByteArrayAsync(url);
                var image = Image.Load<Rgba32>(data);

                if (image.Height == 64 && image.Width == 64)
                {
                    image.Mutate(ctx => ctx.Crop(64, 32));
                }

                _downloadedImages[url] = image;
                _logger.LogInformation("Skin downloaded successfully from {Url}: ({W}x{H})", url, image.Width, image.Height);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to download skin from {Url}", url);
            }
            finally
            {
                _downloading.TryRemove(url, out _);
            }
        });
    }

    public TextureHandle? GetTextureHandle(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;

        if (_textureHandles.TryGetValue(url, out TextureHandle? handle)) return handle;

        if (_downloadedImages.TryRemove(url, out Image<Rgba32>? image))
        {
            handle = _textureManager.Load(image);
            _textureHandles[url] = handle;
            _logger.LogInformation("Skin texture created for {Url}", url);
            return handle;
        }

        return null;
    }

    public void Release(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        if (_downloadedImages.TryRemove(url, out Image<Rgba32>? image))
        {
            image.Dispose();
        }

        if (_textureHandles.TryRemove(url, out TextureHandle? handle))
        {
            _textureManager.Delete(handle);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _httpClient.Dispose();

        foreach (Image<Rgba32> image in _downloadedImages.Values)
        {
            image.Dispose();
        }
        _downloadedImages.Clear();

        foreach (TextureHandle handle in _textureHandles.Values)
        {
            _textureManager.Delete(handle);
        }
        _textureHandles.Clear();
    }
}
