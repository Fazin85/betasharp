# BetaSharp

![C#](https://img.shields.io/badge/language-C%23-512BD4)
![.NET](https://img.shields.io/badge/framework-.NET-512BD4)
![Issues](https://img.shields.io/github/issues/Fazin85/betasharp)
![Pull requests](https://img.shields.io/github/issues-pr/Fazin85/betasharp)

An enhanced version of Minecraft Beta 1.7.3, ported to C#.

# Notice

This project is based on decompiled Minecraft Beta 1.7.3 code and requires a legally purchased copy of the game.\
We do not support or condone piracy. Please purchase Minecraft from https://www.minecraft.net.

## Building

Clone the repository and make sure .NET SDK is installed, for installation visit https://dotnet.microsoft.com/en-us/download. \
The Website lists instructions for downloading the SDK on Windows, macOS and Linux.

### Client

For building the client. It is recommended to build with `--configuration Release` for better performance.

```
cd BetaSharp.Client
dotnet build --configuration Release
```

### Server

For building the server.

```
cd BetaSharp.Server
dotnet build --configuration Release
```

## Contributing

Contributions are welcome, but this is a personal project with no guarantees on review or merge timelines. Feel free to submit contributions, though they may or may not be reviewed or merged depending on the maintainer's availability and discretion.

## AI Policy

Small amounts of AI assistance are allowed, but low-quality or "vibe-coded" content is not. Please ensure all code is high-quality and fully understood before submitting.
