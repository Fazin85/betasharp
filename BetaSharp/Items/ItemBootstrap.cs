using BetaSharp.Registry;

namespace BetaSharp.Items;

public static class ItemBootstrap
{
    public static void RegisterAll()
    {
        foreach (var field in typeof(Item).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
        {
            if (typeof(Item).IsAssignableFrom(field.FieldType) && field.GetValue(null) is Item item)
            {
                var path = IdUtils.ToSnakeCase(field.Name);
                var id = new ResourceLocation(ResourceLocation.DefaultNamespace, path);
                Registries.Items.Register(id, item);
            }
        }
    }
}

