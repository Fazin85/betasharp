using System.Text;
using java.lang;
using java.security;

namespace BetaSharp.Util;

public class MD5String
{
    private readonly string salt;

    public MD5String(string var1)
    {
        salt = var1;
    }

    public string func_27369_a(string var1)
    {
        try
        {
            string var2 = salt + var1;
            MessageDigest var3 = MessageDigest.getInstance("MD5");
            var3.update(Encoding.UTF8.GetBytes(var2), 0, var2.Length);
            return new java.math.BigInteger(1, var3.digest()).toString(16);
        }
        catch (NoSuchAlgorithmException var4)
        {
            throw new RuntimeException(var4);
        }
    }
}