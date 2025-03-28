using GuitarStore.Modules.Catalog.ValueObjects;

namespace GuitarStore.Modules.Catalog.Models;
public sealed class ProductImage
{
    public ProductImage(string imageUrl, bool isMain, ProductId productId)
    {
        ImageUrl = imageUrl;
        IsMain = isMain;
        ProductId = productId;
    }
    private ProductImage() { }
    public int Id { get; private set; }
    public string ImageUrl { get; private set; } = default!;
    public bool IsMain { get; private set; }
    public ProductId ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
}
