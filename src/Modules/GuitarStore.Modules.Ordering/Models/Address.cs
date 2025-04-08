namespace GuitarStore.Modules.Ordering.Models;

internal sealed class Address
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string BuildingNumber { get; set; } = string.Empty;
    public string Apartment { get; set; } = string.Empty;
}
