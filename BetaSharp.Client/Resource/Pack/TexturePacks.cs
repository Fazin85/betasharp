using java.util;

namespace BetaSharp.Client.Resource.Pack;

public class TexturePacks : java.lang.Object
{
    private List availTexturePacks = new ArrayList();
    private readonly TexturePack defaultTexturePack = new BuiltInTexturePack();
    public TexturePack selectedTexturePack;
    private readonly Map field_6538_d = new HashMap();
    private readonly Minecraft mc;
    private readonly java.io.File texturePackDir;
    private string currentTexturePack;

    public TexturePacks(Minecraft var1, java.io.File var2)
    {
        mc = var1;
        texturePackDir = new java.io.File(var2, "texturepacks");
        if (!texturePackDir.exists())
        {
            texturePackDir.mkdirs();
        }

        currentTexturePack = var1.options.skin;
        updateAvaliableTexturePacks();
        selectedTexturePack.func_6482_a();
    }

    public bool setTexturePack(TexturePack var1)
    {
        if (var1 == selectedTexturePack)
        {
            return false;
        }
        else
        {
            selectedTexturePack.closeTexturePackFile();
            currentTexturePack = var1.texturePackFileName;
            selectedTexturePack = var1;
            mc.options.skin = currentTexturePack;
            mc.options.saveOptions();
            selectedTexturePack.func_6482_a();
            return true;
        }
    }

    public void updateAvaliableTexturePacks()
    {
        ArrayList var1 = [];
        selectedTexturePack = null;
        var1.add(defaultTexturePack);
        if (texturePackDir.exists() && texturePackDir.isDirectory())
        {
            java.io.File[] var2 = texturePackDir.listFiles();
            java.io.File[] var3 = var2;
            int var4 = var2.Length;

            for (int var5 = 0; var5 < var4; ++var5)
            {
                java.io.File var6 = var3[var5];
                if (var6.isFile() && var6.getName().ToLower().EndsWith(".zip"))
                {
                    string var7 = var6.getName() + ":" + var6.length() + ":" + var6.lastModified();

                    try
                    {
                        if (!field_6538_d.containsKey(var7))
                        {
                            ZippedTexturePack var8 = new(var6)
                            {
                                field_6488_d = var7
                            };
                            field_6538_d.put(var7, var8);
                            var8.func_6485_a(mc);
                        }

                        TexturePack var12 = (TexturePack)field_6538_d.get(var7);
                        if (var12.texturePackFileName.Equals(currentTexturePack))
                        {
                            selectedTexturePack = var12;
                        }

                        var1.add(var12);
                    }
                    catch (java.io.IOException ex)
                    {
                        ex.printStackTrace();
                    }
                }
            }
        }

        selectedTexturePack ??= defaultTexturePack;

        availTexturePacks.removeAll(var1);
        Iterator var10 = availTexturePacks.iterator();

        while (var10.hasNext())
        {
            TexturePack var11 = (TexturePack)var10.next();
            var11.unload(mc);
            field_6538_d.remove(var11.field_6488_d);
        }

        availTexturePacks = var1;
    }

    public List availableTexturePacks()
    {
        return new ArrayList(availTexturePacks);
    }
}