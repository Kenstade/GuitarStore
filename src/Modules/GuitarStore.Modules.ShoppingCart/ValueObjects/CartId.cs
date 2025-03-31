namespace GuitarStore.Modules.ShoppingCart.ValueObjects;

public readonly record struct CartId(Guid Value)
{
    public static CartId Empty => new(Guid.Empty);
    public static CartId NewCartId() => new(Guid.NewGuid());
}