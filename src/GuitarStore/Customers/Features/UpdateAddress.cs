using FluentValidation;
using GuitarStore.Common.Web;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Customers.Features;

public sealed record UpdateAddressRequest(string City, string Street, string BuildingNumber, string Apartment);
internal sealed class UpdateAddress : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/update-address", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(UpdateAddressRequest request, AppDbContext dbContext,
        IUserContextProvider userContext, IValidator<UpdateAddressRequest> validator)
    {
        var result = validator.Validate(request);
        if (!result.IsValid) return TypedResults.ValidationProblem(result.ToDictionary());

        var userId = userContext.GetUserId();

        var address = await dbContext.Addresses.SingleOrDefaultAsync(a => a.CustomerId == userId);
        if (address == null) return TypedResults.NotFound("Address not found");

        address.City = request.City;
        address.Street = request.Street;
        address.BuildingNumber = request.BuildingNumber;
        address.Apartment = request.Apartment;

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }
}

public class UpdateAddressRequestValidator : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressRequestValidator()
    {
        RuleFor(a => a.City).NotEmpty().MaximumLength(100);
        RuleFor(a => a.Street).NotEmpty().MaximumLength(100);
        RuleFor(a => a.BuildingNumber).NotEmpty().MaximumLength(50);
        RuleFor(a => a.Apartment).NotEmpty().MaximumLength(100);
    }
}
