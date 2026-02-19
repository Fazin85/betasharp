using BetaSharp.NBT;
using BetaSharp.Worlds;
using java.lang;
using java.util;
using Exception = java.lang.Exception;

namespace BetaSharp.Entities;

public class EntityRegistry
{
    private static Map idToClass = new HashMap();
    private static Map classToId = new HashMap();
    private static Map rawIdToClass = new HashMap();
    private static Map classToRawId = new HashMap();
    public static Dictionary<string, int> namesToId = new();

    private static void register(Class entityClass, string id, int rawId)
    {
        idToClass.put(id, entityClass);
        classToId.put(entityClass, id);
        rawIdToClass.put(Integer.valueOf(rawId), entityClass);
        classToRawId.put(entityClass, Integer.valueOf(rawId));
        namesToId.TryAdd(id.ToLower(), rawId);
    }

    public static Entity create(string id, World world)
    {
        Entity? entity = null;

        try
        {
            Class entityClass = (Class)idToClass.get(id);
            if (entityClass != null)
            {
                entity = (Entity)entityClass.getConstructor(World.Class).newInstance(world);
            }
        }
        catch (Exception ex)
        {
            ex.printStackTrace();
        }

        return entity;
    }

    public static Entity getEntityFromNbt(NBTTagCompound nbt, World world)
    {
        Entity entity = null;

        try
        {
            Class entityClass = (Class)idToClass.get(nbt.GetString("id"));
            if (entityClass != null)
            {
                entity = (Entity)entityClass.getConstructor(World.Class).newInstance(world);
            }
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
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
                Class entityClass = (Class)rawIdToClass.get(Integer.valueOf(id));
                if (entityClass != null)
                {
                    var entity = (Entity)entityClass.getConstructor(World.Class).newInstance(world);

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
            Class entityClass = (Class)rawIdToClass.get(Integer.valueOf(rawId));
            if (entityClass != null)
            {
                entity = (Entity)entityClass.getConstructor(World.Class).newInstance(world);
            }
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

    public static int getRawId(Entity entity)
    {
        return ((Integer)classToRawId.get(entity.getClass())).intValue();
    }

    public static string getId(Entity entity)
    {
        return (string)classToId.get(entity.getClass());
    }

    static EntityRegistry()
    {
        register(EntityArrow.Class, "Arrow", 10);
        register(EntitySnowball.Class, "Snowball", 11);
        register(EntityItem.Class, "Item", 1);
        register(EntityPainting.Class, "Painting", 9);
        register(EntityLiving.Class, "Mob", 48);
        register(EntityMonster.Class, "Monster", 49);
        register(EntityCreeper.Class, "Creeper", 50);
        register(EntitySkeleton.Class, "Skeleton", 51);
        register(EntitySpider.Class, "Spider", 52);
        register(EntityGiantZombie.Class, "Giant", 53);
        register(EntityZombie.Class, "Zombie", 54);
        register(EntitySlime.Class, "Slime", 55);
        register(EntityGhast.Class, "Ghast", 56);
        register(EntityPigZombie.Class, "PigZombie", 57);
        register(EntityPig.Class, "Pig", 90);
        register(EntitySheep.Class, "Sheep", 91);
        register(EntityCow.Class, "Cow", 92);
        register(EntityChicken.Class, "Chicken", 93);
        register(EntitySquid.Class, "Squid", 94);
        register(EntityWolf.Class, "Wolf", 95);
        register(EntityTNTPrimed.Class, "PrimedTnt", 20);
        register(EntityFallingSand.Class, "FallingSand", 21);
        register(EntityMinecart.Class, "Minecart", 40);
        register(EntityBoat.Class, "Boat", 41);
    }
}
