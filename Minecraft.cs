using Avalonia;
using Avalonia.Controls;
using betareborn.Blocks;
using betareborn.Client;
using betareborn.Client.Colors;
using betareborn.Client.Guis;
using betareborn.Client.Input;
using betareborn.Client.Network;
using betareborn.Client.Rendering;
using betareborn.Client.Rendering.Core;
using betareborn.Client.Rendering.Entitys;
using betareborn.Client.Rendering.Items;
using betareborn.Client.Resource;
using betareborn.Client.Resource.Pack;
using betareborn.Client.Sound;
using betareborn.Client.Textures;
using betareborn.Entities;
using betareborn.Items;
using betareborn.Launcher;
using betareborn.Profiling;
using betareborn.Server.Internal;
using betareborn.Stats;
using betareborn.Util.Hit;
using betareborn.Util.Maths;
using betareborn.Worlds;
using betareborn.Worlds.Chunks;
using betareborn.Worlds.Chunks.Storage;
using betareborn.Worlds.Dimensions;
using betareborn.Worlds.Storage;
using ImGuiNET;
using java.lang;
using Silk.NET.Input;
using Silk.NET.OpenGL.Legacy;
using Silk.NET.OpenGL.Legacy.Extensions.ImGui;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace betareborn
{
    public partial class Minecraft : java.lang.Object, Runnable
    {
        public static Minecraft INSTANCE;
        public PlayerController playerController;
        private bool fullscreen = false;
        private bool hasCrashed = false;
        public int displayWidth;
        public int displayHeight;
        private Timer timer = new Timer(20.0F);
        public World world;
        public WorldRenderer terrainRenderer;
        public ClientPlayerEntity player;
        public EntityLiving camera;
        public ParticleManager particleManager;
        public Session session = null;
        public string minecraftUri;
        public bool hideQuitButton = false;
        public volatile bool isGamePaused = false;
        public TextureManager textureManager;
        public TextRenderer fontRenderer;
        public GuiScreen currentScreen = null;
        public LoadingScreenRenderer loadingScreen;

        public GameRenderer gameRenderer;

        //private ThreadDownloadResources downloadResourcesThread;
        private int ticksRan = 0;
        private int leftClickCounter = 0;
        private int tempDisplayWidth;
        private int tempDisplayHeight;
        public GuiAchievement guiAchievement;
        public GuiIngame ingameGUI;
        public bool skipRenderWorld = false;
        public HitResult objectMouseOver = null;
        public GameOptions options;
        public SoundManager sndManager = new SoundManager();
        public MouseHelper mouseHelper;
        public TexturePacks texturePackList;
        private java.io.File mcDataDir;
        private WorldStorageSource saveLoader;
        public static long[] frameTimes = new long[512];
        public static long[] tickTimes = new long[512];
        public static int numRecordedFrameTimes = 0;
        public static long hasPaidCheckTime = 0L;
        public StatFileWriter statFileWriter;
        private string serverName;
        private int serverPort;
        private WaterSprite textureWaterFX = new WaterSprite();
        private LavaSprite textureLavaFX = new LavaSprite();
        private static java.io.File minecraftDir = null;
        public volatile bool running = true;
        public string debug = "";
        bool isTakingScreenshot = false;
        long prevFrameTime = -1L;
        public bool inGameHasFocus = false;
        private int mouseTicksRan = 0;
        public bool isRaining = false;
        long systemTime = java.lang.System.currentTimeMillis();
        private int joinPlayerCounter = 0;
        private ImGuiController imGuiController;
        public InternalServer? internalServer;

        public Minecraft(int var4, int var5, bool var6)
        {
            loadingScreen = new LoadingScreenRenderer(this);
            guiAchievement = new GuiAchievement(this);
            tempDisplayHeight = var5;
            fullscreen = var6;
            displayWidth = var4;
            displayHeight = var5;
            fullscreen = var6;

            INSTANCE = this;
        }

        [LibraryImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static partial uint TimeBeginPeriod(uint period);

        [LibraryImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static partial uint TimeEndPeriod(uint period);

        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public void InitializeTimer()
        {
            if (IsWindows)
            {
                TimeBeginPeriod(1);
            }
        }

        public void CleanupTimer()
        {
            if (IsWindows)
            {
                TimeEndPeriod(1);
            }
        }

        public void onMinecraftCrash(UnexpectedThrowable var1)
        {
            hasCrashed = true;
            displayUnexpectedThrowable(var1);
        }

        public void displayUnexpectedThrowable(UnexpectedThrowable var1)
        {
            var1.exception.printStackTrace();
        }

        public void setServer(string var1, int var2)
        {
            serverName = var1;
            serverPort = var2;
        }

        public unsafe void startGame()
        {
            InitializeTimer();

            if (fullscreen)
            {
                Display.setFullscreen(true);
                displayWidth = Display.getDisplayMode().getWidth();
                displayHeight = Display.getDisplayMode().getHeight();
                if (displayWidth <= 0)
                {
                    displayWidth = 1;
                }

                if (displayHeight <= 0)
                {
                    displayHeight = 1;
                }
            }
            else
            {
                Display.setDisplayMode(new DisplayMode(displayWidth, displayHeight));
            }

            Display.setTitle("Minecraft Beta 1.7.3");

            mcDataDir = getMinecraftDir();
            saveLoader = new RegionWorldStorageSource(new java.io.File(mcDataDir, "saves"), true);
            options = new GameOptions(this, mcDataDir);
            Profiler.Enabled = options.debugMode;

            try
            {
                int[] msaaValues = [0, 2, 4, 8];
                Display.MSAA_Samples = msaaValues[options.msaaLevel];
                Display.create();
                GLManager.Init(Display.getGL()!);
            }
            catch (System.Exception var6)
            {
                Console.WriteLine(var6);
            }
            texturePackList = new TexturePacks(this, mcDataDir);
            textureManager = new TextureManager(texturePackList, options);
            fontRenderer = new TextRenderer(options, textureManager);
            WaterColors.loadColors(textureManager.getColors("/misc/watercolor.png"));
            GrassColors.loadColors(textureManager.getColors("/misc/grasscolor.png"));
            FoliageColors.loadColors(textureManager.getColors("/misc/foliagecolor.png"));
            gameRenderer = new GameRenderer(this);
            EntityRenderDispatcher.instance.heldItemRenderer = new HeldItemRenderer(this);
            statFileWriter = new StatFileWriter(session, mcDataDir);
            Achievements.OPEN_INVENTORY.setStatStringFormatter(new StatStringFormatKeyInv(this));
            loadScreen();

            bool anisotropicFiltering = GLManager.GL.IsExtensionPresent("GL_EXT_texture_filter_anisotropic");
            Console.WriteLine($"Anisotropic Filtering Supported: {anisotropicFiltering}");

            if (anisotropicFiltering)
            {
                GLManager.GL.GetFloat(GLEnum.MaxTextureMaxAnisotropy, out float maxAnisotropy);
                GameOptions.MaxAnisotropy = maxAnisotropy;
                Console.WriteLine($"Max Anisotropy: {maxAnisotropy}");
            }
            else
            {
                GameOptions.MaxAnisotropy = 1.0f;
            }

            try
            {
                var window = Display.getWindow();
                var input = window.CreateInput();
                imGuiController = new(GLManager.GL, window, input);
                imGuiController.MakeCurrent();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Failed to initialize ImGui: " + e);
                imGuiController = null;
            }

            Keyboard.create(Display.getGlfw(), Display.getWindowHandle());
            Mouse.create(Display.getGlfw(), Display.getWindowHandle(), Display.getWidth(), Display.getHeight());
            mouseHelper = new MouseHelper();

            checkGLError("Pre startup");
            GLManager.GL.Enable(GLEnum.Texture2D);
            GLManager.GL.ShadeModel(GLEnum.Smooth);
            GLManager.GL.ClearDepth(1.0D);
            GLManager.GL.Enable(GLEnum.DepthTest);
            GLManager.GL.DepthFunc(GLEnum.Lequal);
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.AlphaFunc(GLEnum.Greater, 0.1F);
            GLManager.GL.CullFace(GLEnum.Back);
            GLManager.GL.MatrixMode(GLEnum.Projection);
            GLManager.GL.LoadIdentity();
            GLManager.GL.MatrixMode(GLEnum.Modelview);
            checkGLError("Startup");
            sndManager.loadSoundSettings(options);
            textureManager.addDynamicTexture(textureLavaFX);
            textureManager.addDynamicTexture(textureWaterFX);
            textureManager.addDynamicTexture(new NetherPortalSprite());
            textureManager.addDynamicTexture(new CompassSprite(this));
            textureManager.addDynamicTexture(new ClockSprite(this));
            textureManager.addDynamicTexture(new WaterSideSprite());
            textureManager.addDynamicTexture(new LavaSideSprite());
            textureManager.addDynamicTexture(new FireSprite(0));
            textureManager.addDynamicTexture(new FireSprite(1));
            terrainRenderer = new WorldRenderer(this, textureManager);
            GLManager.GL.Viewport(0, 0, (uint)displayWidth, (uint)displayHeight);
            particleManager = new ParticleManager(world, textureManager);

            MinecraftResourceDownloader downloader = new(this, minecraftDir.getAbsolutePath());
            _ = downloader.DownloadResourcesAsync();

            checkGLError("Post startup");
            ingameGUI = new GuiIngame(this);

            if (serverName != null)
            {
                displayGuiScreen(new GuiConnecting(this, serverName, serverPort));
            }
            else
            {
                displayGuiScreen(new GuiMainMenu());
            }
        }

        private void loadScreen()
        {
            ScaledResolution var1 = new ScaledResolution(options, displayWidth, displayHeight);
            GLManager.GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GLManager.GL.MatrixMode(GLEnum.Projection);
            GLManager.GL.LoadIdentity();
            GLManager.GL.Ortho(0.0D, var1.field_25121_a, var1.field_25120_b, 0.0D, 1000.0D, 3000.0D);
            GLManager.GL.MatrixMode(GLEnum.Modelview);
            GLManager.GL.LoadIdentity();
            GLManager.GL.Translate(0.0F, 0.0F, -2000.0F);
            GLManager.GL.Viewport(0, 0, (uint)displayWidth, (uint)displayHeight);
            GLManager.GL.ClearColor(0.0F, 0.0F, 0.0F, 0.0F);
            Tessellator var2 = Tessellator.instance;
            GLManager.GL.Disable(GLEnum.Lighting);
            GLManager.GL.Enable(GLEnum.Texture2D);
            GLManager.GL.Disable(GLEnum.Fog);
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)textureManager.getTextureId("/title/mojang.png"));
            var2.startDrawingQuads();
            var2.setColorOpaque_I(16777215);
            var2.addVertexWithUV(0.0D, (double)displayHeight, 0.0D, 0.0D, 0.0D);
            var2.addVertexWithUV((double)displayWidth, (double)displayHeight, 0.0D, 0.0D, 0.0D);
            var2.addVertexWithUV((double)displayWidth, 0.0D, 0.0D, 0.0D, 0.0D);
            var2.addVertexWithUV(0.0D, 0.0D, 0.0D, 0.0D, 0.0D);
            var2.draw();
            short var3 = 256;
            short var4 = 256;
            GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
            var2.setColorOpaque_I(16777215);
            func_6274_a((var1.getScaledWidth() - var3) / 2, (var1.getScaledHeight() - var4) / 2, 0, 0, var3, var4);
            GLManager.GL.Disable(GLEnum.Lighting);
            GLManager.GL.Disable(GLEnum.Fog);
            GLManager.GL.Enable(GLEnum.AlphaTest);
            GLManager.GL.AlphaFunc(GLEnum.Greater, 0.1F);
            Display.swapBuffers();
        }

        public void func_6274_a(int var1, int var2, int var3, int var4, int var5, int var6)
        {
            float var7 = 0.00390625F;
            float var8 = 0.00390625F;
            Tessellator var9 = Tessellator.instance;
            var9.startDrawingQuads();
            var9.addVertexWithUV((double)(var1 + 0), (double)(var2 + var6), 0.0D, (double)((float)(var3 + 0) * var7),
                (double)((float)(var4 + var6) * var8));
            var9.addVertexWithUV((double)(var1 + var5), (double)(var2 + var6), 0.0D,
                (double)((float)(var3 + var5) * var7), (double)((float)(var4 + var6) * var8));
            var9.addVertexWithUV((double)(var1 + var5), (double)(var2 + 0), 0.0D, (double)((float)(var3 + var5) * var7),
                (double)((float)(var4 + 0) * var8));
            var9.addVertexWithUV((double)(var1 + 0), (double)(var2 + 0), 0.0D, (double)((float)(var3 + 0) * var7),
                (double)((float)(var4 + 0) * var8));
            var9.draw();
        }

        public static java.io.File getMinecraftDir()
        {
            if (minecraftDir == null)
            {
                minecraftDir = getAppDir("betareborn");
            }

            return minecraftDir;
        }

        public static java.io.File getAppDir(string var0)
        {
            string var1 = java.lang.System.getProperty("user.home", ".");
            java.io.File var2;
            switch (EnumOSMappingHelper.enumOSMappingArray[(int)getOs()])
            {
                case 1:
                case 2:
                    var2 = new java.io.File(var1, '.' + var0 + '/');
                    break;
                case 3:
                    string var3 = java.lang.System.getenv("APPDATA");
                    if (var3 != null)
                    {
                        var2 = new java.io.File(var3, "." + var0 + '/');
                    }
                    else
                    {
                        var2 = new java.io.File(var1, '.' + var0 + '/');
                    }

                    break;
                case 4:
                    var2 = new java.io.File(var1, "Library/Application Support/" + var0);
                    break;
                default:
                    var2 = new java.io.File(var1, var0 + '/');
                    break;
            }

            if (!var2.exists() && !var2.mkdirs())
            {
                throw new RuntimeException("The working directory could not be created: " + var2);
            }
            else
            {
                return var2;
            }
        }

        private static Util.OperatingSystem getOs()
        {
            string var0 = java.lang.System.getProperty("os.name").ToLower();
            return var0.Contains("win")
                ? Util.OperatingSystem.windows
                : (var0.Contains("mac")
                    ? Util.OperatingSystem.macos
                    : (var0.Contains("solaris")
                        ? Util.OperatingSystem.solaris
                        : (var0.Contains("sunos")
                            ? Util.OperatingSystem.solaris
                            : (var0.Contains("linux")
                                ? Util.OperatingSystem.linux
                                : (var0.Contains("unix") ? Util.OperatingSystem.linux : Util.OperatingSystem.unknown)))));
        }

        public WorldStorageSource getSaveLoader()
        {
            return saveLoader;
        }

        public void displayGuiScreen(GuiScreen var1)
        {
            if (!(currentScreen is GuiUnused))
            {
                if (currentScreen != null)
                {
                    currentScreen.onGuiClosed();
                }

                if (var1 is GuiMainMenu)
                {
                    statFileWriter.func_27175_b();
                }

                statFileWriter.syncStats();
                if (var1 == null && world == null)
                {
                    var1 = new GuiMainMenu();
                }
                else if (var1 == null && player.health <= 0)
                {
                    var1 = new GuiGameOver();
                }

                if (var1 is GuiMainMenu)
                {
                    ingameGUI.clearChatMessages();
                }

                currentScreen = (GuiScreen)var1;
                if (var1 != null)
                {
                    setIngameNotInFocus();
                    ScaledResolution var2 = new ScaledResolution(options, displayWidth, displayHeight);
                    int var3 = var2.getScaledWidth();
                    int var4 = var2.getScaledHeight();
                    ((GuiScreen)var1).setWorldAndResolution(this, var3, var4);
                    skipRenderWorld = false;
                }
                else
                {
                    setIngameFocus();
                }
            }
        }

        [Conditional("DEBUG")]
        private void checkGLError(string var1)
        {
            GLEnum var2 = GLManager.GL.GetError();
            if (var2 != 0)
            {
                Console.WriteLine($"#### GL ERROR ####");
                Console.WriteLine($"@ {var1}");
                Console.WriteLine($"> {var2.ToString()}");
                Console.WriteLine($"");
            }
        }

        public void shutdownMinecraftApplet()
        {
            try
            {
                stopInternalServer();
                statFileWriter.func_27175_b();
                statFileWriter.syncStats();

                java.lang.System.@out.println("Stopping!");

                try
                {
                    changeWorld1((World)null);
                }
                catch (Throwable var8)
                {
                }

                try
                {
                    GLAllocation.deleteTexturesAndDisplayLists();
                }
                catch (Throwable var7)
                {
                }

                sndManager.closeMinecraft();
                Mouse.destroy();
                Keyboard.destroy();
            }
            finally
            {
                Display.destroy();
                CleanupTimer();

                if (!hasCrashed)
                {
                    java.lang.System.exit(0);
                }
            }

            java.lang.System.gc();
        }

        public void run()
        {
            running = true;

            try
            {
                startGame();
            }
            catch (java.lang.Exception var17)
            {
                var17.printStackTrace();
                onMinecraftCrash(new UnexpectedThrowable("Failed to start game", var17));
                return;
            }

            try
            {
                long var1 = java.lang.System.currentTimeMillis();
                int var3 = 0;

                while (running)
                {
                    if (options.debugMode)
                    {
                        Profiler.Update(timer.deltaTime);
                        Profiler.Record("frame Time", timer.deltaTime * 1000.0f);
                        Profiler.PushGroup("run");
                    }
                    try
                    {
                        if (Display.isCloseRequested())
                        {
                            shutdown();
                        }

                        if (isGamePaused && world != null)
                        {
                            float var4 = timer.renderPartialTicks;
                            timer.updateTimer();
                            timer.renderPartialTicks = var4;
                        }
                        else
                        {
                            timer.updateTimer();
                        }

                        long var23 = java.lang.System.nanoTime();
                        if (options.debugMode)
                        {
                            Profiler.PushGroup("runTicks");
                        }

                        for (int var6 = 0; var6 < timer.elapsedTicks; ++var6)
                        {
                            ++ticksRan;

                            try
                            {
                                runTick(timer.renderPartialTicks);
                            }
                            catch (MinecraftException var16)
                            {
                                world = null;
                                changeWorld1((World)null);
                                displayGuiScreen(new GuiConflictWarning());
                            }
                        }

                        if (options.debugMode)
                        {
                            Profiler.PopGroup();
                        }

                        long var24 = java.lang.System.nanoTime() - var23;
                        checkGLError("Pre render");
                        BlockRenderer.fancyGrass = true;
                        sndManager.updateListener(player, timer.renderPartialTicks);
                        GLManager.GL.Enable(GLEnum.Texture2D);
                        if (world != null)
                        {
                            if (options.debugMode) Profiler.Start("updateLighting");
                            world.doLightingUpdates();
                            if (options.debugMode) Profiler.Stop("updateLighting");
                        }

                        if (!Keyboard.isKeyDown(Keyboard.KEY_F7))
                        {
                            Display.update();
                        }

                        if (player != null && player.isInsideWall())
                        {
                            options.thirdPersonView = false;
                        }

                        if (!skipRenderWorld)
                        {
                            if (playerController != null)
                            {
                                playerController.setPartialTime(timer.renderPartialTicks);
                            }

                            if (options.debugMode) Profiler.PushGroup("render");
                            gameRenderer.onFrameUpdate(timer.renderPartialTicks);
                            if (options.debugMode) Profiler.PopGroup();
                        }

                        if (imGuiController != null && timer.deltaTime > 0.0f && options.showDebugInfo && options.debugMode)
                        {
                            imGuiController.Update(timer.deltaTime);
                            ProfilerRenderer.Draw();
                            ProfilerRenderer.DrawGraph();

                            ImGui.Begin("Region Info");
                            long rls = Region.RegionCache.getRegionsLoadedSync();
                            long rla = Region.RegionCache.getRegionsLoadedAsync();
                            ImGui.Text($"Regions loaded sync: {rls}");
                            ImGui.Text($"Regions loaded async: {rla}");
                            ImGui.Text($"Regions loaded total: {rls + rla}");
                            ImGui.End();

                            ImGui.Begin("IO");
                            ImGui.Text($"Async IO ops: {AsyncIO.activeTaskCount()}");
                            ImGui.End();

                            ImGui.Begin("Render Info");
                            ImGui.Text($"Chunk Vertex Buffer Allocated MB: {VertexBuffer<ChunkVertex>.Allocated / 1000000.0}");
                            ImGui.End();

                            imGuiController.Render();
                        }

                        if (!Display.isActive())
                        {
                            if (fullscreen)
                            {
                                toggleFullscreen();
                            }

                            java.lang.Thread.sleep(10L);
                        }

                        if (options.showDebugInfo)
                        {
                            displayDebugInfo(var24);
                        }
                        else
                        {
                            prevFrameTime = java.lang.System.nanoTime();
                        }

                        guiAchievement.updateAchievementWindow();

                        if (Keyboard.isKeyDown(Keyboard.KEY_F7))
                        {
                            Display.update();
                        }

                        screenshotListener();

                        if (Display.wasResized())
                        {
                            displayWidth = Display.getWidth();
                            displayHeight = Display.getHeight();
                            if (displayWidth <= 0)
                            {
                                displayWidth = 1;
                            }

                            if (displayHeight <= 0)
                            {
                                displayHeight = 1;
                            }

                            resize(displayWidth, displayHeight);
                        }

                        checkGLError("Post render");
                        ++var3;

                        for (isGamePaused = !isMultiplayerWorld() && currentScreen != null &&
                                            currentScreen.doesGuiPauseGame();
                             java.lang.System.currentTimeMillis() >= var1 + 1000L;
                             var3 = 0)
                        {
                            debug = var3 + " fps, "/* + WorldRenderer.chunksUpdated*/ + "0 chunk updates";
                            //WorldRenderer.chunksUpdated = 0;
                            var1 += 1000L;
                        }
                    }
                    catch (MinecraftException var18)
                    {
                        world = null;
                        changeWorld1((World)null);
                        displayGuiScreen(new GuiConflictWarning());
                    }
                    catch (OutOfMemoryError var19)
                    {
                        crashCleanup();
                        displayGuiScreen(new GuiErrorScreen());
                        java.lang.System.gc();
                    }
                    finally
                    {
                        if (options.debugMode)
                        {
                            Profiler.CaptureFrame();
                            Profiler.PopGroup();
                        }
                    }
                }
            }
            catch (MinecraftError var20)
            {
            }
            catch (Throwable var21)
            {
                crashCleanup();
                var21.printStackTrace();
                onMinecraftCrash(new UnexpectedThrowable("Unexpected error", var21));
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                shutdownMinecraftApplet();
            }
        }

        public void crashCleanup()
        {
            try
            {
                java.lang.System.gc();
            }
            catch (Throwable var3)
            {
            }

            try
            {
                java.lang.System.gc();
                changeWorld1((World)null);
            }
            catch (Throwable var2)
            {
            }

            java.lang.System.gc();
        }

        private void screenshotListener()
        {
            if (Keyboard.isKeyDown(Keyboard.KEY_F2))
            {
                if (!isTakingScreenshot)
                {
                    isTakingScreenshot = true;
                    ingameGUI.addChatMessage(ScreenShotHelper.saveScreenshot(minecraftDir, displayWidth,
                        displayHeight));
                }
            }
            else
            {
                isTakingScreenshot = false;
            }
        }

        private void displayDebugInfo(long var1)
        {
            long var3 = 16666666L;
            if (prevFrameTime == -1L)
            {
                prevFrameTime = java.lang.System.nanoTime();
            }

            long var5 = java.lang.System.nanoTime();
            tickTimes[numRecordedFrameTimes & frameTimes.Length - 1] = var1;
            frameTimes[numRecordedFrameTimes++ & frameTimes.Length - 1] = var5 - prevFrameTime;
            prevFrameTime = var5;
            GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
            GLManager.GL.MatrixMode(GLEnum.Projection);
            GLManager.GL.LoadIdentity();
            GLManager.GL.Ortho(0.0D, (double)displayWidth, (double)displayHeight, 0.0D, 1000.0D, 3000.0D);
            GLManager.GL.MatrixMode(GLEnum.Modelview);
            GLManager.GL.LoadIdentity();
            GLManager.GL.Translate(0.0F, 0.0F, -2000.0F);
            GLManager.GL.LineWidth(1.0F);
            GLManager.GL.Disable(GLEnum.Texture2D);
            Tessellator var7 = Tessellator.instance;
            var7.startDrawing(7);
            int var8 = (int)(var3 / 200000L);
            var7.setColorOpaque_I(536870912);
            var7.addVertex(0.0D, (double)(displayHeight - var8), 0.0D);
            var7.addVertex(0.0D, (double)displayHeight, 0.0D);
            var7.addVertex((double)frameTimes.Length, (double)displayHeight, 0.0D);
            var7.addVertex((double)frameTimes.Length, (double)(displayHeight - var8), 0.0D);
            var7.setColorOpaque_I(538968064);
            var7.addVertex(0.0D, (double)(displayHeight - var8 * 2), 0.0D);
            var7.addVertex(0.0D, (double)(displayHeight - var8), 0.0D);
            var7.addVertex((double)frameTimes.Length, (double)(displayHeight - var8), 0.0D);
            var7.addVertex((double)frameTimes.Length, (double)(displayHeight - var8 * 2), 0.0D);
            var7.draw();
            long var9 = 0L;

            int var11;
            for (var11 = 0; var11 < frameTimes.Length; ++var11)
            {
                var9 += frameTimes[var11];
            }

            var11 = (int)(var9 / 200000L / (long)frameTimes.Length);
            var7.startDrawing(7);
            var7.setColorOpaque_I(541065216);
            var7.addVertex(0.0D, (double)(displayHeight - var11), 0.0D);
            var7.addVertex(0.0D, (double)displayHeight, 0.0D);
            var7.addVertex((double)frameTimes.Length, (double)displayHeight, 0.0D);
            var7.addVertex((double)frameTimes.Length, (double)(displayHeight - var11), 0.0D);
            var7.draw();
            var7.startDrawing(1);

            for (int var12 = 0; var12 < frameTimes.Length; ++var12)
            {
                int var13 = (var12 - numRecordedFrameTimes & frameTimes.Length - 1) * 255 / frameTimes.Length;
                int var14 = var13 * var13 / 255;
                var14 = var14 * var14 / 255;
                int var15 = var14 * var14 / 255;
                var15 = var15 * var15 / 255;
                if (frameTimes[var12] > var3)
                {
                    var7.setColorOpaque_I(-16777216 + var14 * 65536);
                }
                else
                {
                    var7.setColorOpaque_I(-16777216 + var14 * 256);
                }

                long var16 = frameTimes[var12] / 200000L;
                long var18 = tickTimes[var12] / 200000L;
                var7.addVertex((double)((float)var12 + 0.5F), (double)((float)((long)displayHeight - var16) + 0.5F),
                    0.0D);
                var7.addVertex((double)((float)var12 + 0.5F), (double)((float)displayHeight + 0.5F), 0.0D);
                var7.setColorOpaque_I(-16777216 + var14 * 65536 + var14 * 256 + var14 * 1);
                var7.addVertex((double)((float)var12 + 0.5F), (double)((float)((long)displayHeight - var16) + 0.5F),
                    0.0D);
                var7.addVertex((double)((float)var12 + 0.5F),
                    (double)((float)((long)displayHeight - (var16 - var18)) + 0.5F), 0.0D);
            }

            var7.draw();
            GLManager.GL.Enable(GLEnum.Texture2D);
        }

        public void shutdown()
        {
            running = false;
        }

        public void setIngameFocus()
        {
            if (Display.isActive())
            {
                if (!inGameHasFocus)
                {
                    inGameHasFocus = true;
                    mouseHelper.grabMouseCursor();
                    displayGuiScreen((GuiScreen)null);
                    leftClickCounter = 10000;
                    mouseTicksRan = ticksRan + 10000;
                }
            }
        }

        public void stopInternalServer()
        {
            if (internalServer != null)
            {
                internalServer.stop();
                while (!internalServer.stopped)
                {
                    System.Threading.Thread.Sleep(1);
                }
                internalServer = null;
            }
        }

        public void setIngameNotInFocus()
        {
            if (inGameHasFocus)
            {
                if (player != null)
                {
                    player.resetPlayerKeyState();
                }

                inGameHasFocus = false;
                mouseHelper.ungrabMouseCursor();
            }
        }

        public void displayInGameMenu()
        {
            if (currentScreen == null)
            {
                displayGuiScreen(new GuiIngameMenu());
            }
        }

        private void func_6254_a(int var1, bool var2)
        {
            if (!playerController.field_1064_b)
            {
                if (!var2)
                {
                    leftClickCounter = 0;
                }

                if (var1 != 0 || leftClickCounter <= 0)
                {
                    if (var2 && objectMouseOver != null && objectMouseOver.type == HitResultType.TILE &&
                        var1 == 0)
                    {
                        int var3 = objectMouseOver.blockX;
                        int var4 = objectMouseOver.blockY;
                        int var5 = objectMouseOver.blockZ;
                        playerController.sendBlockRemoving(var3, var4, var5, objectMouseOver.side);
                        particleManager.addBlockHitEffects(var3, var4, var5, objectMouseOver.side);
                    }
                    else
                    {
                        playerController.resetBlockRemoving();
                    }
                }
            }
        }

        private void clickMouse(int var1)
        {
            if (var1 != 0 || leftClickCounter <= 0)
            {
                if (var1 == 0)
                {
                    player.swingHand();
                }

                bool var2 = true;
                if (objectMouseOver == null)
                {
                    if (var1 == 0 && !(playerController is PlayerControllerTest))
                    {
                        leftClickCounter = 10;
                    }
                }
                else if (objectMouseOver.type == HitResultType.ENTITY)
                {
                    if (var1 == 0)
                    {
                        playerController.attackEntity(player, objectMouseOver.entity);
                    }

                    if (var1 == 1)
                    {
                        playerController.interactWithEntity(player, objectMouseOver.entity);
                    }
                }
                else if (objectMouseOver.type == HitResultType.TILE)
                {
                    int var3 = objectMouseOver.blockX;
                    int var4 = objectMouseOver.blockY;
                    int var5 = objectMouseOver.blockZ;
                    int var6 = objectMouseOver.side;
                    if (var1 == 0)
                    {
                        playerController.clickBlock(var3, var4, var5, objectMouseOver.side);
                    }
                    else
                    {
                        ItemStack var7 = player.inventory.getSelectedItem();
                        int var8 = var7 != null ? var7.count : 0;
                        if (playerController.sendPlaceBlock(player, world, var7, var3, var4, var5, var6))
                        {
                            var2 = false;
                            player.swingHand();
                        }

                        if (var7 == null)
                        {
                            return;
                        }

                        if (var7.count == 0)
                        {
                            player.inventory.main[player.inventory.selectedSlot] = null;
                        }
                        else if (var7.count != var8)
                        {
                            gameRenderer.itemRenderer.func_9449_b();
                        }
                    }
                }

                if (var2 && var1 == 1)
                {
                    ItemStack var9 = player.inventory.getSelectedItem();
                    if (var9 != null && playerController.sendUseItem(player, world, var9))
                    {
                        gameRenderer.itemRenderer.func_9450_c();
                    }
                }
            }
        }

        public void toggleFullscreen()
        {
            try
            {
                fullscreen = !fullscreen;
                if (fullscreen)
                {
                    Display.setDisplayMode(Display.getDesktopDisplayMode());
                    displayWidth = Display.getDisplayMode().getWidth();
                    displayHeight = Display.getDisplayMode().getHeight();
                    if (displayWidth <= 0)
                    {
                        displayWidth = 1;
                    }

                    if (displayHeight <= 0)
                    {
                        displayHeight = 1;
                    }
                }
                else
                {
                    displayWidth = tempDisplayWidth;
                    displayHeight = tempDisplayHeight;
                    if (displayWidth <= 0)
                    {
                        displayWidth = 1;
                    }

                    if (displayHeight <= 0)
                    {
                        displayHeight = 1;
                    }
                }

                if (currentScreen != null)
                {
                    resize(displayWidth, displayHeight);
                }

                Display.setFullscreen(fullscreen);
                Display.update();
            }
            catch (System.Exception var2)
            {
                Console.WriteLine(var2);
            }
        }

        private void resize(int var1, int var2)
        {
            if (var1 <= 0)
            {
                var1 = 1;
            }

            if (var2 <= 0)
            {
                var2 = 1;
            }

            displayWidth = var1;
            displayHeight = var2;
            Mouse.setDisplayDimensions(displayWidth, displayHeight);

            if (currentScreen != null)
            {
                ScaledResolution var3 = new ScaledResolution(options, var1, var2);
                int var4 = var3.getScaledWidth();
                int var5 = var3.getScaledHeight();
                currentScreen.setWorldAndResolution(this, var4, var5);
            }
        }

        private void clickMiddleMouseButton()
        {
            if (objectMouseOver != null)
            {
                int var1 = world.getBlockId(objectMouseOver.blockX, objectMouseOver.blockY, objectMouseOver.blockZ);
                if (var1 == Block.GRASS_BLOCK.id)
                {
                    var1 = Block.DIRT.id;
                }

                if (var1 == Block.DOUBLE_SLAB.id)
                {
                    var1 = Block.SLAB.id;
                }

                if (var1 == Block.BEDROCK.id)
                {
                    var1 = Block.STONE.id;
                }

                player.inventory.setCurrentItem(var1, playerController is PlayerControllerTest);
            }
        }

        public void runTick(float partialTicks)
        {
            Profiler.PushGroup("runTick");

            Profiler.Start("statFileWriter.func_27178_d");
            statFileWriter.func_27178_d();
            Profiler.Stop("statFileWriter.func_27178_d");
            Profiler.Start("ingameGUI.updateTick");
            ingameGUI.updateTick();
            Profiler.Stop("ingameGUI.updateTick");
            gameRenderer.updateTargetedEntity(1.0F);

            AsyncIO.tick();

            gameRenderer.tick(partialTicks);

            Profiler.Start("chunkProviderLoadOrGenerateSetCurrentChunkOver");

            Profiler.Stop("chunkProviderLoadOrGenerateSetCurrentChunkOver");

            Profiler.Start("playerControllerUpdate");
            if (!isGamePaused && world != null)
            {
                playerController.updateController();
            }

            Profiler.Stop("playerControllerUpdate");

            Profiler.Start("updateDynamicTextures");
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)textureManager.getTextureId("/terrain.png"));
            if (!isGamePaused)
            {
                textureManager.tick();
            }

            Profiler.Stop("updateDynamicTextures");

            if (currentScreen == null && player != null)
            {
                if (player.health <= 0)
                {
                    displayGuiScreen((GuiScreen)null);
                }
                else if (player.isSleeping() && world != null && world.isRemote)
                {
                    displayGuiScreen(new GuiSleepMP());
                }
            }
            else if (currentScreen != null && currentScreen is GuiSleepMP && !player.isSleeping())
            {
                displayGuiScreen((GuiScreen)null);
            }

            if (currentScreen != null)
            {
                leftClickCounter = 10000;
                mouseTicksRan = ticksRan + 10000;
            }

            if (currentScreen != null)
            {
                currentScreen.handleInput();
                if (currentScreen != null)
                {
                    currentScreen.particlesGui.func_25088_a();
                    currentScreen.updateScreen();
                }
            }

            if (currentScreen == null || currentScreen.field_948_f)
            {
                processInputEvents();
            }

            if (world != null)
            {
                if (player != null)
                {
                    ++joinPlayerCounter;
                    if (joinPlayerCounter == 30)
                    {
                        joinPlayerCounter = 0;
                        world.loadChunksNearEntity(player);
                    }
                }

                world.difficulty = options.difficulty;
                if (world.isRemote)
                {
                    world.difficulty = 3;
                }

                Profiler.Start("entityRendererUpdate");
                if (!isGamePaused)
                {
                    gameRenderer.updateCamera();
                }

                Profiler.Stop("entityRendererUpdate");

                if (!isGamePaused)
                {
                    terrainRenderer.updateClouds();
                }

                Profiler.PushGroup("theWorldUpdateEntities");
                if (!isGamePaused)
                {
                    if (world.lightningTicksLeft > 0)
                    {
                        --world.lightningTicksLeft;
                    }

                    world.tickEntities();
                }

                Profiler.PopGroup();

                Profiler.PushGroup("theWorld.tick");
                if (!isGamePaused || isMultiplayerWorld())
                {
                    world.allowSpawning(options.difficulty > 0, true);
                    var renderDistance = options.renderDistance switch
                    {
                        0 => 16,
                        1 => 8,
                        2 => 4,
                        3 => 2,
                        _ => 999,
                    };
                    world.tick(renderDistance);
                }

                Profiler.PopGroup();

                if (!isGamePaused && world != null)
                {
                    world.displayTick(MathHelper.floor_double(player.x),
                        MathHelper.floor_double(player.y), MathHelper.floor_double(player.z));
                }

                if (!isGamePaused)
                {
                    particleManager.updateEffects();
                }
            }

            systemTime = java.lang.System.currentTimeMillis();
            Profiler.PopGroup();
        }

        private void processInputEvents()
        {
            while (Mouse.next())
            {
                long var5 = java.lang.System.currentTimeMillis() - systemTime;
                if (var5 <= 200L)
                {
                    int var3 = Mouse.getEventDWheel();
                    if (var3 != 0)
                    {
                        player.inventory.changeCurrentItem(var3);
                        if (options.field_22275_C)
                        {
                            if (var3 > 0)
                            {
                                var3 = 1;
                            }

                            if (var3 < 0)
                            {
                                var3 = -1;
                            }

                            options.field_22272_F += (float)var3 * 0.25F;
                        }
                    }

                    if (currentScreen == null)
                    {
                        if (!inGameHasFocus && Mouse.getEventButtonState())
                        {
                            setIngameFocus();
                        }
                        else
                        {
                            if (Mouse.getEventButton() == 0 && Mouse.getEventButtonState())
                            {
                                clickMouse(0);
                                mouseTicksRan = ticksRan;
                            }

                            if (Mouse.getEventButton() == 1 && Mouse.getEventButtonState())
                            {
                                clickMouse(1);
                                mouseTicksRan = ticksRan;
                            }

                            if (Mouse.getEventButton() == 2 && Mouse.getEventButtonState())
                            {
                                clickMiddleMouseButton();
                            }
                        }
                    }
                    else if (currentScreen != null)
                    {
                        currentScreen.handleMouseInput();
                    }
                }
            }

            if (leftClickCounter > 0)
            {
                --leftClickCounter;
            }

            while (Keyboard.next())
            {
                player.handleKeyPress(Keyboard.getEventKey(), Keyboard.getEventKeyState());

                if (Keyboard.getEventKeyState())
                {
                    if (Keyboard.getEventKey() == Keyboard.KEY_F11)
                    {
                        toggleFullscreen();
                    }
                    else
                    {
                        if (currentScreen != null)
                        {
                            currentScreen.handleKeyboardInput();
                        }
                        else
                        {
                            if (Keyboard.getEventKey() == Keyboard.KEY_ESCAPE)
                            {
                                displayInGameMenu();
                            }

                            if (Keyboard.getEventKey() == Keyboard.KEY_S && Keyboard.isKeyDown(Keyboard.KEY_F3))
                            {
                                forceReload();
                            }

                            if (Keyboard.getEventKey() == Keyboard.KEY_F1)
                            {
                                options.hideGUI = !options.hideGUI;
                            }

                            if (Keyboard.getEventKey() == Keyboard.KEY_F3)
                            {
                                options.showDebugInfo = !options.showDebugInfo;
                            }

                            if (Keyboard.getEventKey() == Keyboard.KEY_F5)
                            {
                                options.thirdPersonView = !options.thirdPersonView;
                            }

                            if (Keyboard.getEventKey() == Keyboard.KEY_F8)
                            {
                                options.smoothCamera = !options.smoothCamera;
                            }

                            if (Keyboard.getEventKey() == options.keyBindInventory.keyCode)
                            {
                                displayGuiScreen(new GuiInventory(player));
                            }

                            if (Keyboard.getEventKey() == options.keyBindDrop.keyCode)
                            {
                                player.dropSelectedItem();
                            }

                            if (Keyboard.getEventKey() == options.keyBindChat.keyCode)
                            {
                                displayGuiScreen(new GuiChat());
                            }

                            if (Keyboard.getEventKey() == options.keyBindCommand.keyCode)
                            {
                                displayGuiScreen(new GuiChat("/"));
                            }
                        }

                        for (int var6 = 0; var6 < 9; ++var6)
                        {
                            if (Keyboard.getEventKey() == Keyboard.KEY_1 + var6)
                            {
                                player.inventory.selectedSlot = var6;
                            }
                        }

                        if (Keyboard.getEventKey() == options.keyBindToggleFog.keyCode)
                        {
                            options.setOptionValue(EnumOptions.RENDER_DISTANCE,
                                !Keyboard.isKeyDown(Keyboard.KEY_LSHIFT) && !Keyboard.isKeyDown(Keyboard.KEY_RSHIFT) ? 1 : -1);
                        }
                    }
                }
            }

            if (currentScreen == null)
            {
                if (Mouse.isButtonDown(0) && (float)(ticksRan - mouseTicksRan) >= timer.ticksPerSecond / 4.0F &&
                    inGameHasFocus)
                {
                    clickMouse(0);
                    mouseTicksRan = ticksRan;
                }

                if (Mouse.isButtonDown(1) && (float)(ticksRan - mouseTicksRan) >= timer.ticksPerSecond / 4.0F &&
                    inGameHasFocus)
                {
                    clickMouse(1);
                    mouseTicksRan = ticksRan;
                }
            }

            func_6254_a(0, currentScreen == null && Mouse.isButtonDown(0) && inGameHasFocus);
        }

        private void forceReload()
        {
            java.lang.System.@out.println("FORCING RELOAD!");
            sndManager = new SoundManager();
            sndManager.loadSoundSettings(options);
            //downloadResourcesThread.reloadResources();
        }

        public bool isMultiplayerWorld()
        {
            return world != null && world.isRemote;
        }

        public void startWorld(string var1, string var2, long var3)
        {
            changeWorld1((World)null);
            java.lang.System.gc();
            displayGuiScreen(new GuiLevelLoading(var1, var2, var3));
        }


        public void usePortal()
        {
            java.lang.System.@out.println("Toggling dimension!!");
            if (player.dimensionId == -1)
            {
                player.dimensionId = 0;
            }
            else
            {
                player.dimensionId = -1;
            }

            world.remove(player);
            player.dead = false;
            double var1 = player.x;
            double var3 = player.z;
            double var5 = 8.0D;
            World var7;
            if (player.dimensionId == -1)
            {
                var1 /= var5;
                var3 /= var5;
                player.setPositionAndAnglesKeepPrevAngles(var1, player.y, var3, player.yaw,
                    player.pitch);
                if (player.isAlive())
                {
                    world.updateEntity(player, false);
                }

                var7 = null;
                var7 = new World(world, Dimension.fromId(-1));
                changeWorld(var7, "Entering the Nether", player);
            }
            else
            {
                var1 *= var5;
                var3 *= var5;
                player.setPositionAndAnglesKeepPrevAngles(var1, player.y, var3, player.yaw,
                    player.pitch);
                if (player.isAlive())
                {
                    world.updateEntity(player, false);
                }

                var7 = null;
                var7 = new World(world, Dimension.fromId(0));
                changeWorld(var7, "Leaving the Nether", player);
            }

            player.world = world;
            if (player.isAlive())
            {
                player.setPositionAndAnglesKeepPrevAngles(var1, player.y, var3, player.yaw,
                    player.pitch);
                world.updateEntity(player, false);
                (new PortalForcer()).moveToPortal(world, player);
            }
        }

        public void changeWorld1(World var1)
        {
            changeWorld2(var1, "");
        }

        public void changeWorld2(World var1, string var2)
        {
            changeWorld(var1, var2, (EntityPlayer)null);
        }

        private enum BlockedReason
        {
            Chunks,
            Level
        }

        private static bool isBlocked(out BlockedReason reason, out int toSave)
        {
            bool blockedByChunks = Region.RegionCache.isBlocked(out toSave);

            if (blockedByChunks)
            {
                reason = BlockedReason.Chunks;
                return true;
            }

            bool blockedByLevel = AsyncIO.isBlocked();

            if (blockedByLevel)
            {
                reason = BlockedReason.Level;
                return true;
            }

            toSave = 0;
            reason = BlockedReason.Chunks;
            return false;
        }

        public void changeWorld(World var1, string var2, EntityPlayer var3)
        {
            statFileWriter.func_27175_b();
            statFileWriter.syncStats();
            camera = null;
            loadingScreen.printText(var2);
            loadingScreen.progressStage("");
            sndManager.playStreaming((string)null, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F);

            if (world != null)
            {
                world.savingProgress(loadingScreen);

                while (true)
                {
                    bool blocked = isBlocked(out BlockedReason reason, out int toSave);

                    if (!blocked)
                    {
                        break;
                    }

                    loadingScreen.printText(var2);

                    string loadingString = reason == BlockedReason.Chunks
                        ? $"Saving chunks ({toSave} left)"
                        : "Saving level data";

                    loadingScreen.progressStage(loadingString);

                    java.lang.Thread.sleep(33);
                }

                Console.WriteLine("Saved chunks");
                saveLoader.flush();

                Region.RegionCache.deleteSaveHandler();
            }

            world = var1;
            if (var1 != null)
            {
                playerController.func_717_a(var1);
                if (!isMultiplayerWorld())
                {
                    if (var3 == null)
                    {
                        player = (ClientPlayerEntity)var1.getPlayerForProxy(ClientPlayerEntity.Class);
                    }
                }
                else if (player != null)
                {
                    player.teleportToTop();
                    if (var1 != null)
                    {
                        var1.spawnEntity(player);
                    }
                }

                if (!var1.isRemote)
                {
                    func_6255_d(var2);
                }

                if (player == null)
                {
                    player = (ClientPlayerEntity)playerController.createPlayer(var1);
                    player.teleportToTop();
                    playerController.flipPlayer(player);
                }

                player.movementInput = new MovementInputFromOptions(options);
                if (terrainRenderer != null)
                {
                    terrainRenderer.changeWorld(var1);
                }

                if (particleManager != null)
                {
                    particleManager.clearEffects(var1);
                }

                playerController.func_6473_b(player);
                if (var3 != null)
                {
                    var1.saveWorldData();
                }

                var1.addPlayer(player);
                if (var1.isNewWorld)
                {
                    var1.savingProgress(loadingScreen);
                }

                camera = player;
            }
            else
            {
                player = null;
            }

            java.lang.System.gc();
            systemTime = 0L;
        }



        private void func_6255_d(string var1)
        {
            loadingScreen.printText(var1);
            loadingScreen.progressStage("Building terrain");
            short var2 = 128;
            int var3 = 0;
            int var4 = var2 * 2 / 16 + 1;
            var4 *= var4;
            ChunkSource var5 = world.getChunkSource();
            Vec3i var6 = world.getSpawnPos();
            if (player != null)
            {
                var6.x = (int)player.x;
                var6.z = (int)player.z;
            }

            for (int var10 = -var2; var10 <= var2; var10 += 16)
            {
                for (int var8 = -var2; var8 <= var2; var8 += 16)
                {
                    loadingScreen.setLoadingProgress(var3++ * 100 / var4);
                    world.getBlockId(var6.x + var10, 64, var6.z + var8);

                    while (world.doLightingUpdates())
                    {
                    }
                }
            }

            loadingScreen.progressStage("Simulating world for a bit");
            bool var9 = true;
            world.tickChunks();
        }

        public void installResource(string var1, java.io.File var2)
        {
            if (!var2.getPath().EndsWith("ogg"))
            {
                //TODO: ADD SUPPORT FOR MUS SFX?
                return;
            }

            int var3 = var1.IndexOf("/");
            string var4 = var1.Substring(0, var3);
            var1 = var1.Substring(var3 + 1);
            if (var4.Equals("sound", StringComparison.OrdinalIgnoreCase))
            {
                sndManager.addSound(var1, var2);
            }
            else if (var4.Equals("newsound", StringComparison.OrdinalIgnoreCase))
            {
                sndManager.addSound(var1, var2);
            }
            else if (var4.Equals("streaming", StringComparison.OrdinalIgnoreCase))
            {
                sndManager.addStreaming(var1, var2);
            }
            else if (var4.Equals("music", StringComparison.OrdinalIgnoreCase))
            {
                sndManager.addMusic(var1, var2);
            }
            else if (var4.Equals("newmusic", StringComparison.OrdinalIgnoreCase))
            {
                sndManager.addMusic(var1, var2);
            }
        }

        public string func_6262_n()
        {
            return terrainRenderer.getDebugInfoEntities();
        }

        public string func_21002_o()
        {
            return world.getDebugInfo();
        }

        public string func_6245_o()
        {
            return "P: " + particleManager.getStatistics() + ". T: " + world.getEntityCount();
        }

        public void respawn(bool var1, int var2)
        {
            if (!world.isRemote && !world.dimension.hasWorldSpawn())
            {
                usePortal();
            }

            Vec3i var3 = null;
            Vec3i var4 = null;
            bool var5 = true;
            if (player != null && !var1)
            {
                var3 = player.getSpawnPos();
                if (var3 != null)
                {
                    var4 = EntityPlayer.findRespawnPosition(world, var3);
                    if (var4 == null)
                    {
                        player.sendMessage("tile.bed.notValid");
                    }
                }
            }

            if (var4 == null)
            {
                var4 = world.getSpawnPos();
                var5 = false;
            }

            world.updateSpawnPosition();
            world.updateEntityLists();
            int var8 = 0;
            if (player != null)
            {
                var8 = player.id;
                world.remove(player);
            }

            camera = null;
            player = (ClientPlayerEntity)playerController.createPlayer(world);
            player.dimensionId = var2;
            camera = player;
            player.teleportToTop();
            if (var5)
            {
                player.setSpawnPos(var3);
                player.setPositionAndAnglesKeepPrevAngles((double)((float)var4.x + 0.5F), (double)((float)var4.y + 0.1F),
                    (double)((float)var4.z + 0.5F), 0.0F, 0.0F);
            }

            playerController.flipPlayer(player);
            world.addPlayer(player);
            player.movementInput = new MovementInputFromOptions(options);
            player.id = var8;
            player.spawn();
            playerController.func_6473_b(player);
            func_6255_d("Respawning");
            if (currentScreen is GuiGameOver)
            {
                displayGuiScreen((GuiScreen)null);
            }
        }

        public static void startup(string var0, string var1)
        {
            startMainThread(var0, var1, (string)null);
        }

        public static void startMainThread(string var0, string var1, string var2)
        {
            Minecraft mc = new(1920, 1080, false);
            java.lang.Thread var8 = new(mc, "Minecraft main thread");
            var8.setPriority(10);
            mc.minecraftUri = "www.minecraft.net";
            if (var0 != null && var1 != null)
            {
                mc.session = new Session(var0, var1);
            }
            else
            {
                mc.session = new Session("Player" + java.lang.System.currentTimeMillis() % 1000L, "");
            }

            if (var2 != null)
            {
                string[] var9 = var2.Split(":");
                mc.setServer(var9[0], Integer.parseInt(var9[1]));
            }

            var8.start();
        }

        public ClientNetworkHandler getSendQueue()
        {
            return player is EntityClientPlayerMP ? ((EntityClientPlayerMP)player).sendQueue : null;
        }

        private static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        public static void Main(string[] var0)
        {
            bool valid = JarValidator.ValidateJar("b1.7.3.jar");
            string var1 = null;
            string var2 = null;
            var1 = "Player" + java.lang.System.currentTimeMillis() % 1000L;
            if (var0.Length > 0)
            {
                var1 = var0[0];
            }

            var2 = "-";
            if (var0.Length > 1)
            {
                var2 = var0[1];
            }

            if (!valid)
            {
                var app = BuildAvaloniaApp();

                app.StartWithClassicDesktopLifetime(var0, ShutdownMode.OnMainWindowClose);

                if (LauncherWindow.Result != null && LauncherWindow.Result.Success)
                {
                    startup(var1, var2);
                }
            }
            else
            {
                startup(var1, var2);
            }
        }

        public static bool isGuiEnabled()
        {
            return INSTANCE == null || !INSTANCE.options.hideGUI;
        }

        public static bool isFancyGraphicsEnabled()
        {
            return INSTANCE != null;
        }

        public static bool isAmbientOcclusionEnabled()
        {
            return INSTANCE != null;
        }

        public static bool isDebugInfoEnabled()
        {
            return INSTANCE != null && INSTANCE.options.showDebugInfo;
        }

        public static bool lineIsCommand(string var1) => (var1.StartsWith("/"));
    }
}