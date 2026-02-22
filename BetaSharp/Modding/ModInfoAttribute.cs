namespace BetaSharp.Modding;

[AttributeUsage(AttributeTargets.Assembly)]
public class ModInfoAttribute : Attribute
{
    public readonly string Name;
    public readonly string Description;
    public readonly string Author;

    public ModInfoAttribute(string name, string description, string author)
    {
        Name = name;
        Description = description;
        Author = author;
    }
}
