namespace BetaSharp.Modding;

[AttributeUsage(AttributeTargets.Assembly)]
public class ModInfoAttribute : Attribute
{
    public readonly string Name;
    public readonly string Description;
    public readonly string Author;

    /// <summary>
    /// Creates a new ModInfoAttribute with the specified name, description, and author.
    /// </summary>
    /// <param name="name">The name of the mod.</param>
    /// <param name="description">
    /// A description of the mod, can be as long or short as you want.
    /// The first line of the description will be shown in the mod list.
    /// All lines will be shown in the mod's page when it is clicked on.
    /// </param>
    /// <param name="author">The author of the mod.</param>
    public ModInfoAttribute(string name, string description, string author)
    {
        Name = name;
        Description = description;
        Author = author;
    }
}
