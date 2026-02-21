using BetaSharp.Entities;
using BetaSharp.Worlds;

namespace BetaSharp;

public record SpawnListEntry(Func<World, EntityLiving> Factory, int SpawnWeight);
