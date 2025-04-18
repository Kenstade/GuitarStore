namespace GuitarStore.Api.Extensions;

internal static class ModuleExtensions
{
    internal static void AddModulesSettingsFile(this ConfigurationManager configurationManager, string root,
        string environment)
    {
        foreach (var file in Directory.GetFiles(root, "*.appsettings.json", SearchOption.AllDirectories))
        {
            configurationManager.AddJsonFile(file);
        }
        
        foreach (var file in Directory.GetFiles(root,$"*.appsettings.{environment}.json",SearchOption.AllDirectories))
        {
            configurationManager.AddJsonFile(file, true, true);
        }
    }
}