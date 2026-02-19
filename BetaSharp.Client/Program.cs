using BetaSharp;
using BetaSharp.Client;

Log.Initialize(new LogOptions(IsServer: false));
Log.AddCrashHandlers();
Minecraft.Startup(args);
