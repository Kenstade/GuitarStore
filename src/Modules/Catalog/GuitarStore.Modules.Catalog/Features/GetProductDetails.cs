using BuildingBlocks.Core.Logging;
using BuildingBlocks.Core.Validation;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Catalog.Data;
using GuitarStore.Modules.Catalog.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Features;

public sealed record GetProductDetailsRequest(string Id);

internal sealed class GetProductDetails : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("catalog/{id}", async ([AsParameters]GetProductDetailsRequest request, 
            CatalogDbContext dbContext, CancellationToken ct) =>  
        {
            var parsedRequestId = Guid.Parse(request.Id);
            
            var product = await dbContext.Products
                .AsNoTracking()
                .Where(p => p.Id == parsedRequestId)
                .Select(p => new GetProductDetailsResponse
                (
                    p.Name,
                    p.Description,
                    p.Price,
                    p.Category.Name,
                    p.Brand.Name,
                    p.Specifications
                    .Select(ps => new ProductSpecPartialResponse
                    (
                        ps.SpecificationType.Name, 
                        ps.Value
                    )).ToList()
                )).FirstOrDefaultAsync(ct);

            return product != null ? Results.Ok(product)
                                   : Results.Problem(ProductErrors.NotFound(request.Id));
            
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetProductDetailsRequest>>()    
        .AddEndpointFilter<ValidationEndpointFilter<GetProductDetailsRequest>>()
        .WithName("GetProductDetails")
        .WithTags("Catalog")
        .AllowAnonymous();
        
        return builder;
    }
}
public sealed record GetProductDetailsResponse(
    string Name, 
    string? Description, 
    decimal Price,
    string Category,
    string Brand,
    ICollection<ProductSpecPartialResponse> Specs);

public sealed record ProductSpecPartialResponse(string Type, string Value);

public sealed class GetProductDetailsValidator : AbstractValidator<GetProductDetailsRequest>
{
    public GetProductDetailsValidator()
    {
        RuleFor(p => p.Id).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}