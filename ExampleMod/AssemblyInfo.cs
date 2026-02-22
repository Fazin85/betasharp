using System.Reflection;
using BetaSharp.Modding;

/* Some notes:
 * Unfortunately, .NET uses the Microsoft versioning scheme of major.minor.build.revision
 * rather than major.minor.revision.build or major.minor.patch.build. Whether you care is
 * up to you, but I personally recommend omitting the last version number and treating it
 * like major.minor.patch.
 *
 * The AssemblyVersion attribute determines assembly compatibility. If the major or minor
 * version numbers are changed, any assemblies that reference this mod must be recompiled
 * to work with the new version.
 *
 * The launcher and mods menu use the AssemblyInformationalVersion to display the version
 * of the mod. That should be used to specify the version you want your end users to see.
 */

[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: ModInfo("Example Mod", "CoolModder1337", "Surely does something really cool")]
