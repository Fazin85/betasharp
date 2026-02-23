using System;
using System.IO;
using BetaSharp.Client.Input;
using Microsoft.Extensions.Logging;
using File = System.IO.File;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace BetaSharp.Client.Options;

public class GameOptions
{
    private readonly ILogger<GameOptions> _logger = Log.Instance.For<GameOptions>();

    private static readonly string[] RENDER_DISTANCES =
    [
        "options.renderDistance.far",
        "options.renderDistance.normal",
        "options.renderDistance.short",
        "options.renderDistance.tiny",
    ];
    private static readonly string[] Difficulties =
    [
        "options.difficulty.peaceful",
        "options.difficulty.easy",
        "options.difficulty.normal",
        "options.difficulty.hard",
    ];
    private static readonly string[] GuiScales =
    [
        "options.guiScale.auto",
        "options.guiScale.small",
        "options.guiScale.normal",
        "options.guiScale.large",
    ];

    private static readonly string[] AnisoLeves = ["options.off", "2x", "4x", "8x", "16x"];
    private static readonly string[] MSAALeves = ["options.off", "2x", "4x", "8x"];

    public static float MaxAnisotropy = 1.0f;
    public float MusicVolume = 1.0F;
    public float SoundVolume = 1.0F;
    public float MouseSensitivity = 0.5F;
    public float Brightness = 0.5F;
    public bool VSync = false;
    public bool InvertMouse;
    public int renderDistance;
    public bool ViewBobbing = true;
    public float LimitFramerate = 0.42857143f; // 0.428... = 120, 1.0 = 240, 0.0 = 30
    public float Fov = 0.44444445F; // (70 - 30) / 90
    public string Skin = "Default";

    public KeyBinding KeyBindForward = new("key.forward", 17);
    public KeyBinding KeyBindLeft = new("key.left", 30);
    public KeyBinding KeyBindBack = new("key.back", 31);
    public KeyBinding KeyBindRight = new("key.right", 32);
    public KeyBinding KeyBindJump = new("key.jump", 57);
    public KeyBinding KeyBindInventory = new("key.inventory", 18);
    public KeyBinding KeyBindDrop = new("key.drop", 16);
    public KeyBinding KeyBindChat = new("key.chat", 20);
    public KeyBinding KeyBindCommand = new("key.command", Keyboard.KEY_SLASH);
    public KeyBinding KeyBindToggleFog = new("key.fog", 33);
    public KeyBinding KeyBindSneak = new("key.sneak", 42);
    public KeyBinding[] KeyBindings;

    protected Minecraft _mc;
    private readonly string _optionsPath;
    public int Difficulty = 2;
    public bool HideGUI = false;
    public EnumCameraMode CameraMode = EnumCameraMode.FirstPerson;
    public bool ShowDebugInfo = false;
    public string LastServer = "";
    public bool InvertScrolling = false;
    public bool SmoothCamera = false;
    public bool DebugCamera = false;
    public float AmountScrolled = 1.0F;
    public float field_22271_G = 1.0F;
    public int GuiScale;
    public int AnisotropicLevel;
    public int MSAALevel;
    public int INITIAL_MSAA;
    private bool initialDebugMode;
    public bool UseMipmaps = true;
    public bool DebugMode;
    public bool EnvironmentAnimation = true;

    public GameOptions(Minecraft mc, string mcDataDir)
    {
        KeyBindings =
        [
            KeyBindForward,
            KeyBindLeft,
            KeyBindBack,
            KeyBindRight,
            KeyBindJump,
            KeyBindSneak,
            KeyBindDrop,
            KeyBindInventory,
            KeyBindChat,
            KeyBindToggleFog,
        ];
        _mc = mc;
        _optionsPath = System.IO.Path.Combine(mcDataDir, "options.txt");
        LoadOptions();
        INITIAL_MSAA = MSAALevel;
        initialDebugMode = DebugMode;
    }

    public GameOptions() { }

    public string GetKeyBindingDescription(int keyBindingIndex)
    {
        TranslationStorage translations = TranslationStorage.Instance;
        return translations.TranslateKey(KeyBindings[keyBindingIndex].keyDescription);
    }

    public string GetOptionDisplayString(int keyBindingIndex)
    {
        return Keyboard.getKeyName(KeyBindings[keyBindingIndex].keyCode);
    }

    public void SetKeyBinding(int keyBindingIndex, int keyCode)
    {
        KeyBindings[keyBindingIndex].keyCode = keyCode;
        SaveOptions();
    }

    public void SetOptionFloatValue(EnumOptions option, float value)
    {
        switch (option)
        {
            case EnumOptions.Music:
                MusicVolume = value;
                _mc.sndManager.OnSoundOptionsChanged();
                break;
            case EnumOptions.Sound:
                SoundVolume = value;
                _mc.sndManager.OnSoundOptionsChanged();
                break;
            case EnumOptions.Sensitivity:
                MouseSensitivity = value;
                break;
            case EnumOptions.FramerateLimit:
                LimitFramerate = value;
                break;
            case EnumOptions.Fov:
                Fov = value;
                break;
        }

        SaveOptions();
    }

    public void SetOptionValue(EnumOptions option, int increment)
    {
        switch (option)
        {
            case EnumOptions.InvertMouse:
                InvertMouse = !InvertMouse;
                break;
            case EnumOptions.RenderDistance:
                renderDistance = renderDistance + increment & 3;
                break;
            case EnumOptions.GuiScale:
                GuiScale = GuiScale + increment & 3;
                break;
            case EnumOptions.ViewBobbing:
                ViewBobbing = !ViewBobbing;
                break;
            case EnumOptions.VSync:
                VSync = !VSync;
                Display.getGlfw().SwapInterval(VSync ? 1 : 0);
                break;
            case EnumOptions.Difficulty:
                Difficulty = Difficulty + increment & 3;
                break;
            case EnumOptions.Anisotropic:
                AnisotropicLevel = (AnisotropicLevel + increment) % 5;
                int anisoValue = AnisotropicLevel == 0 ? 0 : (int)System.Math.Pow(2, AnisotropicLevel);
                if (anisoValue > MaxAnisotropy)
                {
                    AnisotropicLevel = 0;
                }
                if (Minecraft.INSTANCE?.textureManager != null)
                {
                    Minecraft.INSTANCE.textureManager.Reload();
                }
                break;
            case EnumOptions.Mipmaps:
                UseMipmaps = !UseMipmaps;
                if (Minecraft.INSTANCE?.textureManager != null)
                {
                    Minecraft.INSTANCE.textureManager.Reload();
                }
                break;
            case EnumOptions.Msaa:
                MSAALevel = (MSAALevel + increment) % 4;
                break;
            case EnumOptions.DebugMode:
                DebugMode = !DebugMode;
                Profiling.Profiler.Enabled = DebugMode;
                break;
            case EnumOptions.EnvironmentAnimation:
                EnvironmentAnimation = !EnvironmentAnimation;
                break;
        }

        SaveOptions();
    }

    public float GetOptionFloatValue(EnumOptions option)
    {
        return option switch
        {
            EnumOptions.Music => MusicVolume,
            EnumOptions.Sound => SoundVolume,
            EnumOptions.Sensitivity => MouseSensitivity,
            EnumOptions.FramerateLimit => LimitFramerate,
            EnumOptions.Fov => Fov,
            _ => 0.0F,
        };
    }

    public bool GetOptionOrdinalValue(EnumOptions option)
    {
        return option switch
        {
            EnumOptions.InvertMouse => InvertMouse,
            EnumOptions.ViewBobbing => ViewBobbing,
            EnumOptions.Mipmaps => UseMipmaps,
            EnumOptions.DebugMode => DebugMode,
            EnumOptions.EnvironmentAnimation => EnvironmentAnimation,
            EnumOptions.VSync => VSync,
            _ => false,
        };
    }

    public string GetKeyBinding(EnumOptions option)
    {
        TranslationStorage translations = TranslationStorage.Instance;
        string label = GetOptionLabel(option, translations) + ": ";

        if (option.IsFloat())
        {
            return FormatFloatValue(option, label, translations);
        }
        else if (option == EnumOptions.DebugMode)
        {
            return FormatDebugMode(label, translations);
        }
        else if (option.IsBoolean())
        {
            bool isEnabled = GetOptionOrdinalValue(option);
            return label + (isEnabled ? translations.TranslateKey("options.on") : translations.TranslateKey("options.off"));
        }
        else if (option == EnumOptions.Msaa)
        {
            return FormatMsaaValue(label, translations);
        }
        else
        {
            return FormatEnumValue(option, label, translations);
        }
    }

    private string GetOptionLabel(EnumOptions option, TranslationStorage translations)
    {
        return option switch
        {
            EnumOptions.FramerateLimit => "Max FPS",
            EnumOptions.Brightness => "Brightness",
            EnumOptions.VSync => "VSync",
            EnumOptions.Fov => "FOV",
            _ => translations.TranslateKey(option.GetTranslationKey()),
        };
    }

    private string FormatFloatValue(EnumOptions option, string label, TranslationStorage translations)
    {
        float value = GetOptionFloatValue(option);

        return option switch
        {
            EnumOptions.Sensitivity => value == 0.0F
                ? label + translations.TranslateKey("options.sensitivity.min")
                : value == 1.0F
                    ? label + translations.TranslateKey("options.sensitivity.max")
                    : label + (int)(value * 200.0F) + "%",
            EnumOptions.FramerateLimit => FormatFramerateValue(label, value),
            EnumOptions.Fov => label + (30 + (int)(value * 90.0f)),
            _ => value == 0.0F
                ? label + translations.TranslateKey("options.off")
                : label + $"{(int)(value * 100.0F)}%",
        };
    }

    private string FormatFramerateValue(string label, float value)
    {
        int fps = 30 + (int)(value * 210.0f);
        return label + (fps == 240 ? "Unlimited" : fps + " FPS");
    }

    private string FormatMsaaValue(string label, TranslationStorage translations)
    {
        string result = label + (MSAALevel == 0 ? translations.TranslateKey("options.off") : MSAALeves[MSAALevel]);
        if (MSAALevel != INITIAL_MSAA)
        {
            result += " (Reload required)";
        }
        return result;
    }

    private string FormatDebugMode(string label, TranslationStorage translations)
    {
        bool isEnabled = GetOptionOrdinalValue(EnumOptions.DebugMode);
        string result = label + (isEnabled ? translations.TranslateKey("options.on") : translations.TranslateKey("options.off"));
        if (DebugMode != initialDebugMode)
        {
            result += " [!]";
        }

        return result;
    }

    private string FormatEnumValue(EnumOptions option, string label, TranslationStorage translations)
    {
        return option switch
        {
            EnumOptions.RenderDistance => label + translations.TranslateKey(RENDER_DISTANCES[renderDistance]),
            EnumOptions.Difficulty => label + translations.TranslateKey(Difficulties[Difficulty]),
            EnumOptions.GuiScale => label + translations.TranslateKey(GuiScales[GuiScale]),
            EnumOptions.Anisotropic => label + (AnisotropicLevel == 0 ? translations.TranslateKey("options.off") : AnisoLeves[AnisotropicLevel]),
            _ => label,
        };
    }

    public void LoadOptions()
    {
        try
        {
            if (!File.Exists(_optionsPath)) throw new FileNotFoundException($"Options file not found at {_optionsPath}");
            using StreamReader reader = new StreamReader(_optionsPath);
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                try
                {
                    string[] parts = line.Split(':');
                    if (parts.Length >= 2) LoadOptionFromParts(parts);
                }
                catch (Exception)
                {
                    _logger.LogError($"Skipping bad option: {line}");
                }
            }
        }
        catch (Exception)
        {
            _logger.LogError("Failed to load options");
        }
    }

    private void LoadOptionFromParts(string[] parts)
    {
        if (parts.Length < 2) return;

        string key = parts[0];
        string value = parts[1];

        switch (key)
        {
            case "music": MusicVolume = ParseFloat(value); break;
            case "sound": SoundVolume = ParseFloat(value); break;
            case "mouseSensitivity": MouseSensitivity = ParseFloat(value); break;
            case "invertYMouse": InvertMouse = value == "true"; break;
            case "viewDistance": renderDistance = int.Parse(value); break;
            case "guiScale": GuiScale = int.Parse(value); break;
            case "bobView": ViewBobbing = value == "true"; break;
            case "fpsLimit": LimitFramerate = ParseFloat(value); break;
            case "vsync": VSync = bool.Parse(value); break;
            case "fov": Fov = ParseFloat(value); break;
            case "difficulty": Difficulty = int.Parse(value); break;
            case "skin": Skin = value; break;
            case "lastServer": LastServer = value; break;
            case "anisotropicLevel": AnisotropicLevel = int.Parse(value); break;
            case "msaaLevel":
                MSAALevel = int.Parse(value);
                if (MSAALevel > 3) MSAALevel = 3;
                break;
            case "useMipmaps": UseMipmaps = value == "true"; break;
            case "debugMode": DebugMode = value == "true"; break;
            case "envAnimation": EnvironmentAnimation = value == "true"; break;
            case "cameraMode": CameraMode = (EnumCameraMode)int.Parse(value); break;
            case "thirdPersonView": // backward compatibility
                CameraMode = value == "true" ? EnumCameraMode.ThirdPerson : EnumCameraMode.FirstPerson;
                break;

            default:
                if (key.StartsWith("key_"))
                {
                    string bindName = key[4..];

                    for (int i = 0; i < KeyBindings.Length; ++i)
                    {
                        if (KeyBindings[i].keyDescription == bindName)
                        {
                            KeyBindings[i].keyCode = int.Parse(value);
                            break;
                        }
                    }
                }
                break;
        }
    }

    private float ParseFloat(string value)
    {
        return value switch
        {
            "true" => 1.0F,
            "false" => 0.0F,
            _ => float.Parse(value)
        };
    }

    public void SaveOptions()
    {
        try
        {
            using var writer = new StreamWriter(_optionsPath);
            writer.WriteLine($"music:{MusicVolume}");
            writer.WriteLine($"sound:{SoundVolume}");
            writer.WriteLine($"invertYMouse:{InvertMouse.ToString().ToLower()}");
            writer.WriteLine($"mouseSensitivity:{MouseSensitivity}");
            writer.WriteLine($"viewDistance:{renderDistance}");
            writer.WriteLine($"guiScale:{GuiScale}");
            writer.WriteLine($"bobView:{ViewBobbing.ToString().ToLower()}");
            writer.WriteLine($"fpsLimit:{LimitFramerate}");
            writer.WriteLine($"vsync:{VSync}");
            writer.WriteLine($"fov:{Fov}");
            writer.WriteLine($"difficulty:{Difficulty}");
            writer.WriteLine($"skin:{Skin}");
            writer.WriteLine($"lastServer:{LastServer}");
            writer.WriteLine($"anisotropicLevel:{AnisotropicLevel}");
            writer.WriteLine($"msaaLevel:{MSAALevel}");
            writer.WriteLine($"useMipmaps:{UseMipmaps.ToString().ToLower()}");
            writer.WriteLine($"debugMode:{DebugMode.ToString().ToLower()}");
            writer.WriteLine($"envAnimation:{EnvironmentAnimation.ToString().ToLower()}");
            writer.WriteLine($"cameraMode:{(int)CameraMode}");

            foreach (var bind in KeyBindings)
            {
                writer.WriteLine($"key_{bind.keyDescription}:{bind.keyCode}");
            }

            writer.Close();
        }
        catch (Exception exception)
        {
            _logger.LogError($"Failed to save options: {exception.Message}");
        }
    }
}
