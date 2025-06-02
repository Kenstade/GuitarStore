using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Core.Extensions;

public static class ConfigurationExtensions
{
    public static TModel GetOptions<TModel>(this IConfiguration configuration, string section)
       where TModel : new()
    {
        var options = new TModel();

        var optionsSection = configuration.GetSection(section);
        
        if (optionsSection == null)
        {
            throw new InvalidOperationException(
                $"Configuration section '{section}' not found for type {typeof(TModel).Name}");
        }
            
        optionsSection.Bind(options);

        return options;
    }
}
