using BetaSharp.Registry;

namespace BetaSharp.Blocks;

public static class BlockBootstrap
{
    public static void RegisterAll()
    {
        foreach (var field in typeof(Block).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
        {
            if (typeof(Block).IsAssignableFrom(field.FieldType) && field.GetValue(null) is Block block)
            {
                var path = IdUtils.ToSnakeCase(field.Name);
                var id = new ResourceLocation(ResourceLocation.DefaultNamespace, path);
                Registries.Blocks.Register(id, block);
            }
        }
    }
}

