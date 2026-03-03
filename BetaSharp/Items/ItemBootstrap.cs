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
                var id = new Identifier(Identifier.DefaultNamespace, field.Name.ToLowerInvariant());
                Registries.Items.Register(id, item);
            }
        }
    }
}

