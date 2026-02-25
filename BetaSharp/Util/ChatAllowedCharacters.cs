namespace BetaSharp.Util;

public class ChatAllowedCharacters
{
    public static readonly string allowedCharacters = GetAllowedCharacters();
    public static readonly char[] allowedCharactersArray = ['/', '\n', '\r', '\t', '\u0000', '\f', '`', '?', '*', '\\', '<', '>', '|', '\"', ':'];

    private static string GetAllowedCharacters()
    {
        string content = AssetManager.Instance.getAsset("font.txt").getTextContent();
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;

        return string.Concat(
            content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !line.StartsWith('#'))
        );
    }
}
