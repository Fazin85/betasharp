using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using BetaSharp.Blocks;
using BetaSharp.Client.Achievements;
using BetaSharp.Client.Entities;
using BetaSharp.Client.Guis;
using BetaSharp.Client.Input;
using BetaSharp.Client.Network;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entitys;
using BetaSharp.Client.Rendering.Items;
using BetaSharp.Client.Resource;
using BetaSharp.Client.Resource.Pack;
using BetaSharp.Client.Sound;
using BetaSharp.Client.Textures;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Launcher;
using BetaSharp.Profiling;
using BetaSharp.Server.Internal;
using BetaSharp.Stats;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Colors;
using BetaSharp.Worlds.Storage;
using ImGuiNET;
using java.lang;
using Silk.NET.Input;
using Silk.NET.OpenGL.Legacy;
using Silk.NET.OpenGL.Legacy.Extensions.ImGui;

namespace BetaSharp.Client;

public partial class Minecraft : java.lang.Object, Runnable
{
    public static Minecraft INSTANCE;
    public PlayerController playerController;
    private bool fullscreen = false;
    private bool hasCrashed = false;
    public int displayWidth;
    public int displayHeight;
    private readonly Timer timer = new(20.0F);
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
    private int ticksRan = 0;
    private int leftClickCounter = 0;
    private readonly int tempDisplayWidth;
    private readonly int tempDisplayHeight;
    public GuiAchievement guiAchievement;
    public GuiIngame ingameGUI;
    public bool skipRenderWorld = false;
    public HitResult objectMouseOver = null;
    public GameOptions options;
    public SoundManager sndManager = new();
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
    private readonly WaterSprite textureWaterFX = new();
    private readonly LavaSprite textureLavaFX = new();
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

    public Minecraft(int width, int height, bool isFullscreen)
    {
        loadingScreen = new LoadingScreenRenderer(this);
        guiAchievement = new GuiAchievement(this);
        tempDisplayHeight = height;
        fullscreen = isFullscreen;
        displayWidth = width;
        displayHeight = height;

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

    public void onMinecraftCrash(UnexpectedThrowable crashInfo)
    {
        hasCrashed = true;
        displayUnexpectedThrowable(crashInfo);
    }

    public void displayUnexpectedThrowable(UnexpectedThrowable unexpectedThrowable)
    {
        unexpectedThrowable.exception.printStackTrace();
    }

    public void setServer(string name, int port)
    {
        serverName = name;
        serverPort = port;
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
        saveLoader = new RegionWorldStorageSource(new java.io.File(mcDataDir, "saves"));
        options = new GameOptions(this, mcDataDir);
        Profiler.Enabled = options.debugMode;

        try
        {
            int[] msaaValues = [0, 2, 4, 8];
            Display.MSAA_Samples = msaaValues[options.msaaLevel];

            Display.create();
            Display.getGlfw().SetWindowSizeLimits(Display.getWindowHandle(), 850, 480, 3840, 2160);

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

        StatStringFormatKeyInv format = new(this);
        BetaSharp.Achievements.OpenInventory.GetTranslatedDescription = () =>
        {
            return format.formatString(BetaSharp.Achievements.OpenInventory.TranslationKey);
        };

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
        ScaledResolution var1 = new(options, displayWidth, displayHeight);
        GLManager.GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        GLManager.GL.MatrixMode(GLEnum.Projection);
        GLManager.GL.LoadIdentity();
        GLManager.GL.Ortho(0.0D, var1.ScaledWidthDouble, var1.ScaledHeightDouble, 0.0D, 1000.0D, 3000.0D);
        GLManager.GL.MatrixMode(GLEnum.Modelview);
        GLManager.GL.LoadIdentity();
        GLManager.GL.Translate(0.0F, 0.0F, -2000.0F);
        GLManager.GL.Viewport(0, 0, (uint)displayWidth, (uint)displayHeight);
        GLManager.GL.ClearColor(0.0F, 0.0F, 0.0F, 0.0F);
        Tessellator tessellator = Tessellator.instance;
        GLManager.GL.Disable(GLEnum.Lighting);
        GLManager.GL.Enable(GLEnum.Texture2D);
        GLManager.GL.Disable(GLEnum.Fog);
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)textureManager.getTextureId("/title/mojang.png"));
        tessellator.startDrawingQuads();
        tessellator.setColorOpaque_I(0x00FFFFFF);
        tessellator.addVertexWithUV(0.0D, (double)displayHeight, 0.0D, 0.0D, 0.0D);
        tessellator.addVertexWithUV((double)displayWidth, (double)displayHeight, 0.0D, 0.0D, 0.0D);
        tessellator.addVertexWithUV((double)displayWidth, 0.0D, 0.0D, 0.0D, 0.0D);
        tessellator.addVertexWithUV(0.0D, 0.0D, 0.0D, 0.0D, 0.0D);
        tessellator.draw();
        short var3 = 256;
        short var4 = 256;
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        tessellator.setColorOpaque_I(0x00FFFFFF);
        func_6274_a((var1.ScaledWidth - var3) / 2, (var1.ScaledHeight - var4) / 2, 0, 0, var3, var4);
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
        minecraftDir ??= getAppDir(nameof(BetaSharp));

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
                string var3 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var2 = new java.io.File(var3, "." + var0 + '/');

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

    public void displayGuiScreen(GuiScreen newScreen)
    {
        if (!(currentScreen is GuiUnused))
        {
            currentScreen?.OnGuiClosed();

            if (newScreen is GuiMainMenu)
            {
                statFileWriter.func_27175_b();
            }

            statFileWriter.syncStats();
            if (newScreen == null && world == null)
            {
                newScreen = new GuiMainMenu();
            }
            else if (newScreen == null && player.health <= 0)
            {
                newScreen = new GuiGameOver();
            }

            if (newScreen is GuiMainMenu)
            {
                ingameGUI.clearChatMessages();
            }

            currentScreen = newScreen;

            if (internalServer != null)
            {
                bool shouldPause = newScreen?.DoesGuiPauseGame() ?? false;
                internalServer.SetPaused(shouldPause);
            }

            if (newScreen != null)
            {
                setIngameNotInFocus();
                ScaledResolution scaledResolution = new(options, displayWidth, displayHeight);
                int scaledWidth = scaledResolution.ScaledWidth;
                int scaledHeight = scaledResolution.ScaledHeight;
                newScreen.SetWorldAndResolution(this, scaledWidth, scaledHeight);
                skipRenderWorld = false;
            }
            else
            {
                setIngameFocus();
            }
        }
    }

    [Conditional("DEBUG")]
    private void checkGLError(string location)
    {
        GLEnum glError = GLManager.GL.GetError();
        if (glError != 0)
        {
            Console.WriteLine($"#### GL ERROR ####");
            Console.WriteLine($"@ {location}");
            Console.WriteLine($"> {glError.ToString()}");
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
            catch (Throwable worldChangeException)
            {
            }

            try
            {
                GLAllocation.deleteTexturesAndDisplayLists();
            }
            catch (Throwable textureCleanupException)
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
                Environment.Exit(0);
            }
        }
    }

    public void run()
    {
        running = true;

        try
        {
            startGame();
        }
        catch (java.lang.Exception startupException)
        {
            startupException.printStackTrace();
            onMinecraftCrash(new UnexpectedThrowable("Failed to start game", startupException));
            return;
        }

        try
        {
            long lastFpsCheckTime = java.lang.System.currentTimeMillis();
            int frameCounter = 0;

            while (running)
            {
                if (options.debugMode)
                {
                    Profiler.Update(timer.DeltaTime);
                    Profiler.Record("frame Time", timer.DeltaTime * 1000.0f);
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
                        float previousRenderPartialTicks = timer.renderPartialTicks;
                        timer.UpdateTimer();
                        timer.renderPartialTicks = previousRenderPartialTicks;
                    }
                    else
                    {
                        timer.UpdateTimer();
                    }

                    long tickStartTime = java.lang.System.nanoTime();
                    if (options.debugMode)
                    {
                        Profiler.PushGroup("runTicks");
                    }

                    for (int tickIndex = 0; tickIndex < timer.elapsedTicks; ++tickIndex)
                    {
                        ++ticksRan;

                        try
                        {
                            runTick(timer.renderPartialTicks);
                        }
                        catch (MinecraftException tickException)
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

                    long tickElapsedTime = java.lang.System.nanoTime() - tickStartTime;
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
                        playerController?.setPartialTime(timer.renderPartialTicks);

                        if (options.debugMode) Profiler.PushGroup("render");
                        gameRenderer.onFrameUpdate(timer.renderPartialTicks);
                        if (options.debugMode) Profiler.PopGroup();
                    }

                    if (imGuiController != null && timer.DeltaTime > 0.0f && options.showDebugInfo && options.debugMode)
                    {
                        imGuiController.Update(timer.DeltaTime);
                        ProfilerRenderer.Draw();
                        ProfilerRenderer.DrawGraph();

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
                        displayDebugInfo(tickElapsedTime);
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
                    ++frameCounter;

                    isGamePaused = (!isMultiplayerWorld() || internalServer != null) && currentScreen != null &&
                                   currentScreen.DoesGuiPauseGame();

                    for (;
                         java.lang.System.currentTimeMillis() >= lastFpsCheckTime + 1000L;
                         frameCounter = 0)
                    {
                        debug = frameCounter + " fps";
                        lastFpsCheckTime += 1000L;
                    }
                }
                catch (MinecraftException)
                {
                    world = null;
                    changeWorld1(null);
                    displayGuiScreen(new GuiConflictWarning());
                }
                catch (OutOfMemoryError)
                {
                    crashCleanup();
                    displayGuiScreen(new GuiErrorScreen());
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
        catch (MinecraftError)
        {
        }
        catch (Throwable unexpectedException)
        {
            crashCleanup();
            unexpectedException.printStackTrace();
            onMinecraftCrash(new UnexpectedThrowable("Unexpected error", unexpectedException));
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
            changeWorld1(null);
        }
        catch (System.Exception)
        {
        }
    }

    private void screenshotListener()
    {
        if (Keyboard.isKeyDown(Keyboard.KEY_F2))
        {
            if (!isTakingScreenshot)
            {
                isTakingScreenshot = true;
                int size = displayWidth * displayHeight * 3;
                byte[] pixels = new byte[size];
                GLManager.GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
                unsafe
                {
                    fixed (byte* p = pixels)
                    {
                        GLManager.GL.ReadPixels(0, 0, (uint)displayWidth, (uint)displayHeight, PixelFormat.Rgb, PixelType.UnsignedByte, p);
                    }
                }
                string result = ScreenShotHelper.saveScreenshot(minecraftDir.getAbsolutePath(), displayWidth, displayHeight, pixels);
                ingameGUI.addChatMessage(result);
            }
        }
        else
        {
            isTakingScreenshot = false;
        }
    }

    private void displayDebugInfo(long tickElapsedTime)
    {
        long targetFrameTime = 16666666L;
        if (prevFrameTime == -1L)
        {
            prevFrameTime = java.lang.System.nanoTime();
        }

        long currentNanoTime = java.lang.System.nanoTime();
        tickTimes[numRecordedFrameTimes & frameTimes.Length - 1] = tickElapsedTime;
        frameTimes[numRecordedFrameTimes++ & frameTimes.Length - 1] = currentNanoTime - prevFrameTime;
        prevFrameTime = currentNanoTime;
        GLManager.GL.Clear(ClearBufferMask.DepthBufferBit);
        GLManager.GL.MatrixMode(GLEnum.Projection);
        GLManager.GL.LoadIdentity();
        GLManager.GL.Ortho(0.0D, (double)displayWidth, (double)displayHeight, 0.0D, 1000.0D, 3000.0D);
        GLManager.GL.MatrixMode(GLEnum.Modelview);
        GLManager.GL.LoadIdentity();
        GLManager.GL.Translate(0.0F, 0.0F, -2000.0F);
        GLManager.GL.LineWidth(1.0F);
        GLManager.GL.Disable(GLEnum.Texture2D);
        Tessellator tessellator = Tessellator.instance;
        tessellator.startDrawing(7);
        int barHeightPixels = (int)(targetFrameTime / 200000L);
        tessellator.setColorOpaque_I(536870912);
        tessellator.addVertex(0.0D, (double)(displayHeight - barHeightPixels), 0.0D);
        tessellator.addVertex(0.0D, (double)displayHeight, 0.0D);
        tessellator.addVertex((double)frameTimes.Length, (double)displayHeight, 0.0D);
        tessellator.addVertex((double)frameTimes.Length, (double)(displayHeight - barHeightPixels), 0.0D);
        tessellator.setColorOpaque_I(538968064);
        tessellator.addVertex(0.0D, (double)(displayHeight - barHeightPixels * 2), 0.0D);
        tessellator.addVertex(0.0D, (double)(displayHeight - barHeightPixels), 0.0D);
        tessellator.addVertex((double)frameTimes.Length, (double)(displayHeight - barHeightPixels), 0.0D);
        tessellator.addVertex((double)frameTimes.Length, (double)(displayHeight - barHeightPixels * 2), 0.0D);
        tessellator.draw();
        long totalFrameTimesSum = 0L;

        int averageFrameTimePixels;
        for (averageFrameTimePixels = 0; averageFrameTimePixels < frameTimes.Length; ++averageFrameTimePixels)
        {
            totalFrameTimesSum += frameTimes[averageFrameTimePixels];
        }

        averageFrameTimePixels = (int)(totalFrameTimesSum / 200000L / (long)frameTimes.Length);
        tessellator.startDrawing(7);
        tessellator.setColorOpaque_I(541065216);
        tessellator.addVertex(0.0D, (double)(displayHeight - averageFrameTimePixels), 0.0D);
        tessellator.addVertex(0.0D, (double)displayHeight, 0.0D);
        tessellator.addVertex((double)frameTimes.Length, (double)displayHeight, 0.0D);
        tessellator.addVertex((double)frameTimes.Length, (double)(displayHeight - averageFrameTimePixels), 0.0D);
        tessellator.draw();
        tessellator.startDrawing(1);

        for (int frameIndex = 0; frameIndex < frameTimes.Length; ++frameIndex)
        {
            int colorBrightnessPercent = (frameIndex - numRecordedFrameTimes & frameTimes.Length - 1) * 255 / frameTimes.Length;
            int colorBrightness = colorBrightnessPercent * colorBrightnessPercent / 255;
            colorBrightness = colorBrightness * colorBrightness / 255;
            int colorValue = colorBrightness * colorBrightness / 255;
            colorValue = colorValue * colorValue / 255;
            if (frameTimes[frameIndex] > targetFrameTime)
            {
                tessellator.setColorOpaque_I(unchecked((int)(0xFF000000u + (uint)colorBrightness * 65536u)));
            }
            else
            {
                tessellator.setColorOpaque_I(unchecked((int)(0xFF000000u + (uint)colorBrightness * 256u)));
            }

            long frameTimePixels = frameTimes[frameIndex] / 200000L;
            long tickTimePixels = tickTimes[frameIndex] / 200000L;
            tessellator.addVertex((double)((float)frameIndex + 0.5F), (double)((float)((long)displayHeight - frameTimePixels) + 0.5F),
                0.0D);
            tessellator.addVertex((double)((float)frameIndex + 0.5F), (double)((float)displayHeight + 0.5F), 0.0D);
            tessellator.setColorOpaque_I(unchecked((int)(0xFF000000u + (uint)colorBrightness * 65536u + (uint)colorBrightness * 256u + (uint)colorBrightness * 1u)));
            tessellator.addVertex((double)((float)frameIndex + 0.5F), (double)((float)((long)displayHeight - frameTimePixels) + 0.5F),
                0.0D);
            tessellator.addVertex((double)((float)frameIndex + 0.5F),
                (double)((float)((long)displayHeight - (frameTimePixels - tickTimePixels)) + 0.5F), 0.0D);
        }

        tessellator.draw();
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
            player?.resetPlayerKeyState();

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

    private void func_6254_a(int mouseButton, bool isHoldingMouse)
    {
        if (!playerController.field_1064_b)
        {
            if (!isHoldingMouse)
            {
                leftClickCounter = 0;
            }

            if (mouseButton != 0 || leftClickCounter <= 0)
            {
                if (isHoldingMouse && objectMouseOver != null && objectMouseOver.type == HitResultType.TILE &&
                    mouseButton == 0)
                {
                    int blockX = objectMouseOver.blockX;
                    int blockY = objectMouseOver.blockY;
                    int blockZ = objectMouseOver.blockZ;
                    playerController.sendBlockRemoving(blockX, blockY, blockZ, objectMouseOver.side);
                    particleManager.addBlockHitEffects(blockX, blockY, blockZ, objectMouseOver.side);
                }
                else
                {
                    playerController.resetBlockRemoving();
                }
            }
        }
    }

    private void clickMouse(int mouseButton)
    {
        if (mouseButton != 0 || leftClickCounter <= 0)
        {
            if (mouseButton == 0)
            {
                player.swingHand();
            }

            bool shouldPerformSecondaryAction = true;
            if (objectMouseOver == null)
            {
                if (mouseButton == 0)
                {
                    leftClickCounter = 10;
                }
            }
            else if (objectMouseOver.type == HitResultType.ENTITY)
            {
                if (mouseButton == 0)
                {
                    playerController.attackEntity(player, objectMouseOver.entity);
                }

                if (mouseButton == 1)
                {
                    playerController.interactWithEntity(player, objectMouseOver.entity);
                }
            }
            else if (objectMouseOver.type == HitResultType.TILE)
            {
                int blockX = objectMouseOver.blockX;
                int blockY = objectMouseOver.blockY;
                int blockZ = objectMouseOver.blockZ;
                int blockSide = objectMouseOver.side;
                if (mouseButton == 0)
                {
                    playerController.clickBlock(blockX, blockY, blockZ, objectMouseOver.side);
                }
                else
                {
                    ItemStack selectedItem = player.inventory.getSelectedItem();
                    int itemCountBefore = selectedItem != null ? selectedItem.count : 0;
                    if (playerController.sendPlaceBlock(player, world, selectedItem, blockX, blockY, blockZ, blockSide))
                    {
                        shouldPerformSecondaryAction = false;
                        player.swingHand();
                    }

                    if (selectedItem == null)
                    {
                        return;
                    }

                    if (selectedItem.count == 0)
                    {
                        player.inventory.main[player.inventory.selectedSlot] = null;
                    }
                    else if (selectedItem.count != itemCountBefore)
                    {
                        gameRenderer.itemRenderer.func_9449_b();
                    }
                }
            }

            if (shouldPerformSecondaryAction && mouseButton == 1)
            {
                ItemStack selectedItem = player.inventory.getSelectedItem();
                if (selectedItem != null && playerController.sendUseItem(player, world, selectedItem))
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
        catch (System.Exception displayException)
        {
            Console.WriteLine(displayException);
        }
    }

    private void resize(int newWidth, int newHeight)
    {
        if (newWidth <= 0)
        {
            newWidth = 1;
        }

        if (newHeight <= 0)
        {
            newHeight = 1;
        }

        displayWidth = newWidth;
        displayHeight = newHeight;
        Mouse.setDisplayDimensions(displayWidth, displayHeight);

        if (currentScreen != null)
        {
            ScaledResolution scaledResolution = new(options, newWidth, newHeight);
            int scaledWidth = scaledResolution.ScaledWidth;
            int scaledHeight = scaledResolution.ScaledHeight;
            currentScreen.SetWorldAndResolution(this, scaledWidth, scaledHeight);
        }
    }

    private void clickMiddleMouseButton()
    {
        if (objectMouseOver != null)
        {
            int blockId = world.getBlockId(objectMouseOver.blockX, objectMouseOver.blockY, objectMouseOver.blockZ);
            if (blockId == Block.GrassBlock.id)
            {
                blockId = Block.Dirt.id;
            }

            if (blockId == Block.DoubleSlab.id)
            {
                blockId = Block.Slab.id;
            }

            if (blockId == Block.Bedrock.id)
            {
                blockId = Block.Stone.id;
            }

            player.inventory.setCurrentItem(blockId, false);
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
            currentScreen.HandleInput();
            if (currentScreen != null)
            {
                currentScreen.ParticlesGui.updateParticles();
                currentScreen.UpdateScreen();
            }
        }

        if (currentScreen == null || currentScreen.AllowUserInput)
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
            if (internalServer != null)
            {
                internalServer.SetDifficulty(options.difficulty);
            }

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
            if (!isGamePaused || (isMultiplayerWorld() && internalServer == null))
            {
                world.allowSpawning(options.difficulty > 0, true);
                world.Tick();
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
            long timeSinceLastMouseEvent = java.lang.System.currentTimeMillis() - systemTime;
            if (timeSinceLastMouseEvent <= 200L)
            {
                int mouseWheelDelta = Mouse.getEventDWheel();
                if (mouseWheelDelta != 0)
                {
                    player.inventory.changeCurrentItem(mouseWheelDelta);
                    if (options.field_22275_C)
                    {
                        if (mouseWheelDelta > 0)
                        {
                            mouseWheelDelta = 1;
                        }

                        if (mouseWheelDelta < 0)
                        {
                            mouseWheelDelta = -1;
                        }

                        options.field_22272_F += (float)mouseWheelDelta * 0.25F;
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
                else
                {
                    currentScreen?.HandleMouseInput();
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
                        currentScreen.HandleKeyboardInput();
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
                            // Open chat with '/' and place the cursor after it
                            displayGuiScreen(new GuiChat("/", true));
                        }
                    }

                    for (int slotIndex = 0; slotIndex < 9; ++slotIndex)
                    {
                        if (Keyboard.getEventKey() == Keyboard.KEY_1 + slotIndex)
                        {
                            player.inventory.selectedSlot = slotIndex;
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
    }

    public bool isMultiplayerWorld()
    {
        return world != null && world.isRemote;
    }

    public void startWorld(string worldName, string mainMenuText, long seed)
    {
        changeWorld1((World)null);
        displayGuiScreen(new GuiLevelLoading(worldName, seed));
    }

    public void changeWorld1(World newWorld)
    {
        changeWorld2(newWorld, "");
    }

    public void changeWorld2(World newWorld, string loadingMessage)
    {
        changeWorld(newWorld, loadingMessage, (EntityPlayer)null);
    }

    public void changeWorld(World newWorld, string loadingText, EntityPlayer targetEntity)
    {
        statFileWriter.func_27175_b();
        statFileWriter.syncStats();
        camera = null;
        loadingScreen.printText(loadingText);
        loadingScreen.progressStage("");
        sndManager.playStreaming((string)null, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F);

        world = newWorld;
        if (newWorld != null)
        {
            playerController.func_717_a(newWorld);
            if (!isMultiplayerWorld())
            {
                if (targetEntity == null)
                {
                    player = (ClientPlayerEntity)newWorld.getPlayerForProxy(ClientPlayerEntity.Class);
                }
            }
            else if (player != null)
            {
                player.teleportToTop();
                newWorld?.SpawnEntity(player);
            }

            if (player == null)
            {
                player = (ClientPlayerEntity)playerController.createPlayer(newWorld);
                player.teleportToTop();
                playerController.flipPlayer(player);
            }

            player.movementInput = new MovementInputFromOptions(options);
            terrainRenderer?.changeWorld(newWorld);

            particleManager?.clearEffects(newWorld);

            playerController.func_6473_b(player);
            if (targetEntity != null)
            {
                newWorld.saveWorldData();
            }

            newWorld.addPlayer(player);
            if (newWorld.isNewWorld)
            {
                newWorld.savingProgress(loadingScreen);
            }

            camera = player;
        }
        else
        {
            player = null;
        }

        systemTime = 0L;
    }

    private void showText(string loadingText)
    {
        loadingScreen.printText(loadingText);
        loadingScreen.progressStage("Building terrain");
        short loadingRadius = 128;
        int loadedChunkCount = 0;
        int totalChunksToLoad = loadingRadius * 2 / 16 + 1;
        totalChunksToLoad *= totalChunksToLoad;
        Vec3i centerPos = world.getSpawnPos();
        if (player != null)
        {
            centerPos.x = (int)player.x;
            centerPos.z = (int)player.z;
        }

        for (int xOffset = -loadingRadius; xOffset <= loadingRadius; xOffset += 16)
        {
            for (int zOffset = -loadingRadius; zOffset <= loadingRadius; zOffset += 16)
            {
                loadingScreen.setLoadingProgress(loadedChunkCount++ * 100 / totalChunksToLoad);
                world.getBlockId(centerPos.x + xOffset, 64, centerPos.z + zOffset);

                while (world.doLightingUpdates())
                {
                }
            }
        }

        loadingScreen.progressStage("Simulating world for a bit");
        world.tickChunks();
    }

    public void installResource(string resourcePath, java.io.File resourceFile)
    {
        if (!resourceFile.getPath().EndsWith("ogg"))
        {
            //TODO: ADD SUPPORT FOR MUS SFX?
            return;
        }

        int slashIndex = resourcePath.IndexOf("/");
        string category = resourcePath.Substring(0, slashIndex);
        resourcePath = resourcePath.Substring(slashIndex + 1);
        if (category.Equals("sound", StringComparison.OrdinalIgnoreCase))
        {
            sndManager.addSound(resourcePath, resourceFile);
        }
        else if (category.Equals("newsound", StringComparison.OrdinalIgnoreCase))
        {
            sndManager.addSound(resourcePath, resourceFile);
        }
        else if (category.Equals("streaming", StringComparison.OrdinalIgnoreCase))
        {
            sndManager.addStreaming(resourcePath, resourceFile);
        }
        else if (category.Equals("music", StringComparison.OrdinalIgnoreCase))
        {
            sndManager.addMusic(resourcePath, resourceFile);
        }
        else if (category.Equals("newmusic", StringComparison.OrdinalIgnoreCase))
        {
            sndManager.addMusic(resourcePath, resourceFile);
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

    public void respawn(bool ignoreSpawnPosition, int newDimensionId)
    {
        Vec3i playerSpawnPos = null;
        Vec3i respawnPos = null;
        bool useBedSpawn = true;
        if (player != null && !ignoreSpawnPosition)
        {
            playerSpawnPos = player.getSpawnPos();
            if (playerSpawnPos != null)
            {
                respawnPos = EntityPlayer.findRespawnPosition(world, playerSpawnPos);
                if (respawnPos == null)
                {
                    player.sendMessage("tile.bed.notValid");
                }
            }
        }

        if (respawnPos == null)
        {
            respawnPos = world.getSpawnPos();
            useBedSpawn = false;
        }

        world.UpdateSpawnPosition();
        world.updateEntityLists();
        int previousPlayerId = 0;
        if (player != null)
        {
            previousPlayerId = player.id;
            world.Remove(player);
        }

        camera = null;
        player = (ClientPlayerEntity)playerController.createPlayer(world);
        player.dimensionId = newDimensionId;
        camera = player;
        player.teleportToTop();
        if (useBedSpawn)
        {
            player.setSpawnPos(playerSpawnPos);
            player.setPositionAndAnglesKeepPrevAngles((double)((float)respawnPos.x + 0.5F), (double)((float)respawnPos.y + 0.1F),
                (double)((float)respawnPos.z + 0.5F), 0.0F, 0.0F);
        }

        playerController.flipPlayer(player);
        world.addPlayer(player);
        player.movementInput = new MovementInputFromOptions(options);
        player.id = previousPlayerId;
        player.spawn();
        playerController.func_6473_b(player);
        showText("Respawning");
        if (currentScreen is GuiGameOver)
        {
            displayGuiScreen((GuiScreen)null);
        }
    }

    private static void StartMainThread(string playerName, string sessionToken)
    {
        Minecraft mc = new(1280, 720, false);
        java.lang.Thread mainThread = new(mc, "Minecraft main thread");
        mainThread.setPriority(10);
        mc.minecraftUri = "www.minecraft.net";
        if (playerName != null && sessionToken != null)
        {
            mc.session = new Session(playerName, sessionToken);
        }
        else
        {
            mc.session = new Session("Player" + java.lang.System.currentTimeMillis() % 1000L, "");
        }

        mainThread.start();
    }

    public ClientNetworkHandler getSendQueue()
    {
        return player is EntityClientPlayerMP ? ((EntityClientPlayerMP)player).sendQueue : null;
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    public static void Startup(string[] args)
    {
        bool valid = JarValidator.ValidateJar("b1.7.3.jar");
        string playerName = null;
        string sessionToken = null;
        playerName = "Player" + java.lang.System.currentTimeMillis() % 1000L;
        if (args.Length > 0)
        {
            playerName = args[0];
        }

        sessionToken = "-";
        if (args.Length > 1)
        {
            sessionToken = args[1];
        }

        if (!valid)
        {
            var app = BuildAvaloniaApp();

            app.StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);

            if (LauncherWindow.Result != null && LauncherWindow.Result.Success)
            {
                StartMainThread(playerName, sessionToken);
            }
        }
        else
        {
            StartMainThread(playerName, sessionToken);
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
