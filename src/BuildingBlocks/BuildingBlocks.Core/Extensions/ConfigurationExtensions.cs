﻿using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Core.Extensions;

internal static class ConfigurationExtensions
{
    internal static TModel? GetOptions<TModel>(this IConfiguration configuration, string section)
       where TModel : new()
    {
        var options = new TModel();

        var optionsSection = configuration.GetSection(section);
        optionsSection.Bind(options);

        return options;
    }
}
