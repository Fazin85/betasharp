namespace BetaSharp.Client.Input;

public class KeyBinding : java.lang.Object
{
    public string keyDescription;
    public int keyCode;

    public KeyBinding(string desc, int code)
    {
        keyDescription = desc;
        keyCode = code;
    }
}