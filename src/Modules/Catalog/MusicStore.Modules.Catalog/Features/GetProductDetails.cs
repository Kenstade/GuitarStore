using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Catalog.Data;
using MusicStore.Modules.Catalog.Errors;

namespace MusicStore.Modules.Catalog.Features;

internal sealed record GetProductDetailsRequest(string Id);

internal sealed class GetProductDetails : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("catalog/{id}", async ([AsParameters]GetProductDetailsRequest request, 
            CatalogDbContext dbContext, CancellationToken ct) =>  
        {
            var product = await dbContext.Products
                .AsNoTracking()
                .Where(p => p.Id == Guid.Parse(request.Id))
                .Select(p => new GetProductDetailsResponse
                (
                    p.Name,
                    p.Description,
                    p.Price,
                    p.Category.Name,
                    p.Brand.Name,
                    p.Specifications
                    .Select(ps => new ProductSpecSummary
                    (
                        ps.SpecificationType.Name, 
                        ps.Value
                    )).ToList()
                )).FirstOrDefaultAsync(ct);

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