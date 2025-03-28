﻿namespace GuitarStore.Modules.Catalog.ValueObjects;
public readonly record struct ProductId(Guid Value)
{
    public static ProductId Empty => new(Guid.Empty);
    public static ProductId NewProductId() => new(Guid.NewGuid());
}
