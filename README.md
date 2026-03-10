# BetaSharp

[![Discord](https://img.shields.io/badge/chat%20on-discord-7289DA)](https://discord.gg/x9AGsjnWv4)
![C#](https://img.shields.io/badge/language-C%23-512BD4)
![.NET](https://img.shields.io/badge/framework-.NET-512BD4)
![Issues](https://img.shields.io/github/issues/Fazin85/betasharp)
![Pull requests](https://img.shields.io/github/issues-pr/Fazin85/betasharp)

An enhanced version of Minecraft Beta 1.7.3, ported to C#.

# Notice

> [!IMPORTANT]
> This project is based on decompiled Minecraft Beta 1.7.3 code and requires a legally purchased copy of the game.\
> We do not support or condone piracy. Please purchase Minecraft from https://www.minecraft.net.

## Running

The launcher is the recommended way to play. It authenticates with your Microsoft account and starts the client automatically. \
Clone the repository and run the following commands.

```
cd BetaSharp.Launcher
dotnet run --configuration Release
```

## Building

Clone the repository and make sure the .NET 10 SDK is installed. For installation, visit https://dotnet.microsoft.com/en-us/download. \
The Website lists instructions for downloading the SDK on Windows, macOS and Linux.

It is recommended to build with `--configuration Release` for better performance. \
The server and client expect the JAR file to be in their running directory.

```
cd BetaSharp.(Launcher/Client/Server)
dotnet build
```

## Publishing

Cross-platform support in v1 targets `win-x64`, `linux-x64`, and `osx-x64`.

For native-host publishes, Native AOT is enabled automatically:

```
dotnet publish BetaSharp.Launcher/BetaSharp.Launcher.csproj -c Release -r <host-rid>
```

For cross-OS publishes, use IL fallback:

```
dotnet publish BetaSharp.Launcher/BetaSharp.Launcher.csproj -c Release -r <target-rid> -p:PublishAot=false
```

Launcher artifacts include RID-scoped client payloads at:

```
BetaSharp.Launcher/bin/Release/net10.0/<target-rid>/publish/Client/<target-rid>/
```

The launcher first resolves the client from `Client/<rid>/`, then falls back to `Client/` for local development runs.

## Linux Authentication Notes

Launcher Microsoft sign-in on Linux requires `xdg-open` and WebKitGTK/libsoup runtime libraries.
If prerequisites are missing, the launcher will show a dependency error before interactive sign-in starts.

## Contributing

Contributions are welcome! Please read our [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

This is a personal project with no guarantees on review or merge timelines. Feel free to submit contributions, though they may or may not be reviewed or merged depending on the maintainer's availability and discretion.
