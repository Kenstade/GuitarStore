using GuitarStore.Modules.Catalog.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Contracts;

public interface ICatalogService
{
    Task<OrderDetails?> GetProductForCart(Guid id, CancellationToken ct = default);
}

internal sealed class CatalogService : ICatalogService
{
    private readonly CatalogDbContext _dbContext;
    public CatalogService(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<OrderDetails?> GetProductForCart(Guid productId, CancellationToken ct = default)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == productId)
            .Select(p => new OrderDetails
            (
                p.Name, 
                p.GetMainImage(),
                p.Price
            ))
            .FirstOrDefaultAsync(ct);
        
        return product;
    }
}   
