﻿using BuildingBlocks.Core.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Core.Security;

public static class Extensions
{
    public static IServiceCollection AddCustomIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetOptions<JwtOptions>(nameof(JwtOptions));
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.Audience = jwtOptions.Audience;
                o.MetadataAddress = jwtOptions.MetadataAddress;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtOptions.ValidIssuer
                };
            });
        services.AddAuthorization();
        return services;
    }
}