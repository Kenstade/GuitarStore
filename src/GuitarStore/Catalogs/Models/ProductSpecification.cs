namespace GuitarStore.Catalogs.Models;

public class ProductSpecification
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public int SpecificationTypeId { get; set; }
    public SpecificationType SpecificationType { get; set; } = default!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;
}
