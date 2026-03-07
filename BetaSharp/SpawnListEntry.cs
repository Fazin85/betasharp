using BetaSharp.Entities;
using BetaSharp.Worlds.Core;

namespace BetaSharp;

public record SpawnListEntry(Func<World, EntityLiving> Factory);
