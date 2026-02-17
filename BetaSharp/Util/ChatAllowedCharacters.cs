using java.io;

namespace BetaSharp.Util;

public class ChatAllowedCharacters : java.lang.Object
{
    public static readonly string allowedCharacters = getAllowedCharacters();
    public static readonly char[] allowedCharactersArray = ['/', '\n', '\r', '\t', '\u0000', '\f', '`', '?', '*', '\\', '<', '>', '|', '\"', ':'];

    private static string getAllowedCharacters()
    {
        string var0 = "";

        try
        {
            BufferedReader var1 = new(new java.io.StringReader(AssetManager.Instance.getAsset("font.txt").getTextContent()));
            string var2 = "";

            while (true)
            {
                var2 = var1.readLine();
                if (var2 == null)
                {
                    var1.close();
                    break;
                }

                if (!var2.StartsWith('#'))
                {
                    var0 += var2;
                }
            }
        }
        catch (java.lang.Exception) { }

        return var0;
    }
}