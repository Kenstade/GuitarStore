namespace MusicStore.Modules.Order.Models;

internal sealed class Address
{
    public Address(string city, string street, string buildingNumber, string apartment)
    {
        City = city;
        Street = street;
        BuildingNumber = buildingNumber;
        Apartment = apartment;
    }
    
    public string City { get; set; }
    public string Street { get; set; } 
    public string BuildingNumber { get; set; }
    public string Apartment { get; set; } 
}