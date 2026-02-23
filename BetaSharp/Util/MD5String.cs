using System.Security.Cryptography;
using System.Text;

namespace BetaSharp.Util;

public class MD5String
{
    private readonly string salt;

    public MD5String(string salt) => this.salt = salt;

    public string Hash(string str)
    {
        string saltedString = salt + str;
        byte[] inputBytes = Encoding.UTF8.GetBytes(saltedString);
        byte[] hashBytes = MD5.HashData(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}