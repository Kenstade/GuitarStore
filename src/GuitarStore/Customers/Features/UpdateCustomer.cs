using FluentValidation;
using GuitarStore.Common.Web;
using GuitarStore.Customers.Errors;
using GuitarStore.Data;

namespace GuitarStore.Customers.Features;
//TODO: доделать валидацию и добавить поля
public sealed record UpdateCustomerRequest(string PhoneNumber, string FullName);
internal sealed class UpdateCustomer : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/update", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(UpdateCustomerRequest request, AppDbContext dbContext,
        IUserContextProvider userContext)
    {
        var userId = userContext.GetUserId();
        var customer = await dbContext.Customers
            .FindAsync(userId);

        if(customer == null) return TypedResults.Problem(new CustomerNotFoundError(userId));

        customer.PhoneNumber = request.PhoneNumber;
        customer.FullName = request.FullName;

        dbContext.Customers.Update(customer);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }
}

internal class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
{

}
