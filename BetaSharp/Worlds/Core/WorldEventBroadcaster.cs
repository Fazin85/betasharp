using BetaSharp.Entities;

namespace BetaSharp.Worlds.Core;

public class WorldEventBroadcaster
{
    private readonly List<IWorldAccess> _eventListeners;

    public WorldEventBroadcaster(List<IWorldAccess> eventListeners)
    {
        _eventListeners = eventListeners;
    }

    public void PlaySoundToEntity(Entity entity, string sound, float volume, float pitch)
    {
        foreach (var t in _eventListeners)
        {
            t.playSound(sound, entity.x, entity.y - entity.standingEyeHeight, entity.z, volume,
                pitch);
        }
    }

    public void PlaySoundAtPos(double x, double y, double z, string sound, float volume, float pitch)
    {
        foreach (var t in _eventListeners)
        {
            t.playSound(sound, x, y, z, volume, pitch);
        }
    }

    public void PlayStreamingAtPos(string? music, int x, int y, int z)
    {
        foreach (var t in _eventListeners)
        {
            t.playStreaming(music, x, y, z);
        }
    }

    public void AddParticle(string particle, double x, double y, double z, double velocityX, double velocityY, double velocityZ)
    {
        foreach (var t in _eventListeners)
        {
            t.spawnParticle(particle, x, y, z, velocityX, velocityY, velocityZ);
        }
    }

    public void BlockUpdateEvent(int x, int y, int z)
    {
        foreach (var t in _eventListeners)
        {
            t.blockUpdate(x, y, z);
        }
    }

    public void AddWorldAccess(IWorldAccess worldAccess) => _eventListeners.Add(worldAccess);

    public void RemoveWorldAccess(IWorldAccess worldAccess) => _eventListeners.Remove(worldAccess);
}
