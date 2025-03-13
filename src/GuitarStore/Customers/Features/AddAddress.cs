using FluentValidation;
using GuitarStore.Common.Web;
using GuitarStore.Customers.Models;
using GuitarStore.Data;

namespace GuitarStore.Customers.Features;

public sealed record AddAddressRequest(string City, string Street, string BuildingNumber, string Apartment);
internal sealed class AddAddress : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/add-address", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(AddAddressRequest request,AppDbContext dbContext, 
        IUserContextProvider userContext, IValidator<AddAddressRequest> validator)
    {
        var result = validator.Validate(request);
        if (!result.IsValid) return TypedResults.ValidationProblem(result.ToDictionary());
         
        var userId = userContext.GetUserId();

        await dbContext.Addresses.AddAsync(new Address
        {
            City = request.City,
            Street = request.Street,
            BuildingNumber = request.BuildingNumber,
            Apartment = request.Apartment,
            CustomerId = userId
        });

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();

    }
}

public class AddAddressRequestValidator : AbstractValidator<AddAddressRequest>
{
    public AddAddressRequestValidator()
    {
        RuleFor(a => a.City).NotEmpty().MaximumLength(100);
        RuleFor(a => a.Street).NotEmpty().MaximumLength(100);
        RuleFor(a => a.BuildingNumber).NotEmpty().MaximumLength(50);
        RuleFor(a => a.Apartment).NotEmpty().MaximumLength(100);
    }
}
