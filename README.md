# BetaSharp

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Minecraft](https://img.shields.io/badge/Minecraft-Beta%201.7.3-62B47A?style=for-the-badge&logo=minecraft&logoColor=white)
![Status](https://img.shields.io/badge/Status-In%20Development-yellow?style=for-the-badge)

An enhanced version of Minecraft Beta 1.7.3, ported to C#.

## Legal Notice

⚠️ **Important:** This project is a derivative work based on decompiled Minecraft Beta 1.7.3 code.

- You must own a legitimate copy of Minecraft to use this client.
- Microsoft account authentication is required or you must provide your own legally owned client.jar.
- No game assets are distributed with this project.
- We do not support or condone piracy. Please purchase Minecraft from https://www.minecraft.net/.

## Contributing

Contributions are welcome, but this is a personal project with no guarantees on review or merge timelines. Feel free to submit contributions, though they may or may not be reviewed or merged depending on the maintainer's availability and discretion.

## Coding Style

Use Allman style formatting for braces:
```csharp
if (...)
{
}
```

References to Minecraft object should be called `mc`, not `theMinecraft`, `minecraft`, or `game`.

## Build from source
If you don't yet have the .NET SDK and Runtime, please install it beforehand.

### Get .NET 10.0
#### Windows
Simply install the .NET 10 runtime with the installer Microsoft provides [on their website](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

#### Linux (Debian)
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-1.0 dotnet-runtime-10.0
```

#### Linux (Fedora)
```bash
sudo dnf install dotnet-sdk-10.0 dotnet-runtime-10.0
```

### Building
Clone the source code.
```bash
git clone https://github.com/Fazin85/betasharp
cd betasharp
```

#### Client
If you'd like to use the client, run the following command.
```bash
cd BetaSharp.Client
dotnet run -c Debug
```
Use `Release` instead of `Debug` if you'd like a more performant but less debuggable binary.

#### Server
If you'd like to run a server, instead of a client, move over into the `BetaSharp.Server` directory, then build from there.
```bash
cd BetaSharp.Server
dotnet run -c Debug
```
Use `Release` instead of `Debug` if you'd like a more performant but less debuggable binary.

## License

This project is shared openly for collaboration. All code is derivative of Minecraft and subject to Mojang's rights.