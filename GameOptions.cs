using betareborn.Client.Input;
using betareborn.Client.Resource.Language;
using betareborn.Stats;
using java.io;
using java.lang;

namespace betareborn
{
    public class GameOptions : java.lang.Object
    {
        private static readonly string[] RENDER_DISTANCES = ["options.renderDistance.far", "options.renderDistance.normal", "options.renderDistance.short", "options.renderDistance.tiny"];
        private static readonly string[] DIFFICULTIES = ["options.difficulty.peaceful", "options.difficulty.easy", "options.difficulty.normal", "options.difficulty.hard"];
        private static readonly string[] GUISCALES = ["options.guiScale.auto", "options.guiScale.small", "options.guiScale.normal", "options.guiScale.large"];
        // private static readonly string[] LIMIT_FRAMERATES = ["performance.max", "performance.balanced", "performance.powersaver"];
        private static readonly string[] ANISO_LEVELS = ["options.off", "2x", "4x", "8x", "16x"];
        private static readonly string[] MSAA_LEVELS = ["options.off", "2x", "4x", "8x"];
        public static float MaxAnisotropy = 1.0f;
        public float musicVolume = 1.0F;
        public float soundVolume = 1.0F;
        public float mouseSensitivity = 0.5F;
        public bool invertMouse = false;
        public int renderDistance = 0;
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
        public bool field_22275_C = false;
        public bool smoothCamera = false;
        public bool debugCamera = false;
        public float field_22272_F = 1.0F;
        public float field_22271_G = 1.0F;
        public int guiScale = 0;
        public int anisotropicLevel = 0;
        public int msaaLevel = 0;
        public int INITIAL_MSAA = 0;
        public bool useMipmaps = true;
        public bool debugMode = false;
        public bool environmentAnimation = true;

        public GameOptions(Minecraft var1, java.io.File var2)
        {
            keyBindings = [keyBindForward, keyBindLeft, keyBindBack, keyBindRight, keyBindJump, keyBindSneak, keyBindDrop, keyBindInventory, keyBindChat, keyBindToggleFog];
            mc = var1;
            optionsFile = new java.io.File(var2, "options.txt");
            loadOptions();
            INITIAL_MSAA = msaaLevel;
        }

        public GameOptions()
        {
        }

        public string getKeyBindingDescription(int var1)
        {
            TranslationStorage var2 = TranslationStorage.getInstance();
            return var2.translateKey(keyBindings[var1].keyDescription);
        }

        public string getOptionDisplayString(int var1)
        {
            return Keyboard.getKeyName(keyBindings[var1].keyCode);
        }

        public void setKeyBinding(int var1, int var2)
        {
            keyBindings[var1].keyCode = var2;
            saveOptions();
        }

        public void setOptionFloatValue(EnumOptions options, float value)
        {
            if (options == EnumOptions.MUSIC)
            {
                musicVolume = value;
                mc.sndManager.onSoundOptionsChanged();
            }

            if (options == EnumOptions.SOUND)
            {
                soundVolume = value;
                mc.sndManager.onSoundOptionsChanged();
            }

            if (options == EnumOptions.FRAMERATE_LIMIT)
            {
                limitFramerate = value;
            }

            if (options == EnumOptions.FOV)
            {
                fov = value;
            }

            if (options == EnumOptions.SENSITIVITY)
            {
                mouseSensitivity = value;
            }
        }

        public void setOptionValue(EnumOptions options, int value)
        {
            if (options == EnumOptions.INVERT_MOUSE)
            {
                invertMouse = !invertMouse;
            }

            if (options == EnumOptions.RENDER_DISTANCE)
            {
                renderDistance = renderDistance + value & 3;
            }

            if (options == EnumOptions.GUI_SCALE)
            {
                guiScale = guiScale + value & 3;
            }

            if (options == EnumOptions.VIEW_BOBBING)
            {
                viewBobbing = !viewBobbing;
            }

            // if (var1 == EnumOptions.FRAMERATE_LIMIT)
            // {
            //     limitFramerate = (limitFramerate + var2 + 3) % 3;
            // }

            if (options == EnumOptions.DIFFICULTY)
            {
                difficulty = difficulty + value & 3;
            }

            if (options == EnumOptions.ANISOTROPIC)
            {
                anisotropicLevel = (anisotropicLevel + value) % 5;
                int val = anisotropicLevel == 0 ? 0 : (int)System.Math.Pow(2, anisotropicLevel);
                if (val > MaxAnisotropy)
                {
                    anisotropicLevel = 0;
                }

                if (Minecraft.INSTANCE?.textureManager != null)
                {
                    Minecraft.INSTANCE.textureManager.reload();
                }
            }

            if (options == EnumOptions.MIPMAPS)
            {
                useMipmaps = !useMipmaps;
                if (Minecraft.INSTANCE?.textureManager != null)
                {
                    Minecraft.INSTANCE.textureManager.reload();
                }
            }

            if (options == EnumOptions.MSAA)
            {
                msaaLevel = (msaaLevel + value) % 4;
            }

            if (options == EnumOptions.DEBUG_MODE)
            {
                debugMode = !debugMode;
                Profiling.Profiler.Enabled = debugMode;
            }

            if (options == EnumOptions.ENVIRONMENT_ANIMATION)
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
            switch (EnumOptionsMappingHelper.enumOptionsMappingHelperArray[option.ordinal()])
            {
                case 1:
                    return invertMouse;
                case 2:
                    return viewBobbing;
                case 3:
                    return useMipmaps;
                case 4:
                    return debugMode;
                case 5:
                    return environmentAnimation;
                default:
                    return false;
            }
        }

        public string getKeyBinding(EnumOptions option)
        {
            TranslationStorage var2 = TranslationStorage.getInstance();
            string var3 = (option == EnumOptions.FRAMERATE_LIMIT ? "Max FPS" : (option == EnumOptions.FOV ? "FOV" : var2.translateKey(option.getEnumString()))) + ": ";
            if (option.getEnumFloat())
            {
                float var5 = getOptionFloatValue(option);
                if (option == EnumOptions.SENSITIVITY)
                {
                    return var5 == 0.0F ? var3 + var2.translateKey("options.sensitivity.min") : (var5 == 1.0F ? var3 + var2.translateKey("options.sensitivity.max") : var3 + (int)(var5 * 200.0F) + "%");
                }
                if (option == EnumOptions.FRAMERATE_LIMIT)
                {
                    int fps = 30 + (int)(var5 * 210.0f);
                    return var3 + (fps == 240 ? "Unlimited" : fps + " FPS");
                }
                if (option == EnumOptions.FOV)
                {
                    int fovVal = 30 + (int)(var5 * 90.0f);
                    return var3 + fovVal;
                }
                return (var5 == 0.0F ? var3 + var2.translateKey("options.off") : var3 + (int)(var5 * 100.0F) + "%");
            }
            else if (option.getEnumBoolean())
            {
                bool var4 = getOptionOrdinalValue(option);
                return var4 ? var3 + var2.translateKey("options.on") : var3 + var2.translateKey("options.off");
            }
            else if (option == EnumOptions.MSAA)
            {
                string label = var3 + (msaaLevel == 0 ? var2.translateKey("options.off") : MSAA_LEVELS[msaaLevel]);
                if (msaaLevel != INITIAL_MSAA)
                {
                    label += " (Reload required)";
                }
                return label;
            }
            else
            {
                return option == EnumOptions.RENDER_DISTANCE ? var3 + var2.translateKey(RENDER_DISTANCES[renderDistance]) : (option == EnumOptions.DIFFICULTY ? var3 + var2.translateKey(DIFFICULTIES[difficulty]) : (option == EnumOptions.GUI_SCALE ? var3 + var2.translateKey(GUISCALES[guiScale]) : (option == EnumOptions.ANISOTROPIC ? var3 + (anisotropicLevel == 0 ? var2.translateKey("options.off") : ANISO_LEVELS[anisotropicLevel]) : var3)));
            }
        }

        public void loadOptions()
        {
            try
            {
                if (!optionsFile.exists())
                {
                    return;
                }

                BufferedReader bufferedReader = new(new FileReader(optionsFile));
                string optionLine = "";

                while (true)
                {
                    optionLine = bufferedReader.readLine();
                    if (optionLine == null)
                    {
                        bufferedReader.close();
                        break;
                    }

                    try
                    {
                        string[] keyValue = optionLine.Split(":");
                        string key = keyValue[0];
                        string value = keyValue[1];
                        if (key.Equals("music"))
                        {
                            musicVolume = parseFloat(value);
                        }

                        if (key.Equals("sound"))
                        {
                            soundVolume = parseFloat(value);
                        }

                        if (key.Equals("mouseSensitivity"))
                        {
                            mouseSensitivity = parseFloat(value);
                        }

                        if (key.Equals("invertYMouse"))
                        {
                            invertMouse = value.Equals("true");
                        }

                        if (key.Equals("viewDistance"))
                        {
                            renderDistance = int.Parse(value);
                        }

                        if (key.Equals("guiScale"))
                        {
                            guiScale = int.Parse(value);
                        }

                        if (key.Equals("bobView"))
                        {
                            viewBobbing = value.Equals("true");
                        }

                        if (key.Equals("fpsLimit"))
                        {
                            limitFramerate = parseFloat(value);
                        }

                        if (key.Equals("fov"))
                        {
                            fov = parseFloat(value);
                        }

                        if (key.Equals("difficulty"))
                        {
                            difficulty = int.Parse(value);
                        }

                        if (key.Equals("skin"))
                        {
                            skin = value;
                        }

                        if (key.Equals("lastServer") && keyValue.Length >= 2)
                        {
                            lastServer = value;
                        }

                        if (key.Equals("anisotropicLevel"))
                        {
                            anisotropicLevel = int.Parse(value);
                        }
                        if (key.Equals("msaaLevel"))
                        {
                            msaaLevel = int.Parse(value);
                            if (msaaLevel > 3) msaaLevel = 3;
                        }

                        if (key.Equals("useMipmaps"))
                        {
                            useMipmaps = value.Equals("true");
                        }

                        if (key.Equals("debugMode"))
                        {
                            debugMode = value.Equals("true");
                        }

                        if (key.Equals("envAnimation"))
                        {
                            environmentAnimation = value.Equals("true");
                        }

                        for (int i = 0; i < keyBindings.Length; ++i)
                        {
                            if (key.Equals("key_" + keyBindings[i].keyDescription))
                            {
                                keyBindings[i].keyCode = int.Parse(value);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine("Skipping bad option: " + optionLine);
                    }
                }
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Failed to load options");
            }

        }

        private float parseFloat(string var1)
        {
            return var1.Equals("true") ? 1.0F : (var1.Equals("false") ? 0.0F : float.Parse(var1));
        }

        public void saveOptions()
        {
            try
            {
                using System.IO.StreamWriter optionsFile = new(this.optionsFile.getAbsolutePath());
                optionsFile.WriteLine("music:" + musicVolume);
                optionsFile.WriteLine("sound:" + soundVolume);
                optionsFile.WriteLine("invertYMouse:" + invertMouse);
                optionsFile.WriteLine("mouseSensitivity:" + mouseSensitivity);
                optionsFile.WriteLine("viewDistance:" + renderDistance);
                optionsFile.WriteLine("guiScale:" + guiScale);
                optionsFile.WriteLine("bobView:" + viewBobbing);
                optionsFile.WriteLine("fpsLimit:" + limitFramerate);
                optionsFile.WriteLine("fov:" + fov);
                optionsFile.WriteLine("difficulty:" + difficulty);
                optionsFile.WriteLine("skin:" + skin);
                optionsFile.WriteLine("lastServer:" + lastServer);
                optionsFile.WriteLine("anisotropicLevel:" + anisotropicLevel);
                optionsFile.WriteLine("msaaLevel:" + msaaLevel);
                optionsFile.WriteLine("useMipmaps:" + useMipmaps);
                optionsFile.WriteLine("debugMode:" + debugMode);
                optionsFile.WriteLine("envAnimation:" + environmentAnimation);

                for (int i = 0; i < keyBindings.Length; ++i)
                {
                    optionsFile.WriteLine("key_" + keyBindings[i].keyDescription + ":" + keyBindings[i].keyCode);
                }

                optionsFile.Close();
            }
            catch (System.Exception var3)
            {
                System.Console.WriteLine("Failed to save options: " + var3.Message);
            }
        }
    }
}