using BetaSharp.Client.Input;
using java.io;

namespace BetaSharp.Client.Options;

public class GameOptions : java.lang.Object
{
    private static readonly string[] RENDER_DISTANCES =
    [
        "options.renderDistance.far",
        "options.renderDistance.normal",
        "options.renderDistance.short",
        "options.renderDistance.tiny",
    ];
    private static readonly string[] DIFFICULTIES =
    [
        "options.difficulty.peaceful",
        "options.difficulty.easy",
        "options.difficulty.normal",
        "options.difficulty.hard",
    ];
    private static readonly string[] GUISCALES =
    [
        "options.guiScale.auto",
        "options.guiScale.small",
        "options.guiScale.normal",
        "options.guiScale.large",
    ];
    // private static readonly string[] LIMIT_FRAMERATES = ["performance.max", "performance.balanced", "performance.powersaver"];
    private static readonly string[] ANISO_LEVELS = ["options.off", "2x", "4x", "8x", "16x"];
    private static readonly string[] MSAA_LEVELS = ["options.off", "2x", "4x", "8x"];
    public static float MaxAnisotropy = 1.0f;
    public float musicVolume = 1.0F;
    public float soundVolume = 1.0F;
    public float mouseSensitivity = 0.5F;
    public bool invertMouse;
    public int renderDistance;
    public bool viewBobbing = true;
    public float limitFramerate = 0.42857143f; // 0.428... = 120, 1.0 = 240, 0.0 = 30
    public float fov = 0.44444445F; // (70 - 30) / 90
    public string skin = "Default";
    public KeyBinding keyBindForward = new("key.forward", 17);
    public KeyBinding keyBindLeft = new("key.left", 30);
    public KeyBinding keyBindBack = new("key.back", 31);
    public KeyBinding keyBindRight = new("key.right", 32);
    public KeyBinding keyBindJump = new("key.jump", 57);
    public KeyBinding keyBindInventory = new("key.inventory", 18);
    public KeyBinding keyBindDrop = new("key.drop", 16);
    public KeyBinding keyBindChat = new("key.chat", 20);
    public KeyBinding keyBindCommand = new("key.command", Keyboard.KEY_SLASH);
    public KeyBinding keyBindToggleFog = new("key.fog", 33);
    public KeyBinding keyBindSneak = new("key.sneak", 42);
    public KeyBinding[] keyBindings;
    protected Minecraft mc;
    private readonly java.io.File optionsFile;
    public int difficulty = 2;
    public bool hideGUI = false;
    public bool thirdPersonView = false;
    public bool showDebugInfo = false;
    public string lastServer = "";
    public bool invertScrolling = false;
    public bool smoothCamera = false;
    public bool debugCamera = false;
    public float amountScrolled = 1.0F;
    public float field_22271_G = 1.0F;
    public int guiScale;
    public int anisotropicLevel;
    public int msaaLevel;
    public int INITIAL_MSAA;
    public bool useMipmaps = true;
    public bool debugMode;
    public bool environmentAnimation = true;

    public GameOptions(Minecraft mc, java.io.File mcDataDir)
    {
        keyBindings =
        [
            keyBindForward,
            keyBindLeft,
            keyBindBack,
            keyBindRight,
            keyBindJump,
            keyBindSneak,
            keyBindDrop,
            keyBindInventory,
            keyBindChat,
            keyBindToggleFog,
        ];
        this.mc = mc;
        optionsFile = new java.io.File(mcDataDir, "options.txt");
        loadOptions();
        INITIAL_MSAA = msaaLevel;
    }

    public GameOptions()
    {
    }

    public string getKeyBindingDescription(int keyBindingIndex)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        return translations.translateKey(keyBindings[keyBindingIndex].keyDescription);
    }

    public string getOptionDisplayString(int keyBindingIndex)
    {
        return Keyboard.getKeyName(keyBindings[keyBindingIndex].keyCode);
    }

    public void setKeyBinding(int keyBindingIndex, int keyCode)
    {
        keyBindings[keyBindingIndex].keyCode = keyCode;
        saveOptions();
    }

    public void setOptionFloatValue(EnumOptions option, float value)
    {
        if (option == EnumOptions.MUSIC)
        {
            musicVolume = value;
            mc.sndManager.OnSoundOptionsChanged();
        }
        else if (option == EnumOptions.SOUND)
        {
            soundVolume = value;
            mc.sndManager.OnSoundOptionsChanged();
        }
        else if (option == EnumOptions.SENSITIVITY)
        {
            mouseSensitivity = value;
        }
        else if (option == EnumOptions.FRAMERATE_LIMIT)
        {
            limitFramerate = value;
        }
        else if (option == EnumOptions.FOV)
        {
            fov = value;
        }

        saveOptions();
    }

    public void setOptionValue(EnumOptions option, int increment)
    {
        if (option == EnumOptions.INVERT_MOUSE)
        {
            invertMouse = !invertMouse;
        }
        else if (option == EnumOptions.RENDER_DISTANCE)
        {
            renderDistance = renderDistance + increment & 3;
        }
        else if (option == EnumOptions.GUI_SCALE)
        {
            guiScale = guiScale + increment & 3;
        }
        else if (option == EnumOptions.VIEW_BOBBING)
        {
            viewBobbing = !viewBobbing;
        }
        else if (option == EnumOptions.DIFFICULTY)
        {
            difficulty = difficulty + increment & 3;
        }
        else if (option == EnumOptions.ANISOTROPIC)
        {
            anisotropicLevel = (anisotropicLevel + increment) % 5;
            int anisoValue = anisotropicLevel == 0 ? 0 : (int)System.Math.Pow(2, anisotropicLevel);
            if (anisoValue > MaxAnisotropy)
            {
                anisotropicLevel = 0;
            }
            if (Minecraft.INSTANCE?.textureManager != null)
            {
                Minecraft.INSTANCE.textureManager.Reload();
            }
        }
        else if (option == EnumOptions.MIPMAPS)
        {
            useMipmaps = !useMipmaps;
            if (Minecraft.INSTANCE?.textureManager != null)
            {
                Minecraft.INSTANCE.textureManager.Reload();
            }
        }
        else if (option == EnumOptions.MSAA)
        {
            msaaLevel = (msaaLevel + increment) % 4;
        }
        else if (option == EnumOptions.DEBUG_MODE)
        {
            debugMode = !debugMode;
            Profiling.Profiler.Enabled = debugMode;
        }
        else if (option == EnumOptions.ENVIRONMENT_ANIMATION)
        {
            environmentAnimation = !environmentAnimation;
        }

        saveOptions();
    }

    public float getOptionFloatValue(EnumOptions option)
    {
        if (option == EnumOptions.MUSIC) return musicVolume;
        if (option == EnumOptions.SOUND) return soundVolume;
        if (option == EnumOptions.SENSITIVITY) return mouseSensitivity;
        if (option == EnumOptions.FRAMERATE_LIMIT) return limitFramerate;
        if (option == EnumOptions.FOV) return fov;
        return 0.0F;
    }

    public bool getOptionOrdinalValue(EnumOptions option)
    {
        int mappedValue = EnumOptionsMappingHelper.enumOptionsMappingHelperArray[option.ordinal()];
        return mappedValue switch
        {
            1 => invertMouse,
            2 => viewBobbing,
            3 => useMipmaps,
            4 => debugMode,
            5 => environmentAnimation,
            _ => false
        };
    }

    public string getKeyBinding(EnumOptions option)
    {
        TranslationStorage translations = TranslationStorage.getInstance();
        string label = GetOptionLabel(option, translations) + ": ";

        if (option.getEnumFloat())
        {
            return FormatFloatValue(option, label, translations);
        }
        else if (option.getEnumBoolean())
        {
            bool isEnabled = getOptionOrdinalValue(option);
            return label + (isEnabled ? translations.translateKey("options.on") : translations.translateKey("options.off"));
        }
        else if (option == EnumOptions.MSAA)
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
        if (option == EnumOptions.FRAMERATE_LIMIT) return "Max FPS";
        if (option == EnumOptions.FOV) return "FOV";
        return translations.translateKey(option.getEnumString());
    }

    private string FormatFloatValue(EnumOptions option, string label, TranslationStorage translations)
    {
        float value = getOptionFloatValue(option);

        if (option == EnumOptions.SENSITIVITY)
        {
            return value == 0.0F ? label + translations.translateKey("options.sensitivity.min") : (value == 1.0F ? label + translations.translateKey("options.sensitivity.max") : label + (int)(value * 200.0F) + "%");
        }
        else if (option == EnumOptions.FRAMERATE_LIMIT)
        {
            return FormatFramerateValue(label, value);
        }
        else if (option == EnumOptions.FOV)
        {
            return label + (30 + (int)(value * 90.0f));
        }
        else
        {
            return (value == 0.0F ? label + translations.translateKey("options.off") : label + (int)(value * 100.0F) + "%");
        }
    }

    private string FormatFramerateValue(string label, float value)
    {
        int fps = 30 + (int)(value * 210.0f);
        return label + (fps == 240 ? "Unlimited" : fps + " FPS");
    }

    private string FormatMsaaValue(string label, TranslationStorage translations)
    {
        string result = label + (msaaLevel == 0 ? translations.translateKey("options.off") : MSAA_LEVELS[msaaLevel]);
        if (msaaLevel != INITIAL_MSAA)
        {
            result += " (Reload required)";
        }
        return result;
    }

    private string FormatEnumValue(EnumOptions option, string label, TranslationStorage translations)
    {
        if (option == EnumOptions.RENDER_DISTANCE) return label + translations.translateKey(RENDER_DISTANCES[renderDistance]);
        if (option == EnumOptions.DIFFICULTY) return label + translations.translateKey(DIFFICULTIES[difficulty]);
        if (option == EnumOptions.GUI_SCALE) return label + translations.translateKey(GUISCALES[guiScale]);
        if (option == EnumOptions.ANISOTROPIC) return label + (anisotropicLevel == 0 ? translations.translateKey("options.off") : ANISO_LEVELS[anisotropicLevel]);
        return label;
    }

    public void loadOptions()
    {
        try
        {
            if (!optionsFile.exists())
            {
                return;
            }

            BufferedReader reader = new(new FileReader(optionsFile));
            string line = "";

            while (true)
            {
                line = reader.readLine();
                if (line == null)
                {
                    reader.close();
                    break;
                }

                try
                {
                    string[] parts = line.Split(':');
                    LoadOptionFromParts(parts);
                }
                catch (System.Exception)
                {
                    Log.Error($"Skipping bad option: {line}");
                }
            }
        }
        catch (System.Exception)
        {
            Log.Error("Failed to load options");
        }
    }

    private void LoadOptionFromParts(string[] parts)
    {
        switch (parts[0])
        {
            case "music":
                musicVolume = parseFloat(parts[1]);
                break;
            case "sound":
                soundVolume = parseFloat(parts[1]);
                break;
            case "mouseSensitivity":
                mouseSensitivity = parseFloat(parts[1]);
                break;
            case "invertYMouse":
                invertMouse = parts[1].Equals("true");
                break;
            case "viewDistance":
                renderDistance = int.Parse(parts[1]);
                break;
            case "guiScale":
                guiScale = int.Parse(parts[1]);
                break;
            case "bobView":
                viewBobbing = parts[1].Equals("true");
                break;
            case "fpsLimit":
                limitFramerate = parseFloat(parts[1]);
                break;
            case "fov":
                fov = parseFloat(parts[1]);
                break;
            case "difficulty":
                difficulty = int.Parse(parts[1]);
                break;
            case "skin":
                skin = parts[1];
                break;
            case "lastServer" when parts.Length >= 2:
                lastServer = parts[1];
                break;
            case "anisotropicLevel":
                anisotropicLevel = int.Parse(parts[1]);
                break;
            case "msaaLevel":
                msaaLevel = int.Parse(parts[1]);
                if (msaaLevel > 3) msaaLevel = 3;
                break;
            case "useMipmaps":
                useMipmaps = parts[1].Equals("true");
                break;
            case "debugMode":
                debugMode = parts[1].Equals("true");
                break;
            case "envAnimation":
                environmentAnimation = parts[1].Equals("true");
                break;
        }

        // Load keybindings
        for (int i = 0; i < keyBindings.Length; ++i)
        {
            if (parts[0].Equals("key_" + keyBindings[i].keyDescription))
            {
                keyBindings[i].keyCode = int.Parse(parts[1]);
            }
        }
    }

    private float parseFloat(string value)
    {
        return value switch
        {
            "true" => 1.0F,
            "false" => 0.0F,
            _ => float.Parse(value)
        };
    }

    public void saveOptions()
    {
        try
        {
            using System.IO.StreamWriter writer = new(optionsFile.getAbsolutePath());
            writer.WriteLine("music:" + musicVolume);
            writer.WriteLine("sound:" + soundVolume);
            writer.WriteLine("invertYMouse:" + invertMouse);
            writer.WriteLine("mouseSensitivity:" + mouseSensitivity);
            writer.WriteLine("viewDistance:" + renderDistance);
            writer.WriteLine("guiScale:" + guiScale);
            writer.WriteLine("bobView:" + viewBobbing);
            writer.WriteLine("fpsLimit:" + limitFramerate);
            writer.WriteLine("fov:" + fov);
            writer.WriteLine("difficulty:" + difficulty);
            writer.WriteLine("skin:" + skin);
            writer.WriteLine("lastServer:" + lastServer);
            writer.WriteLine("anisotropicLevel:" + anisotropicLevel);
            writer.WriteLine("msaaLevel:" + msaaLevel);
            writer.WriteLine("useMipmaps:" + useMipmaps);
            writer.WriteLine("debugMode:" + debugMode);
            writer.WriteLine("envAnimation:" + environmentAnimation);

            for (int i = 0; i < keyBindings.Length; ++i)
            {
                writer.WriteLine("key_" + keyBindings[i].keyDescription + ":" + keyBindings[i].keyCode);
            }

            writer.Close();
        }
        catch (System.Exception exception)
        {
            Log.Error($"Failed to save options: {exception.Message}");
        }
    }
}
