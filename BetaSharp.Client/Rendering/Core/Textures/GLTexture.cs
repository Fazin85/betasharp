using BetaSharp.Client.Rendering.Core.OpenGL;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Core.Textures;

public class GLTexture : IDisposable
{
    private static readonly ILogger s_logger = Log.Instance.For<GLTexture>();
    private static readonly Dictionary<uint, (string Source, DateTime CreatedAt)> s_activeTextures = [];

    public uint Id { get; private set; }
    public string Source { get; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public static int ActiveTextureCount => s_activeTextures.Count;

    // Sampler state — cached and applied when creating the texture
    private bool _filterNearest = true;
    private bool _wrapRepeat = true;
    private float _anisotropy = 1.0f;
    private int _maxMipLevel;

    public GLTexture(string source)
    {
        Source = source;
        Id = GLManager.GL.GenTexture();
        s_activeTextures.Add(Id, (source, DateTime.Now));
    }

    public void Bind()
    {
        if (Id != 0)
        {
            TextureStats.NotifyBind();
            GLManager.GL.BindTexture(GLEnum.Texture2D, Id);
        }
    }

    public void SetFilter(TextureMinFilter min, TextureMagFilter mag)
    {
        Bind();
        _filterNearest = mag == TextureMagFilter.Nearest;
        // Apply via IGL which is now EmulatedGL — this is a no-op there but we track it
        GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)min);
        GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)mag);
        // Recreate sampler with new filter if texture already exists
        UpdateSampler();
    }

    public void SetWrap(TextureWrapMode s, TextureWrapMode t)
    {
        Bind();
        _wrapRepeat = s == TextureWrapMode.Repeat;
        GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)s);
        GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)t);
        UpdateSampler();
    }

    public void SetMaxLevel(int level)
    {
        Bind();
        _maxMipLevel = level;
        GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMaxLevel, level);
    }

    public unsafe void Upload(int width, int height, byte* ptr, int level = 0, PixelFormat format = PixelFormat.Rgba, InternalFormat internalFormat = InternalFormat.Rgba)
    {
        if (level == 0)
        {
            Width = width;
            Height = height;
        }
        Bind();
        GLManager.GL.TexImage2D(TextureTarget.Texture2D, level, internalFormat, (uint)width, (uint)height, 0, format, PixelType.UnsignedByte, ptr);

        // After upload, ensure the sampler is created/updated
        if (level == 0) UpdateSampler();
    }

    public unsafe void UploadSubImage(int x, int y, int width, int height, byte* ptr, int level = 0, PixelFormat format = PixelFormat.Rgba)
    {
        Bind();
        GLManager.GL.TexSubImage2D(GLEnum.Texture2D, level, x, y, (uint)width, (uint)height, (GLEnum)format, (GLEnum)PixelType.UnsignedByte, ptr);
    }

    public void SetAnisotropicFilter(float level)
    {
        _anisotropy = level;
        UpdateSampler();
    }

    private void UpdateSampler()
    {
        if (Id == 0) return;

        var gl = GLManager.GL as EmulatedGL;
        if (gl == null) return;

        // Check if we have a texture registered for this ID
        if (!gl.TryGetTexture(Id, out var view, out _)) return;
        if (view == null) return;

        // Create new sampler with current filter/wrap state
        var addressMode = _wrapRepeat
            ? global::Vuldrid.SamplerAddressMode.Wrap
            : global::Vuldrid.SamplerAddressMode.Clamp;

        var filter = _filterNearest
            ? global::Vuldrid.SamplerFilter.MinPoint_MagPoint_MipPoint
            : global::Vuldrid.SamplerFilter.MinLinear_MagLinear_MipLinear;

        uint maxAniso = _anisotropy > 1.0f ? (uint)_anisotropy : 0;

        var sampler = GLManager.Factory.CreateSampler(new global::Vuldrid.SamplerDescription(
            addressMode, addressMode, addressMode,
            filter,
            null,
            maxAniso,
            0, (uint)_maxMipLevel, 0,
            global::Vuldrid.SamplerBorderColor.TransparentBlack));

        gl.RegisterTexture(Id, view, sampler);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (Id != 0)
        {
            GLManager.GL.DeleteTexture(Id);
            s_activeTextures.Remove(Id, out _);
            Id = 0;
        }
    }

    public static void LogLeakReport()
    {
        if (s_activeTextures.Count == 0) return;

        s_logger.LogWarning("Found {Count} leaked OpenGL textures on shutdown!", s_activeTextures.Count);
        foreach (KeyValuePair<uint, (string Source, DateTime CreatedAt)> entry in s_activeTextures)
        {
            s_logger.LogWarning("Leaked Texture ID: {Id}, Source: {Source}, Created At: {CreatedAt}", entry.Key, entry.Value.Source, entry.Value.CreatedAt);
        }
    }
}
