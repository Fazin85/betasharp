using BetaSharp.NBT;
using BetaSharp.Worlds;
using Exception = System.Exception;

namespace BetaSharp.Entities;

public class EntityRegistry
{
    private static Dictionary<string, Type> idToClass = new ();
    private static Dictionary<Type, string> classToId = new ();
    private static Dictionary<int, Type> rawIdToClass = new ();
    private static Dictionary<Type, int> classToRawId = new ();
    public static Dictionary<string, int> namesToId = new();

    private static void Register(Type entityType, string id, int rawId)
    {
        idToClass.Add(id, entityType);
        classToId.Add(entityType, id);
        rawIdToClass.Add(rawId, entityType);
        classToRawId.Add(entityType, rawId);
        namesToId.TryAdd(id.ToLower(), rawId);
    }

    public static Entity Create(string id, World world)
    {
        Entity? entity = null;

        try
        {
	        entity = (Entity)Activator.CreateInstance(idToClass[id], world)!;
        }
        catch (Exception e)
        {
	        Log.Error(e);
        }

        return entity;
    }

    public static Entity getEntityFromNbt(NBTTagCompound nbt, World world)
    {
        Entity entity = null;

        try
        {
	        entity = ((Entity)Activator.CreateInstance(idToClass[nbt.GetString("id")], world)!);
        }
        catch (Exception e)
        {
	        Log.Error(e);
        }

        if (entity != null)
        {
            entity.read(nbt);
        }
        else
        {
            Log.Info($"Skipping Entity with id {nbt.GetString("id")}");
        }

        return entity;
    }

    public static Entity? createEntityAt(string name, World world, float x, float y, float z)
    {
        name = name.ToLower();
        try
        {
            if (namesToId.TryGetValue(name, out int id))
            {
				if(rawIdToClass.TryGetValue(id, out Type type))
                {
                    var entity = ((Entity)Activator.CreateInstance(type, world));

                    if (entity != null)
                    {
                        entity.setPosition(x, y, z);
                        entity.setPositionAndAngles(x, y, z, 0, 0);
                        if (!world.SpawnEntity(entity))
                        {
                            Log.Error($"Entity `{name}` with ID:`{id}` failed to join world.");
                        }
                    }

                    return entity;
                }
                else
                {
                    Log.Error($"Failed to convert entity of name `{name}` and id `{id}` to a class.");
                }
            }
            else
            {
                Log.Error($"Unable to find entity of name `{name}`.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return null;
    }

    public static Entity create(int rawId, World world)
    {
        Entity entity = null;

        try
        {
	        entity = ((Entity)Activator.CreateInstance(rawIdToClass[rawId], world));
        }
        catch (java.lang.Exception ex)
        {
            Log.Error(ex);
        }

        if (entity == null)
        {
            Log.Info($"Skipping Entity with id {rawId}");
        }

        return entity;
    }

    public static int GetRawId(Entity entity)
    {
        return classToRawId[entity.GetType()];
    }

    public static string GetId(Entity entity)
    {
        return classToId[entity.GetType()];
    }

    static EntityRegistry()
    {
        Register(typeof(EntityArrow), "Arrow", 10);
        Register(typeof(EntitySnowball), "Snowball", 11);
        Register(typeof(EntityItem), "Item", 1);
        Register(typeof(EntityPainting), "Painting", 9);
        Register(typeof(EntityLiving), "Mob", 48);
        Register(typeof(EntityMonster), "Monster", 49);
        Register(typeof(EntityCreeper), "Creeper", 50);
        Register(typeof(EntitySkeleton), "Skeleton", 51);
        Register(typeof(EntitySpider), "Spider", 52);
        Register(typeof(EntityGiantZombie), "Giant", 53);
        Register(typeof(EntityZombie), "Zombie", 54);
        Register(typeof(EntitySlime), "Slime", 55);
        Register(typeof(EntityGhast), "Ghast", 56);
        Register(typeof(EntityPigZombie), "PigZombie", 57);
        Register(typeof(EntityPig), "Pig", 90);
        Register(typeof(EntitySheep), "Sheep", 91);
        Register(typeof(EntityCow), "Cow", 92);
        Register(typeof(EntityChicken), "Chicken", 93);
        Register(typeof(EntitySquid), "Squid", 94);
        Register(typeof(EntityWolf), "Wolf", 95);
        Register(typeof(EntityTNTPrimed), "PrimedTnt", 20);
        Register(typeof(EntityFallingSand), "FallingSand", 21);
        Register(typeof(EntityMinecart), "Minecart", 40);
        Register(typeof(EntityBoat), "Boat", 41);
    }
}
