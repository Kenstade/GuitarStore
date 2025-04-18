namespace GuitarStore.Api.Extensions;

internal static class ModuleExtensions
{
    public static void AddModulesSettingsFile(this ConfigurationManager configurationManager, string root,
        string environment)
    {
        foreach (string file in Directory.GetFiles(root, "*.appsettings.json", SearchOption.AllDirectories))
        {
            configurationManager.AddJsonFile(file);
        }
        
        foreach (string file in Directory.GetFiles(
                     root,
                     $"*.appsettings.{environment}.json",
                     SearchOption.AllDirectories))
        {
            configurationManager.AddJsonFile(file, true, true);
        }
    }
}