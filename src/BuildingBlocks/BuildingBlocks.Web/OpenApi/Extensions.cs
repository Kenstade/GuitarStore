using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

namespace BuildingBlocks.Web.OpenApi;
public static class Extensions
{
    public static OpenApiOptions AddKeycloakAuthentication(this OpenApiOptions options, IConfiguration configuration)
    {
        var keycloakOptions = configuration.GetOptions<KeyCloakOptions>(nameof(KeyCloakOptions));
        
        var scheme = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.OAuth2,
            Name = "Authorization",
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(keycloakOptions.AuthorizationUrl),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "openid" },
                        { "profile", "profile" }
                    }
                }
            },
        };
        
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes.Add("Keycloak", scheme);
            document.SecurityRequirements = new List<OpenApiSecurityRequirement>
            {
                new() { { scheme, new List<string>() } }
            };
            
            return Task.CompletedTask;
        });
        
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                operation.Security = [new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Keycloak",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        []
                    }
                }];
            }
            return Task.CompletedTask;
        });
        
        return options;
    }
}
