namespace BetaSharp.Registry;

public readonly struct Identifier : IEquatable<Identifier>
{
    public string Namespace { get; }
    public string Path { get; }

    public Identifier(string @namespace, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or whitespace.", nameof(path));

        Namespace = string.IsNullOrWhiteSpace(@namespace) ? DefaultNamespace : @namespace;
        Path = path;
    }

    public static string DefaultNamespace => "betasharp";

    public static Identifier Parse(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        value = value.Trim();
        if (value.Length == 0)
            throw new ArgumentException("Identifier cannot be empty.", nameof(value));

        int sepIndex = value.IndexOf(':');
        if (sepIndex < 0)
        {
            return new Identifier(DefaultNamespace, value);
        }

        if (sepIndex == 0 || sepIndex == value.Length - 1)
            throw new ArgumentException($"Invalid identifier '{value}'. Expected 'namespace:path' or 'path'.", nameof(value));

        var ns = value[..sepIndex];
        var path = value[(sepIndex + 1)..];
        return new Identifier(ns, path);
    }

    public override string ToString() => $"{Namespace}:{Path}";

    public bool Equals(Identifier other)
    {
        return string.Equals(Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase)
               && string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => obj is Identifier other && Equals(other);

    public override int GetHashCode()
    {
        return HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(Namespace),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Path));
    }

    public static bool operator ==(Identifier left, Identifier right) => left.Equals(right);
    public static bool operator !=(Identifier left, Identifier right) => !left.Equals(right);
}

