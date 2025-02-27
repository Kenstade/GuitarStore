namespace GuitarStore.Customers.Models;

public class Customer
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public Address Address { get; set; } = default!;
}
