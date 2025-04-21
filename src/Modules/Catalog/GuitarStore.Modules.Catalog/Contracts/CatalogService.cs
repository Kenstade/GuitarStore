using GuitarStore.Modules.Catalog.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Contracts;

internal sealed class CatalogService : ICatalogService
{
    private readonly CatalogDbContext _dbContext;
    public CatalogService(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ProductDetails?> GetProductForCartAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == productId)
            .Select(p => new ProductDetails
            (
                p.Name, 
                p.GetMainImage(),
                p.Price
            ))
            .FirstOrDefaultAsync(ct);
        
        return product;
    }
}   
