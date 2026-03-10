using System;

namespace BetaSharp.Launcher.Features.Authentication;

internal sealed class AuthenticationPreflightException(string message) : Exception(message);
