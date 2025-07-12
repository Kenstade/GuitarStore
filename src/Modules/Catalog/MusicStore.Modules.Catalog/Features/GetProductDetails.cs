using BuildingBlocks.Core.Dapper;
using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MusicStore.Modules.Catalog.Errors;

namespace MusicStore.Modules.Catalog.Features;

internal sealed record GetProductDetailsRequest(string Id);

internal sealed class GetProductDetails : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("catalog/{id}", async ([AsParameters]GetProductDetailsRequest request, 
                IDbConnectionFactory dbConnectionFactory,CancellationToken ct) =>  
        {
            await using var connection = await dbConnectionFactory.OpenConnectionAsync();

            const string sql =
                $"""
                 SELECT 
                 p.name as {nameof(GetProductDetailsResponse.Name)},
                 p.description as {nameof(GetProductDetailsResponse.Description)},
                 p.price as {nameof(GetProductDetailsResponse.Price)},
                 c.name as {nameof(GetProductDetailsResponse.Category)},
                 b.name as {nameof(GetProductDetailsResponse.Brand)},
                 (
                    SELECT jsonb_agg(jsonb_build_object(
                    'Type', st.name,
                    'Value', ps.value
                    ))
                    FROM catalog.product_specification ps
                    JOIN catalog.specification_type st ON ps.specification_type_id = st.id
                    WHERE ps.product_id = p.id
                 ) as Specs
                 FROM catalog.products p
                 JOIN catalog.categories c ON p.category_id = c.id
                 JOIN catalog.brands b ON p.brand_id = b.id
                 WHERE p.id = @requestId::uuid
                 """;
            
            var requestId = Guid.Parse(request.Id);
            var product = await connection.QueryFirstOrDefaultAsync(sql, new { requestId });

            return product is not null 
                ? Results.Ok(product)
                : Results.Problem(ProductErrors.NotFound(request.Id));
            
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetProductDetails>>()    
        .AddEndpointFilter<ValidationEndpointFilter<GetProductDetailsRequest>>()
        .WithTags("Catalog")
        .WithName("GetProductDetails")
        .WithSummary("Get product details by ID")
        .AllowAnonymous();
        
        return builder;
    }
}
internal sealed record GetProductDetailsResponse(
    string Name, 
    string? Description, 
    decimal Price,
    string Category,
    string Brand,
    ICollection<ProductSpecSummary> Specs);

internal sealed record ProductSpecSummary(string Type, string Value);

internal sealed class GetProductDetailsValidator : AbstractValidator<GetProductDetailsRequest>
{
    public GetProductDetailsValidator()
    {
        RuleFor(p => p.Id).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}