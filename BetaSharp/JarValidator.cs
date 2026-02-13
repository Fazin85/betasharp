using System.Security.Cryptography;

namespace BetaSharp;

public class JarValidator
{
    private const string EXPECTED_HASH = "af1fa04b8006d3ef78c7e24f8de4aa56f439a74d7f314827529062d5bab6db4c";

    public static bool ValidateJar(string jarPath)
    {
        if (!File.Exists(jarPath))
            return false;

        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(jarPath);
        byte[] hashBytes = sha256.ComputeHash(stream);
        string actualHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return actualHash.Equals(EXPECTED_HASH, StringComparison.OrdinalIgnoreCase);
    }
}