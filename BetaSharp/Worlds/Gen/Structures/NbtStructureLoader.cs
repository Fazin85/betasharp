using System.IO;
using BetaSharp;
using BetaSharp.NBT;
using BetaSharp.Util;
using BetaSharp.Worlds.Gen.Structures;
using java.io;
using static BetaSharp.Worlds.Gen.Structures.NbtStructure;
public static class NbtStructureLoader
{
    public static NbtStructure load(string path)
    {
        using FileStream stream = System.IO.File.OpenRead(path);
        NBTTagCompound nbt = NbtIo.Read(stream);
        List<StructureEntity> entities = new List<StructureEntity>();
        string name = nbt.GetString("Name");
        int width = nbt.GetInteger("Width");
        int height = nbt.GetInteger("Height");
        int depth = nbt.GetInteger("Depth");
        int[] blocks = nbt.GetByteArray("Blocks").Select(b => b & 0xFF).ToArray();
        int[] data = nbt.GetByteArray("Data").Select(b => b & 0xFF).ToArray();
        NBTTagList entityList = nbt.GetTagList("Entities");
        for (int i = 0; i < entityList.TagCount(); i++)
        {
            NBTTagCompound entNbt = (NBTTagCompound)entityList.TagAt(i);
            entities.Add(new StructureEntity
            {
                EntityId = entNbt.GetString("Id"),
                LocalX = entNbt.GetFloat("LocalX"),
                LocalY = entNbt.GetFloat("LocalY"),
                LocalZ = entNbt.GetFloat("LocalZ"),
                ExtraData = entNbt.HasKey("Data") ? entNbt.GetCompoundTag("Data") : null
            });


            
        }
            return new NbtStructure(name, width, height, depth, blocks, data, entities);
    }

    public static void save(NbtStructure structure, Stream stream)
    {
        NBTTagCompound nbt = new NBTTagCompound();
        nbt.SetString("Name", structure.Name);
        nbt.SetInteger("Width", structure.Width);
        nbt.SetInteger("Height", structure.Height);
        nbt.SetInteger("Depth", structure.Depth);
        nbt.SetByteArray("Blocks", structure.blocksIds);
        nbt.SetByteArray("Data", structure.blockMetas);
        NBTTagList entityList = new NBTTagList();
        foreach (StructureEntity se in structure.ents)
        {
            NBTTagCompound entNbt = new NBTTagCompound();
            entNbt.SetString("Id", se.EntityId);
            entNbt.SetFloat("LocalX", se.LocalX);
            entNbt.SetFloat("LocalY", se.LocalY);
            entNbt.SetFloat("LocalZ", se.LocalZ);
            if (se.ExtraData != null)
                entNbt.SetCompoundTag("Data", se.ExtraData);
            entityList.SetTag(entNbt);
        }
        
        //nbt.SetByteArray("Entities", structure.ents);
        // tu devras exposer les arrays via des getters ou les passer en paramètre
        NbtIo.Write(nbt, stream);
    }
}
