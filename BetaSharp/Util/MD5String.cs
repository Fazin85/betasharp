using System.Text;
using java.lang;
using java.security;

namespace BetaSharp.Util;

public class MD5String
{
    private readonly string salt;

    public MD5String(string salt)
    {
        this.salt = salt;
    }

    public string hash(string str)
    {
        try
        {
            string saltedString = salt + str;
            MessageDigest var3 = MessageDigest.getInstance("MD5");
            var3.update(Encoding.UTF8.GetBytes(saltedString), 0, saltedString.Length);
            return new java.math.BigInteger(1, var3.digest()).toString(16);
        }
        catch (NoSuchAlgorithmException ex)
        {
            throw new RuntimeException(ex);
        }
    }
}