using betareborn.Textures;
using java.awt;
using java.awt.image;
using java.io;
using java.lang;
using java.nio;
using java.util;
using javax.imageio;
using Silk.NET.OpenGL.Legacy;
using static betareborn.Textures.TextureAtlasMipmapGenerator;

namespace betareborn.Rendering
{
    public class RenderEngine : java.lang.Object
    {
        private readonly HashMap textureMap = [];
        private readonly HashMap field_28151_c = [];
        private readonly HashMap textureNameToImageMap = [];
        private readonly IntBuffer singleIntBuffer = GLAllocation.createDirectIntBuffer(1);
        private readonly ByteBuffer imageData = GLAllocation.createDirectByteBuffer(1048576);
        private readonly java.util.List textureList = new ArrayList();
        private readonly Map urlToImageDataMap = new HashMap();
        private readonly GameSettings options;
        private bool clampTexture = false;
        private bool blurTexture = false;
        private readonly TexturePackList texturePack;
        private readonly BufferedImage missingTextureImage = new(64, 64, 2);

        public RenderEngine(TexturePackList var1, GameSettings var2)
        {
            texturePack = var1;
            options = var2;
            Graphics var3 = missingTextureImage.getGraphics();
            var3.setColor(java.awt.Color.WHITE);
            var3.fillRect(0, 0, 64, 64);
            var3.setColor(java.awt.Color.BLACK);
            var3.drawString("missingtex", 1, 10);
            var3.dispose();
        }

        public int[] func_28149_a(string var1)
        {
            TexturePackBase var2 = texturePack.selectedTexturePack;
            int[] var3 = (int[])field_28151_c.get(var1);
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
                        var3 = func_28148_b(unwrapImageByColumns(readTextureImage(var2.getResourceAsStream(var1[2..]))));
                    }
                    else if (var1.StartsWith("%clamp%"))
                    {
                        clampTexture = true;
                        var3 = func_28148_b(readTextureImage(var2.getResourceAsStream(var1[7..])));
                        clampTexture = false;
                    }
                    else if (var1.StartsWith("%blur%"))
                    {
                        blurTexture = true;
                        var3 = func_28148_b(readTextureImage(var2.getResourceAsStream(var1[6..])));
                        blurTexture = false;
                    }
                    else
                    {
                        InputStream var7 = var2.getResourceAsStream(var1);
                        if (var7 == null)
                        {
                            var3 = func_28148_b(missingTextureImage);
                        }
                        else
                        {
                            var3 = func_28148_b(readTextureImage(var7));
                        }
                    }

                    field_28151_c.put(var1, var3);
                    return var3;
                }
                catch (java.io.IOException var5)
                {
                    var5.printStackTrace();
                    int[] var4 = func_28148_b(missingTextureImage);
                    field_28151_c.put(var1, var4);
                    return var4;
                }
            }
        }

        private int[] func_28148_b(BufferedImage var1)
        {
            int var2 = var1.getWidth();
            int var3 = var1.getHeight();
            int[] var4 = new int[var2 * var3];
            var1.getRGB(0, 0, var2, var3, var4, 0, var2);
            return var4;
        }

        private int[] func_28147_a(BufferedImage var1, int[] var2)
        {
            int var3 = var1.getWidth();
            int var4 = var1.getHeight();
            var1.getRGB(0, 0, var3, var4, var2, 0, var3);
            return var2;
        }

        public int getTexture(string var1)
        {
            TexturePackBase var2 = texturePack.selectedTexturePack;
            Integer var3 = (Integer)textureMap.get(var1);
            if (var3 != null)
            {
                return var3.intValue();
            }
            else
            {
                try
                {
                    singleIntBuffer.clear();
                    GLAllocation.generateTextureNames(singleIntBuffer);
                    int var6 = singleIntBuffer.get(0);
                    if (var1.StartsWith("##"))
                    {
                        setupTexture(unwrapImageByColumns(readTextureImage(var2.getResourceAsStream(var1[2..]))), var6);
                    }
                    else if (var1.StartsWith("%clamp%"))
                    {
                        clampTexture = true;
                        setupTexture(readTextureImage(var2.getResourceAsStream(var1[7..])), var6);
                        clampTexture = false;
                    }
                    else if (var1.StartsWith("%blur%"))
                    {
                        blurTexture = true;
                        setupTexture(readTextureImage(var2.getResourceAsStream(var1[6..])), var6);
                        blurTexture = false;
                    }
                    else
                    {
                        InputStream var7 = var2.getResourceAsStream(var1);
                        if (var7 == null)
                        {
                            setupTexture(missingTextureImage, var6);
                        }
                        else
                        {
                            if (var1.Contains("terrain.png"))
                            {
                                BufferedImage img = readTextureImage(var7);

                                TextureAtlas[] mips = GenerateMipmaps(bufferedImageToTextureAtlas(img), 16);

                                GLManager.GL.BindTexture(TextureTarget.Texture2D, (uint)var6);

                                for (int mipLevel = 0; mipLevel < mips.Length; mipLevel++)
                                {
                                    TextureAtlas mip = mips[mipLevel];
                                    byte[] pixelData = ToByteArray(mip.Pixels);

                                    unsafe
                                    {
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
                                }

                                GLManager.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                                                 (int)TextureMinFilter.NearestMipmapNearest);
                                GLManager.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                                                 (int)TextureMagFilter.Nearest);
                                GLManager.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel,
                                                 mips.Length - 1);
                            }
                            else
                            {
                                setupTexture(readTextureImage(var7), var6);
                            }
                        }
                    }

                    textureMap.put(var1, Integer.valueOf(var6));
                    return var6;
                }
                catch (java.io.IOException var5)
                {
                    var5.printStackTrace();
                    GLAllocation.generateTextureNames(singleIntBuffer);
                    int var4 = singleIntBuffer.get(0);
                    setupTexture(missingTextureImage, var4);
                    textureMap.put(var1, Integer.valueOf(var4));
                    return var4;
                }
            }
        }

        private BufferedImage unwrapImageByColumns(BufferedImage var1)
        {
            int var2 = var1.getWidth() / 16;
            BufferedImage var3 = new(16, var1.getHeight() * var2, 2);
            Graphics var4 = var3.getGraphics();

            for (int var5 = 0; var5 < var2; ++var5)
            {
                var4.drawImage(var1, -var5 * 16, var5 * var1.getHeight(), (ImageObserver)null);
            }

            var4.dispose();
            return var3;
        }

        public int allocateAndSetupTexture(BufferedImage var1)
        {
            singleIntBuffer.clear();
            GLAllocation.generateTextureNames(singleIntBuffer);
            int var2 = singleIntBuffer.get(0);
            setupTexture(var1, var2);
            textureNameToImageMap.put(Integer.valueOf(var2), var1);
            return var2;
        }

        public unsafe void setupTexture(BufferedImage var1, int var2)
        {
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var2);

            GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
            GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);

            if (blurTexture)
            {
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
            }

            if (clampTexture)
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
            int var12;
            int var13;
            int var14;
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

            imageData.clear();
            imageData.put(var6);
            imageData.position(0).limit(var6.Length);

            BufferHelper.UsePointer(imageData, (p) =>
            {
                var ptr = (byte*)p;
                GLManager.GL.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba, (uint)var3, (uint)var4, 0, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
            });
        }

        public unsafe void func_28150_a(int[] var1, int var2, int var3, int var4)
        {
            //TODO: this is probably wrong and will crash

            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var4);

            GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
            GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);

            if (blurTexture)
            {
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
                GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
            }
            if (clampTexture)
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

            imageData.clear();
            imageData.put(var5);
            imageData.position(0).limit(var5.Length);

            byte[] imageArray = imageData.array();
            int offset = imageData.arrayOffset();

            fixed (byte* ptr = imageArray)
            {
                GLManager.GL.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, (uint)var2, (uint)var3, GLEnum.Rgba, GLEnum.UnsignedByte, ptr + offset);
            }
        }

        public void deleteTexture(int var1)
        {
            textureNameToImageMap.remove(Integer.valueOf(var1));
            singleIntBuffer.clear();
            singleIntBuffer.put(var1);
            singleIntBuffer.flip();
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

        public void registerTextureFX(TextureFX var1)
        {
            textureList.add(var1);
            var1.onTick();
        }

        public unsafe void updateDynamicTextures()
        {
            int var1;
            TextureFX var2;
            int var3;
            int var4;

            for (var1 = 0; var1 < textureList.size(); ++var1)
            {
                var2 = (TextureFX)textureList.get(var1);
                var2.onTick();
                imageData.clear();
                imageData.put(var2.imageData);
                imageData.position(0).limit(var2.imageData.Length);
                var2.bindImage(this);
                BufferHelper.UsePointer(imageData, (p =>
                {
                    var ptr = (byte*)p;
                    for (var3 = 0; var3 < var2.tileSize; ++var3)
                    {
                        for (var4 = 0; var4 < var2.tileSize; ++var4)
                        {
                            GLManager.GL.TexSubImage2D(GLEnum.Texture2D, 0,
                                var2.iconIndex % 16 * 16 + var3 * 16,
                                var2.iconIndex / 16 * 16 + var4 * 16,
                                16, 16, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
                        }
                    }
                }));

                UpdateTileMipmaps(var2.iconIndex, var2.imageData, var2.tileSize);
            }

            for (var1 = 0; var1 < textureList.size(); ++var1)
            {
                var2 = (TextureFX)textureList.get(var1);
                if (var2.textureId > 0)
                {
                    imageData.clear();
                    imageData.put(var2.imageData);
                    imageData.position(0).limit(var2.imageData.Length);
                    GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var2.textureId);
                    BufferHelper.UsePointer(imageData, (p =>
                    {
                        var ptr = (byte*)p;
                        GLManager.GL.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, 16, 16, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
                    }));

                    UpdateSingleTextureMipmaps((uint)var2.textureId, var2.imageData);
                }
            }
        }

        private unsafe void UpdateTileMipmaps(int tileIndex, byte[] tileData, int tileSize)
        {
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
                int mipTileX = (tileIndex % 16) * mipTileSize;
                int mipTileY = (tileIndex / 16) * mipTileSize;

                byte[] mipData = new byte[tileMipmaps[mipLevel].Length * 4];
                for (int i = 0; i < tileMipmaps[mipLevel].Length; i++)
                {
                    mipData[i * 4 + 0] = tileMipmaps[mipLevel][i].R;
                    mipData[i * 4 + 1] = tileMipmaps[mipLevel][i].G;
                    mipData[i * 4 + 2] = tileMipmaps[mipLevel][i].B;
                    mipData[i * 4 + 3] = tileMipmaps[mipLevel][i].A;
                }

                fixed (byte* ptr = mipData)
                {
                    GLManager.GL.TexSubImage2D(GLEnum.Texture2D, mipLevel,
                        mipTileX, mipTileY,
                        (uint)mipTileSize, (uint)mipTileSize,
                        GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
                }
            }
        }

        private TextureAtlasMipmapGenerator.Color[][] GenerateSingleTileMipmaps(TextureAtlasMipmapGenerator.Color[] tile, int tileSize)
        {
            int maxMipLevels = (int)System.Math.Log2(tileSize) + 1;
            TextureAtlasMipmapGenerator.Color[][] mipmaps = new TextureAtlasMipmapGenerator.Color[maxMipLevels][];
            mipmaps[0] = tile;

            for (int mipLevel = 1; mipLevel < maxMipLevels; mipLevel++)
            {
                int currentSize = tileSize >> (mipLevel - 1);
                int newSize = tileSize >> mipLevel;
                mipmaps[mipLevel] = DownsampleTile(mipmaps[mipLevel - 1], currentSize, newSize);
            }

            return mipmaps;
        }

        private unsafe void UpdateSingleTextureMipmaps(uint textureId, byte[] imageData)
        {
            GLManager.GL.BindTexture(GLEnum.Texture2D, textureId);

            TextureAtlasMipmapGenerator.Color[] pixels = new TextureAtlasMipmapGenerator.Color[imageData.Length / 4];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new TextureAtlasMipmapGenerator.Color(
                    imageData[i * 4 + 0],
                    imageData[i * 4 + 1],
                    imageData[i * 4 + 2],
                    imageData[i * 4 + 3]
                );
            }

            TextureAtlasMipmapGenerator.Color[][] mipmaps = GenerateSingleTileMipmaps(pixels, 16);

            for (int mipLevel = 1; mipLevel < mipmaps.Length; mipLevel++)
            {
                int mipSize = 16 >> mipLevel;
                byte[] mipData = new byte[mipmaps[mipLevel].Length * 4];

                for (int i = 0; i < mipmaps[mipLevel].Length; i++)
                {
                    mipData[i * 4 + 0] = mipmaps[mipLevel][i].R;
                    mipData[i * 4 + 1] = mipmaps[mipLevel][i].G;
                    mipData[i * 4 + 2] = mipmaps[mipLevel][i].B;
                    mipData[i * 4 + 3] = mipmaps[mipLevel][i].A;
                }

                fixed (byte* ptr = mipData)
                {
                    GLManager.GL.TexImage2D(GLEnum.Texture2D, mipLevel,
                        (int)GLEnum.Rgba8, (uint)mipSize, (uint)mipSize, 0,
                        GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
                }
            }
        }

        public void refreshTextures()
        {
            TexturePackBase var1 = texturePack.selectedTexturePack;
            Iterator var2 = textureNameToImageMap.keySet().iterator();

            BufferedImage var4;
            while (var2.hasNext())
            {
                int var3 = ((Integer)var2.next()).intValue();
                var4 = (BufferedImage)textureNameToImageMap.get(Integer.valueOf(var3));
                setupTexture(var4, var3);
            }

            //ThreadDownloadImageData var8;
            //for (var2 = urlToImageDataMap.values().iterator(); var2.hasNext(); var8.textureSetupComplete = false)
            //{
            //    var8 = (ThreadDownloadImageData)var2.next();
            //}

            var2 = textureMap.keySet().iterator();

            string var9;
            while (var2.hasNext())
            {
                var9 = (string)var2.next();

                try
                {
                    if (var9.StartsWith("##"))
                    {
                        var4 = unwrapImageByColumns(readTextureImage(var1.getResourceAsStream(var9[2..])));
                    }
                    else if (var9.StartsWith("%clamp%"))
                    {
                        clampTexture = true;
                        var4 = readTextureImage(var1.getResourceAsStream(var9[7..]));
                    }
                    else if (var9.StartsWith("%blur%"))
                    {
                        blurTexture = true;
                        var4 = readTextureImage(var1.getResourceAsStream(var9[6..]));
                    }
                    else
                    {
                        var4 = readTextureImage(var1.getResourceAsStream(var9));
                    }

                    int var5 = ((Integer)textureMap.get(var9)).intValue();
                    setupTexture(var4, var5);
                    blurTexture = false;
                    clampTexture = false;
                }
                catch (java.io.IOException var7)
                {
                    var7.printStackTrace();
                }
            }

            var2 = field_28151_c.keySet().iterator();

            while (var2.hasNext())
            {
                var9 = (string)var2.next();

                try
                {
                    if (var9.StartsWith("##"))
                    {
                        var4 = unwrapImageByColumns(readTextureImage(var1.getResourceAsStream(var9[2..])));
                    }
                    else if (var9.StartsWith("%clamp%"))
                    {
                        clampTexture = true;
                        var4 = readTextureImage(var1.getResourceAsStream(var9[7..]));
                    }
                    else if (var9.StartsWith("%blur%"))
                    {
                        blurTexture = true;
                        var4 = readTextureImage(var1.getResourceAsStream(var9[6..]));
                    }
                    else
                    {
                        var4 = readTextureImage(var1.getResourceAsStream(var9));
                    }

                    func_28147_a(var4, (int[])field_28151_c.get(var9));
                    blurTexture = false;
                    clampTexture = false;
                }
                catch (java.io.IOException var6)
                {
                    var6.printStackTrace();
                }
            }

        }

        private BufferedImage readTextureImage(InputStream var1)
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
                byte a = (byte)((argb >> 24) & 0xFF);
                byte r = (byte)((argb >> 16) & 0xFF);
                byte g = (byte)((argb >> 8) & 0xFF);
                byte b = (byte)(argb & 0xFF);

                atlas.Pixels[i] = new TextureAtlasMipmapGenerator.Color(r, g, b, a);
            }

            return atlas;
        }

        public void bindTexture(int var1)
        {
            if (var1 >= 0)
            {
                GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var1);
            }
        }
    }
}