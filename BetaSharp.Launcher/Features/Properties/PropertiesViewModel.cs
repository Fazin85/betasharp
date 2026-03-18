using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Hosting;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BetaSharp.Launcher.Features.Properties;

// Props should not be hard-coded.
internal sealed partial class PropertiesViewModel(NavigationService navigationService) : ObservableValidator
{
    [ObservableProperty]
    public partial bool IsReady { get; set; }

    [ObservableProperty]
    [RegularExpression(@"^$|^(\d{1,3}\.){3}\d{1,3}$", ErrorMessage = "Server IP must be empty or a valid IPv4 address.")]
    public partial string? ServerIp { get; set; }

    [ObservableProperty]
    public partial bool DualStack { get; set; } = false;

    [ObservableProperty]
    [Range(1, 65535, ErrorMessage = "Server port must be between 1 and 65535.")]
    public partial int ServerPort { get; set; } = 25565;

    [ObservableProperty]
    public partial bool OnlineMode { get; set; } = true;

    [ObservableProperty]
    public partial bool SpawnAnimals { get; set; } = true;

    [ObservableProperty]
    public partial bool Pvp { get; set; } = true;

    [ObservableProperty]
    public partial bool AllowFlight { get; set; } = false;

    [ObservableProperty]
    [Range(2, 32, ErrorMessage = "View distance must be between 2 and 32.")]
    public partial int ViewDistance { get; set; } = 10;

    [ObservableProperty]
    [Range(1, 500, ErrorMessage = "Max players must be between 1 and 500.")]
    public partial int MaxPlayers { get; set; } = 20;

    [ObservableProperty]
    public partial bool WhiteList { get; set; } = false;

    [ObservableProperty]
    [Required(ErrorMessage = "Level name is required.")]
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Level name cannot be empty.")]
    public partial string LevelName { get; set; } = "world";

    [ObservableProperty]
    public partial string? LevelSeed { get; set; }

    [ObservableProperty]
    public partial int LevelType { get; set; }

    [ObservableProperty]
    public partial string? GeneratorSettings { get; set; }

    [ObservableProperty]
    public partial bool SpawnMonsters { get; set; } = true;

    [ObservableProperty]
    [Range(0, 9999, ErrorMessage = "Spawn region size must be between 0 and 9999.")]
    public partial int SpawnRegionSize { get; set; } = 196;

    [ObservableProperty]
    public partial bool AllowNether { get; set; } = true;

    private readonly string _path = Path.Combine(AppContext.BaseDirectory, nameof(Kind.Server), "server.properties");

    [RelayCommand]
    private async Task InitializeAsync()
    {
        IsReady = false;

        try
        {
            foreach (string line in await File.ReadAllLinesAsync(_path))
            {
                string trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#') || trimmed.StartsWith('!'))
                {
                    continue;
                }

                int index = trimmed.IndexOf('=');

                if (index < 0)
                {
                    index = trimmed.IndexOf(':');
                }

                if (index < 0)
                {
                    continue;
                }

                string value = trimmed[(index + 1)..].Trim();

                switch (trimmed[..index].Trim())
                {
                    case "server-ip":
                        {
                            ServerIp = value;
                            break;
                        }

                    case "dual-stack" when bool.TryParse(value, out bool result):
                        {
                            DualStack = result;
                            break;
                        }

                    case "server-port" when int.TryParse(value, out int result):
                        {
                            ServerPort = result;
                            break;
                        }

                    case "online-mode" when bool.TryParse(value, out bool result):
                        {
                            OnlineMode = result;
                            break;
                        }

                    case "spawn-animals" when bool.TryParse(value, out bool result):
                        {
                            SpawnAnimals = result;
                            break;
                        }

                    case "pvp" when bool.TryParse(value, out bool result):
                        {
                            Pvp = result;
                            break;
                        }

                    case "allow-flight" when bool.TryParse(value, out bool result):
                        {
                            AllowFlight = result;
                            break;
                        }

                    case "view-distance" when int.TryParse(value, out int result):
                        {
                            ViewDistance = result;
                            break;
                        }

                    case "max-players" when int.TryParse(value, out int result):
                        {
                            MaxPlayers = result;
                            break;
                        }

                    case "white-list" when bool.TryParse(value, out bool result):
                        {
                            WhiteList = result;
                            break;
                        }

                    case "level-name" when !string.IsNullOrWhiteSpace(value):
                        {
                            LevelName = value;
                            break;
                        }

                    case "level-seed":
                        {
                            LevelSeed = value;
                            break;
                        }

                    case "level-type":
                        {
                            LevelType = value.ToUpperInvariant() switch
                            {
                                "DEFAULT" => 0,
                                "FLAT" => 1,
                                _ => 0
                            };
                            break;
                        }

                    case "generator-settings":
                        {
                            GeneratorSettings = value;
                            break;
                        }

                    case "spawn-monsters" when bool.TryParse(value, out bool result):
                        {
                            SpawnMonsters = result;
                            break;
                        }

                    case "spawn-region-size" when int.TryParse(value, out int result):
                        {
                            SpawnRegionSize = result;
                            break;
                        }

                    case "allow-nether" when bool.TryParse(value, out bool result):
                        {
                            AllowNether = result;
                            break;
                        }
                }
            }
        }
        catch (IOException)
        {
            await WriteAsync();
        }

        IsReady = true;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
#pragma warning disable IL2026
        ValidateAllProperties();
#pragma warning restore IL2026

        if (HasErrors)
        {
            return;
        }

        await WriteAsync();

        navigationService.Navigate<HostingViewModel>();
    }

    [RelayCommand]
    private void Back()
    {
        navigationService.Navigate<HostingViewModel>();
    }

    private async Task WriteAsync()
    {
        string levelType = LevelType switch
        {
            0 => "DEFAULT",
            1 => "FLAT",
            _ => "DEFAULT"
        };

        string value = $"""
                        # BetaSharp server properties
                        # Generated by BetaSharp's launcher
                        server-ip={ServerIp}
                        dual-stack={DualStack}
                        server-port={ServerPort}
                        online-mode={OnlineMode}
                        spawn-animals={SpawnAnimals}
                        pvp={Pvp}
                        allow-flight={AllowFlight}
                        view-distance={ViewDistance}
                        max-players={MaxPlayers}
                        white-list={WhiteList}
                        level-name={LevelName}
                        level-seed={LevelSeed}
                        level-type={levelType}
                        generator-settings={GeneratorSettings}
                        spawn-monsters={SpawnMonsters}
                        spawn-region-size={SpawnRegionSize}
                        allow-nether={AllowNether}
                        """;

        await File.WriteAllTextAsync(_path, value);
    }
}
