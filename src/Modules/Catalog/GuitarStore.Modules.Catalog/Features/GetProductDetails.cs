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
public sealed record GetProductDetailsRequest(string ProductId);

internal sealed class GetProductDetails : IEndpoint
{
    private readonly CatalogDbContext _dbContext;

    public GetProductDetails(CatalogDbContext dbContext) 
    {
        _dbContext = dbContext;
    }

    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("catalog/{id}", async ([AsParameters] GetProductDetailsRequest request, CancellationToken ct) =>  
        {
            var parsedRequest = Guid.Parse(request.ProductId);
            
            return await Handle(parsedRequest, ct);
        })
        .AddEndpointFilter<LoggingEndpointFilter<GetProductDetailsRequest>>()    
        .AddEndpointFilter<ValidationEndpointFilter<GetProductDetailsRequest>>()
        .WithName("GetProductDetails")
        .WithTags("Catalog")
        .AllowAnonymous();
        
        return builder;
    }

    private async Task<IResult> Handle(Guid requestId, CancellationToken ct)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == requestId)
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

        return product != null ? TypedResults.Ok(product)
                               : TypedResults.Problem(new ProductNotFoundError(requestId));
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
        RuleFor(p => p.ProductId).NotEmpty().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
    }
}