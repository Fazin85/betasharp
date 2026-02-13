using betareborn.Client.Resource.Pack;
using betareborn.Client.Textures;
using betareborn.Util;
using java.awt;
using java.awt.image;
using java.io;
using java.lang;
using java.nio;
using java.util;
using javax.imageio;
using Silk.NET.OpenGL.Legacy;
using static betareborn.Client.Textures.TextureAtlasMipmapGenerator;

namespace betareborn.Client.Rendering.Core
{
    public class TextureManager : java.lang.Object
    {
        private readonly HashMap textures = [];
        private readonly HashMap colors = [];
        private readonly HashMap images = [];
        private readonly IntBuffer idbuffer = GLAllocation.createDirectIntBuffer(1);
        private readonly ByteBuffer imageBuffer = GLAllocation.createDirectByteBuffer(1048576);
        private readonly List<DynamicTexture> dynamicTextures = [];
        private readonly Map downloadedImages = new HashMap();
        private readonly GameOptions gameOptions;
        private bool clamp = false;
        private bool blur = false;
        private readonly TexturePacks texturePacks;
        private readonly BufferedImage missingTextureImage = new(64, 64, 2);

        public TextureManager(TexturePacks var1, GameOptions var2)
        {
            texturePacks = var1;
            gameOptions = var2;
            Graphics var3 = missingTextureImage.getGraphics();
            var3.setColor(java.awt.Color.WHITE);
            var3.fillRect(0, 0, 64, 64);
            var3.setColor(java.awt.Color.BLACK);
            var3.drawString("missingtex", 1, 10);
            var3.dispose();
        }

        public int[] getColors(string var1)
        {
            TexturePack var2 = texturePacks.selectedTexturePack;
            int[] var3 = (int[])colors.get(var1);
            if (var3 != null)
            {
                return var3;
            }
            else
            {
                try
                {
                    object var6 = null;
                    if (var1.StartsWith("##"))
                    {
                        var3 = readColors(rescale(readImage(var2.getResourceAsStream(var1[2..]))));
                    }
                    else if (var1.StartsWith("%clamp%"))
                    {
                        clamp = true;
                        var3 = readColors(readImage(var2.getResourceAsStream(var1[7..])));
                        clamp = false;
                    }
                    else if (var1.StartsWith("%blur%"))
                    {
                        blur = true;
                        var3 = readColors(readImage(var2.getResourceAsStream(var1[6..])));
                        blur = false;
                    }
                    else
                    {
                        InputStream var7 = var2.getResourceAsStream(var1);
                        if (var7 == null)
                        {
                            var3 = readColors(missingTextureImage);
                        }
                        else
                        {
                            var3 = readColors(readImage(var7));
                        }
                    }

                    colors.put(var1, var3);
                    return var3;
                }
                catch (java.io.IOException var5)
                {
                    var5.printStackTrace();
                    int[] var4 = readColors(missingTextureImage);
                    colors.put(var1, var4);
                    return var4;
                }
            }
        }

        private int[] readColors(BufferedImage var1)
        {
            int var2 = var1.getWidth();
            int var3 = var1.getHeight();
            int[] var4 = new int[var2 * var3];
            var1.getRGB(0, 0, var2, var3, var4, 0, var2);
            return var4;
        }

        private int[] readColors(BufferedImage var1, int[] var2)
        {
            int var3 = var1.getWidth();
            int var4 = var1.getHeight();
            var1.getRGB(0, 0, var3, var4, var2, 0, var3);
            return var2;
        }

        public int getTextureId(string var1)
        {
            TexturePack var2 = texturePacks.selectedTexturePack;
            Integer var3 = (Integer)textures.get(var1);
            if (var3 != null)
            {
                return var3.intValue();
            }
            else
            {
                try
                {
                    idbuffer.clear();
                    GLAllocation.generateTextureNames(idbuffer);
                    int var6 = idbuffer.get(0);
                    if (var1.StartsWith("##"))
                    {
                        load(rescale(readImage(var2.getResourceAsStream(var1[2..]))), var6);
                    }
                    else if (var1.StartsWith("%clamp%"))
                    {
                        clamp = true;
                        load(readImage(var2.getResourceAsStream(var1[7..])), var6);
                        clamp = false;
                    }
                    else if (var1.StartsWith("%blur%"))
                    {
                        blur = true;
                        load(readImage(var2.getResourceAsStream(var1[6..])), var6);
                        blur = false;
                    }
                    else
                    {
                        InputStream var7 = var2.getResourceAsStream(var1);
                        if (var7 == null)
                        {
                            load(missingTextureImage, var6);
                        }
                        else
                        {
                            load(readImage(var7), var6, var1.Contains("terrain.png"));
                        }
                    }

                    textures.put(var1, Integer.valueOf(var6));
                    return var6;
                }
                catch (java.io.IOException var5)
                {
                    var5.printStackTrace();
                    GLAllocation.generateTextureNames(idbuffer);
                    int var4 = idbuffer.get(0);
                    load(missingTextureImage, var4);
                    textures.put(var1, Integer.valueOf(var4));
                    return var4;
                }
            }
        }

        private BufferedImage rescale(BufferedImage var1)
        {
            int var2 = var1.getWidth() / 16;
            BufferedImage var3 = new(16, var1.getHeight() * var2, 2);
            Graphics var4 = var3.getGraphics();

            for (int var5 = 0; var5 < var2; ++var5)
            {
                var4.drawImage(var1, -var5 * 16, var5 * var1.getHeight(), null);
            }

            var4.dispose();
            return var3;
        }

        public int load(BufferedImage var1)
        {
            idbuffer.clear();
            GLAllocation.generateTextureNames(idbuffer);
            int var2 = idbuffer.get(0);
            load(var1, var2);
            images.put(Integer.valueOf(var2), var1);
            return var2;
        }

        public unsafe void load(BufferedImage var1, int var2)
        {
            load(var1, var2, false);
        }

        public unsafe void load(BufferedImage var1, int var2, bool isTerrain)
        {
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var2);

            if (isTerrain)
            {
                int tileSize = var1.getWidth() / 16;
                TextureAtlas[] mips = GenerateMipmaps(bufferedImageToTextureAtlas(var1), tileSize);

                int mipCount = gameOptions.useMipmaps ? mips.Length : 1;

                for (int mipLevel = 0; mipLevel < mipCount; mipLevel++)
                {
                    TextureAtlas mip = mips[mipLevel];
                    byte[] pixelData = ToByteArray(mip.Pixels);

                    fixed (byte* ptr = pixelData)
                    {
                        GLManager.GL.TexImage2D(
                            TextureTarget.Texture2D,
                            mipLevel,
                            InternalFormat.Rgba8,
                            (uint)mip.Width,
                            (uint)mip.Height,
                            0,
                            PixelFormat.Rgba,
                            PixelType.UnsignedByte,
                            ptr
                        );
                    }
                }

                if (gameOptions.useMipmaps)
                {
                    GLManager.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                                     (int)TextureMinFilter.NearestMipmapNearest);
                }
                else
                {
                    GLManager.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                                     (int)TextureMinFilter.Nearest);
                }

                GLManager.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                                 (int)TextureMagFilter.Nearest);
                GLManager.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel,
                                 mipCount - 1);

                if (GLManager.GL.IsExtensionPresent("GL_EXT_texture_filter_anisotropic"))
                {
                    float aniso = gameOptions.anisotropicLevel == 0 ? 1.0f : (float)System.Math.Pow(2, gameOptions.anisotropicLevel);
                    aniso = System.Math.Clamp(aniso, 1.0f, GameOptions.MaxAnisotropy);

                    GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMaxAnisotropy, aniso);
                }
                return;
            }

            GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
            GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);

            if (blur)
            {
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
            }

            if (clamp)
            {
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.Clamp);
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.Clamp);
            }
            else
            {
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.Repeat);
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.Repeat);
            }

            int var3 = var1.getWidth();
            int var4 = var1.getHeight();
            int[] var5 = new int[var3 * var4];
            byte[] var6 = new byte[var3 * var4 * 4];
            var1.getRGB(0, 0, var3, var4, var5, 0, var3);

            int var7;
            int var8;
            int var9;
            int var10;
            int var11;
            for (var7 = 0; var7 < var5.Length; ++var7)
            {
                var8 = var5[var7] >> 24 & 255;
                var9 = var5[var7] >> 16 & 255;
                var10 = var5[var7] >> 8 & 255;
                var11 = var5[var7] & 255;

                var6[var7 * 4 + 0] = (byte)var9;
                var6[var7 * 4 + 1] = (byte)var10;
                var6[var7 * 4 + 2] = (byte)var11;
                var6[var7 * 4 + 3] = (byte)var8;
            }

            imageBuffer.clear();
            imageBuffer.put(var6);
            imageBuffer.position(0).limit(var6.Length);

            BufferHelper.UsePointer(imageBuffer, (p) =>
            {
                var ptr = (byte*)p;
                GLManager.GL.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba, (uint)var3, (uint)var4, 0, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
            });
        }

        public unsafe void bind(int[] var1, int var2, int var3, int var4)
        {
            //TODO: this is probably wrong and will crash

            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var4);

            GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
            GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);

            if (blur)
            {
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
            }
            if (clamp)
            {
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.Clamp);
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.Clamp);
            }
            else
            {
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.Repeat);
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.Repeat);
            }

            byte[] var5 = new byte[var2 * var3 * 4];

            for (int var6 = 0; var6 < var1.Length; ++var6)
            {
                int var7 = var1[var6] >> 24 & 255;
                int var8 = var1[var6] >> 16 & 255;
                int var9 = var1[var6] >> 8 & 255;
                int var10 = var1[var6] & 255;

                var5[var6 * 4 + 0] = (byte)var8;
                var5[var6 * 4 + 1] = (byte)var9;
                var5[var6 * 4 + 2] = (byte)var10;
                var5[var6 * 4 + 3] = (byte)var7;
            }

            imageBuffer.clear();
            imageBuffer.put(var5);
            imageBuffer.position(0).limit(var5.Length);

            byte[] imageArray = imageBuffer.array();
            int offset = imageBuffer.arrayOffset();

            fixed (byte* ptr = imageArray)
            {
                GLManager.GL.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, (uint)var2, (uint)var3, GLEnum.Rgba, GLEnum.UnsignedByte, ptr + offset);
            }
        }

        public void delete(int var1)
        {
            images.remove(Integer.valueOf(var1));
            idbuffer.clear();
            idbuffer.put(var1);
            idbuffer.flip();
            //GL11.glDeleteTextures(singleIntBuffer);
            GLManager.GL.DeleteTexture((uint)var1);
        }

        //public int getTextureForDownloadableImage(string var1, string var2)
        //{
        //    ThreadDownloadImageData var3 = (ThreadDownloadImageData)urlToImageDataMap.get(var1);
        //    if (var3 != null && var3.image != null && !var3.textureSetupComplete)
        //    {
        //        if (var3.textureName < 0)
        //        {
        //            var3.textureName = allocateAndSetupTexture(var3.image);
        //        }
        //        else
        //        {
        //            setupTexture(var3.image, var3.textureName);
        //        }

        //        var3.textureSetupComplete = true;
        //    }

        //    return var3 != null && var3.textureName >= 0 ? var3.textureName : (var2 == null ? -1 : getTexture(var2));
        //}

        //public ThreadDownloadImageData obtainImageData(string var1, ImageBuffer var2)
        //{
        //    ThreadDownloadImageData var3 = (ThreadDownloadImageData)urlToImageDataMap.get(var1);
        //    if (var3 == null)
        //    {
        //        urlToImageDataMap.put(var1, new ThreadDownloadImageData(var1, var2));
        //    }
        //    else
        //    {
        //        ++var3.referenceCount;
        //    }

        //    return var3;
        //}

        //public void releaseImageData(string var1)
        //{
        //    ThreadDownloadImageData var2 = (ThreadDownloadImageData)urlToImageDataMap.get(var1);
        //    if (var2 != null)
        //    {
        //        --var2.referenceCount;
        //        if (var2.referenceCount == 0)
        //        {
        //            if (var2.textureName >= 0)
        //            {
        //                deleteTexture(var2.textureName);
        //            }

        //            urlToImageDataMap.remove(var1);
        //        }
        //    }

        //}

        public void addDynamicTexture(DynamicTexture var1)
        {
            dynamicTextures.Add(var1);
            var1.tick();
        }

        public unsafe void tick()
        {
            int var1;
            DynamicTexture var2;
            int var3;
            int var4;

            for (var1 = 0; var1 < dynamicTextures.Count; ++var1)
            {
                var2 = dynamicTextures[var1];
                var2.tick();
                imageBuffer.clear();
                imageBuffer.put(var2.pixels);
                imageBuffer.position(0).limit(var2.pixels.Length);
                var2.bind(this);
                BufferHelper.UsePointer(imageBuffer, p =>
                {
                    var ptr = (byte*)p;
                    int fxPixelSize = (int)System.Math.Sqrt(var2.pixels.Length / 4);
                    for (var3 = 0; var3 < var2.replicate; ++var3)
                    {
                        for (var4 = 0; var4 < var2.replicate; ++var4)
                        {
                            GLManager.GL.TexSubImage2D(GLEnum.Texture2D, 0,
                                var2.sprite % 16 * fxPixelSize + var3 * fxPixelSize,
                                var2.sprite / 16 * fxPixelSize + var4 * fxPixelSize,
                                (uint)fxPixelSize, (uint)fxPixelSize, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
                        }
                    }
                });

                if (var2.atlas == DynamicTexture.FXImage.Terrain)
                {
                    UpdateTileMipmaps(var2.sprite, var2.pixels, var2.replicate);
                }
            }

            for (var1 = 0; var1 < dynamicTextures.Count; ++var1)
            {
                var2 = dynamicTextures[var1];
                if (var2.copyTo > 0)
                {
                    imageBuffer.clear();
                    imageBuffer.put(var2.pixels);
                    imageBuffer.position(0).limit(var2.pixels.Length);
                    GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var2.copyTo);
                    BufferHelper.UsePointer(imageBuffer, p =>
                    {
                        var ptr = (byte*)p;
                        int fxPixelSize = (int)System.Math.Sqrt(var2.pixels.Length / 4);
                        GLManager.GL.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, (uint)fxPixelSize, (uint)fxPixelSize, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
                    });
                }
            }
        }

        private unsafe void UpdateTileMipmaps(int tileIndex, byte[] tileData, int tileSize)
        {
            if (!gameOptions.useMipmaps)
            {
                return;
            }

            int totalPixels = tileData.Length / 4;
            int pixelSize = (int)System.Math.Sqrt(totalPixels);

            TextureAtlasMipmapGenerator.Color[] tilePixels = new TextureAtlasMipmapGenerator.Color[totalPixels];

            for (int i = 0; i < totalPixels; i++)
            {
                tilePixels[i] = new TextureAtlasMipmapGenerator.Color(
                    tileData[i * 4 + 0],
                    tileData[i * 4 + 1],
                    tileData[i * 4 + 2],
                    tileData[i * 4 + 3]
                );
            }

            TextureAtlasMipmapGenerator.Color[][] tileMipmaps = GenerateSingleTileMipmaps(tilePixels, pixelSize);

            for (int mipLevel = 1; mipLevel < tileMipmaps.Length; mipLevel++)
            {
                int mipTileSize = pixelSize >> mipLevel;
                int mipTileX = tileIndex % 16 * mipTileSize;
                int mipTileY = tileIndex / 16 * mipTileSize;

                Span<byte> mipData = new(new byte[tileMipmaps[mipLevel].Length * 4]);
                for (int i = 0; i < tileMipmaps[mipLevel].Length; i++)
                {
                    mipData[i * 4 + 0] = tileMipmaps[mipLevel][i].R;
                    mipData[i * 4 + 1] = tileMipmaps[mipLevel][i].G;
                    mipData[i * 4 + 2] = tileMipmaps[mipLevel][i].B;
                    mipData[i * 4 + 3] = tileMipmaps[mipLevel][i].A;
                }

                GLManager.GL.TexSubImage2D<byte>(GLEnum.Texture2D, mipLevel,
                    mipTileX, mipTileY,
                    (uint)mipTileSize, (uint)mipTileSize,
                    GLEnum.Rgba, GLEnum.UnsignedByte, mipData);
            }
        }

        private TextureAtlasMipmapGenerator.Color[][] GenerateSingleTileMipmaps(TextureAtlasMipmapGenerator.Color[] tile, int tileSize)
        {
            int maxMipLevels = (int)System.Math.Log2(tileSize) + 1;
            TextureAtlasMipmapGenerator.Color[][] mipmaps = new TextureAtlasMipmapGenerator.Color[maxMipLevels][];
            mipmaps[0] = tile;

            for (int mipLevel = 1; mipLevel < maxMipLevels; mipLevel++)
            {
                int currentSize = tileSize >> mipLevel - 1;
                int newSize = tileSize >> mipLevel;
                mipmaps[mipLevel] = DownsampleTile(mipmaps[mipLevel - 1], currentSize, newSize);
            }

            return mipmaps;
        }

        public void reload()
        {
            TexturePack var1 = texturePacks.selectedTexturePack;
            Iterator var2 = images.keySet().iterator();

            BufferedImage var4;
            while (var2.hasNext())
            {
                int var3 = ((Integer)var2.next()).intValue();
                var4 = (BufferedImage)images.get(Integer.valueOf(var3));
                load(var4, var3);
            }

            //ThreadDownloadImageData var8;
            //for (var2 = urlToImageDataMap.values().iterator(); var2.hasNext(); var8.textureSetupComplete = false)
            //{
            //    var8 = (ThreadDownloadImageData)var2.next();
            //}

            var2 = textures.keySet().iterator();

            string var9;
            while (var2.hasNext())
            {
                var9 = (string)var2.next();

                try
                {
                    if (var9.StartsWith("##"))
                    {
                        var4 = rescale(readImage(var1.getResourceAsStream(var9[2..])));
                    }
                    else if (var9.StartsWith("%clamp%"))
                    {
                        clamp = true;
                        var4 = readImage(var1.getResourceAsStream(var9[7..]));
                    }
                    else if (var9.StartsWith("%blur%"))
                    {
                        blur = true;
                        var4 = readImage(var1.getResourceAsStream(var9[6..]));
                    }
                    else
                    {
                        var4 = readImage(var1.getResourceAsStream(var9));
                    }

                    int var5 = ((Integer)textures.get(var9)).intValue();
                    load(var4, var5, var9.Contains("terrain.png"));
                    blur = false;
                    clamp = false;
                }
                catch (java.io.IOException var7)
                {
                    var7.printStackTrace();
                }
            }

            var2 = colors.keySet().iterator();

            while (var2.hasNext())
            {
                var9 = (string)var2.next();

                try
                {
                    if (var9.StartsWith("##"))
                    {
                        var4 = rescale(readImage(var1.getResourceAsStream(var9[2..])));
                    }
                    else if (var9.StartsWith("%clamp%"))
                    {
                        clamp = true;
                        var4 = readImage(var1.getResourceAsStream(var9[7..]));
                    }
                    else if (var9.StartsWith("%blur%"))
                    {
                        blur = true;
                        var4 = readImage(var1.getResourceAsStream(var9[6..]));
                    }
                    else
                    {
                        var4 = readImage(var1.getResourceAsStream(var9));
                    }

                    readColors(var4, (int[])colors.get(var9));
                    blur = false;
                    clamp = false;
                }
                catch (java.io.IOException var6)
                {
                    var6.printStackTrace();
                }
            }

        }

        private BufferedImage readImage(InputStream var1)
        {
            BufferedImage var2 = ImageIO.read(var1);
            var1.close();
            return var2;
        }

        private TextureAtlas bufferedImageToTextureAtlas(BufferedImage bufferedImage)
        {
            int width = bufferedImage.getWidth();
            int height = bufferedImage.getHeight();

            TextureAtlas atlas = new TextureAtlas(width, height);

            // Get the raw pixel data from BufferedImage
            int[] pixels = new int[width * height];
            bufferedImage.getRGB(0, 0, width, height, pixels, 0, width);

            // Convert from int ARGB to Color struct
            for (int i = 0; i < pixels.Length; i++)
            {
                int argb = pixels[i];

                // Extract ARGB components from the int
                byte a = (byte)(argb >> 24 & 0xFF);
                byte r = (byte)(argb >> 16 & 0xFF);
                byte g = (byte)(argb >> 8 & 0xFF);
                byte b = (byte)(argb & 0xFF);

                atlas.Pixels[i] = new TextureAtlasMipmapGenerator.Color(r, g, b, a);
            }

            return atlas;
        }

        public void bindTexture(int id)
        {
            if (id >= 0)
            {
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)id);
            }
        }
    }
}