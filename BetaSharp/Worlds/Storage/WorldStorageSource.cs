using java.util;

namespace BetaSharp.Worlds.Storage;

public interface WorldStorageSource
{
    string getName();

    WorldStorage get(string saveName, bool createPlayerDataDir);

    List getAll();

    void flush();

    WorldProperties getProperties(string var1);

    void delete(string var1);

    void rename(string var1, string var2);


}