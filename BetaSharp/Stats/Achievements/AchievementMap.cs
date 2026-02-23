namespace BetaSharp.Stats.Achievements;

internal class AchievementMap
{
    public static readonly AchievementMap Instance = new();
    
    private readonly Dictionary<int, string> _guidMap = new();

    private AchievementMap()
    {
        try
        {
            string content = AssetManager.Instance.getAsset("achievement/map.txt").getTextContent();
            
            using StringReader reader = new(content);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                
                if (parts.Length >= 2 && int.TryParse(parts[0], out int id))
                {
                    _guidMap[id] = parts[1].Trim();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading achievement map: {ex}");
        }
    }

    public static string GetGuid(int id)
    {
        return Instance._guidMap.TryGetValue(id, out string? guid) ? guid : string.Empty;
    }
}