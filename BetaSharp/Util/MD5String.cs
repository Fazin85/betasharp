using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace BetaSharp.Util;

public class MD5String
{
    private readonly string salt;

    public MD5String(string salt)
    {
        this.salt = salt;
    }

    // Suppress CA5351 because we need MD5 for legacy checksum compatibility, not security.
#pragma warning disable CA5351 
    public string Hash(string str)
    {
        byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(salt + str));
        
        // Replicates the legacy Java behavior where leading zeros are truncated
        return new BigInteger(hashBytes, isUnsigned: true, isBigEndian: true).ToString("x");
    }
#pragma warning restore CA5351
}