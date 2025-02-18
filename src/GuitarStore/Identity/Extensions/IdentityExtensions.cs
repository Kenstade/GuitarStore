using System.Text;
using GuitarStore.Common.Extensions;
using GuitarStore.Identity.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GuitarStore.Identity.Extensions;

internal static class IdentityExtensions
{
    internal static IServiceCollection AddJwtConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<JwtService>();

        var jwtOptions = configuration.GetOptions<JwtOptions>(nameof(JwtOptions));
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        if (jwtOptions == null) throw new ArgumentNullException(nameof(jwtOptions));

        services.AddAuthentication(options =>
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };

                //options.MapInboundClaims = false;
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
               
            });

        services.AddAuthorization();

        return services;
    }    
}
