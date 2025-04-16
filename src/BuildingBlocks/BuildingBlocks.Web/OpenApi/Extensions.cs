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
        var keycloakOptions = configuration.GetOptions<KeycloakOptions>(nameof(KeycloakOptions));
        
        var scheme = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.OAuth2,
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

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Keycloak",
                        Type = ReferenceType.SecurityScheme
                    },
                    In = ParameterLocation.Header,
                    Name = "Bearer",
                    Scheme = "Bearer",
                },
                []
            }
        };
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes.Add("Keycloak", scheme);
            document.SecurityRequirements.Add(securityRequirement);
            return Task.CompletedTask;
        });
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                operation.Security = [new OpenApiSecurityRequirement { [scheme] = [] }];
            }
            return Task.CompletedTask;
        });
        return options;
    }
}
