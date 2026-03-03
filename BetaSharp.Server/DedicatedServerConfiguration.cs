using Microsoft.Extensions.Logging;
using Exception = System.Exception;

namespace BetaSharp.Server;

internal class DedicatedServerConfiguration : IServerConfiguration
{
    private static readonly ILogger<DedicatedServerConfiguration> logger = Log.Instance.For<DedicatedServerConfiguration>();
    private readonly Dictionary<string, string> _properties = new(StringComparer.Ordinal);
    private readonly string _propertiesFilePath;

    public DedicatedServerConfiguration(string filePath)
    {
        _propertiesFilePath = filePath;
        if (File.Exists(filePath))
        {
            try
            {
                LoadProperties(filePath);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to load {filePath}: {ex}");
                generateNew();
            }
        }
        else
        {
            logger.LogWarning($"{filePath} does not exist");
            generateNew();
        }
    }

    private void LoadProperties(string filePath)
    {
        foreach (string line in File.ReadAllLines(filePath))
        {
            string trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#'))
                continue;
            int idx = trimmed.IndexOf('=');
            if (idx >= 0)
            {
                string key = trimmed[..idx].Trim();
                string value = trimmed[(idx + 1)..].Trim();
                _properties[key] = value;
            }
        }
    }

    public void generateNew()
    {
        logger.LogInformation("Generating new properties file");
        save();
    }

    public void save()
    {
        Save();
    }

    public void Save()
    {
        try
        {
            using StreamWriter writer = new(_propertiesFilePath, false);
            writer.WriteLine("#BetaSharp server properties");
            foreach (var kv in _properties)
            {
                writer.WriteLine($"{kv.Key}={kv.Value}");
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Failed to save {_propertiesFilePath}: {ex}");
            generateNew();
        }
    }

    public string getProperty(string property, string fallback)
    {
        return GetProperty(property, fallback);
    }

    public string GetProperty(string property, string fallback)
    {
        if (!_properties.ContainsKey(property))
        {
            _properties[property] = fallback;
            save();
        }

        return _properties.TryGetValue(property, out string? value) ? value : fallback;
    }

    public int getProperty(string property, int fallback)
    {
        return GetProperty(property, fallback);
    }

    public int GetProperty(string property, int fallback)
    {
        try
        {
            return int.Parse(getProperty(property, fallback.ToString()));
        }
        catch (Exception)
        {
            _properties[property] = fallback.ToString();
            return fallback;
        }
    }

    public bool getProperty(string property, bool fallback)
    {
        return GetProperty(property, fallback);
    }

    public bool GetProperty(string property, bool fallback)
    {
        try
        {
            string val = getProperty(property, fallback.ToString().ToLower());
            return string.Equals(val, "true", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception)
        {
            _properties[property] = fallback.ToString().ToLower();
            return fallback;
        }
    }

    public void setProperty(string property, bool value)
    {
        SetProperty(property, value);
    }

    public void SetProperty(string property, bool value)
    {
        _properties[property] = value.ToString().ToLower();
        save();
    }

    public string GetServerIp(string fallback) => GetProperty("server-ip", fallback);
    public int GetServerPort(int fallback) => GetProperty("server-port", fallback);
    public bool GetDualStack(bool fallback) => GetProperty("dual-stack", fallback);
    public bool GetOnlineMode(bool fallback) => GetProperty("online-mode", fallback);
    public bool GetSpawnAnimals(bool fallback) => GetProperty("spawn-animals", fallback);
    public bool GetPvpEnabled(bool fallback) => GetProperty("pvp", fallback);
    public bool GetAllowFlight(bool fallback) => GetProperty("allow-flight", fallback);
    public string GetLevelName(string fallback) => GetProperty("level-name", fallback);
    public string GetLevelSeed(string fallback) => GetProperty("level-seed", fallback);
    public bool GetSpawnMonsters(bool fallback) => GetProperty("spawn-monsters", fallback);
    public bool GetAllowNether(bool fallback) => GetProperty("allow-nether", fallback);
    public int GetMaxPlayers(int fallback) => GetProperty("max-players", fallback);
    public int GetViewDistance(int fallback) => GetProperty("view-distance", fallback);
    public bool GetWhiteList(bool fallback) => GetProperty("white-list", fallback);
    public int GetSpawnRegionSize(int fallback) => GetProperty("spawn-region-size", fallback);
}
