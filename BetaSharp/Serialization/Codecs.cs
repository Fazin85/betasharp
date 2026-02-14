using WhiteTowerGames.DataFixerSharper;
using WhiteTowerGames.DataFixerSharper.Codecs;

namespace BetaSharp.Serialization;

public static class Codecs
{
    public static readonly Codec<byte> Int8 = BuiltinCodecs.Int32.SafeMap<byte>(
        i8 => (int)i8,
        i32 => (byte)i32
    );

    public static readonly Codec<short> Int16 = BuiltinCodecs.Int32.SafeMap<short>(
        i16 => (int)i16,
        i32 => (short)i32
    );

    public static readonly Codec<byte[]> ByteArray = Int8.ForArray();
}
