﻿using GuitarStore.Common;
using GuitarStore.Common.Interfaces;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

//namespace GuitarStore.Customers.Features;

//public record UpdateCustomerInfoRequest( string PhoneNumber, string FullName);
//public class UpdateCustomerInfo : IEndpoint
//{
//    public static void Map(IEndpointRouteBuilder app) => app
//        .MapPut("/update", HandleAsync)
//        .RequireAuthorization();
//    private static async Task<IResult> HandleAsync(UpdateCustomerInfoRequest request, AppDbContext dbContext, 
//        IUserContextProvider userContext)
//    {
//        var userId = userContext.GetUserId();
//        var customer = await dbContext.Customers
//            .SingleOrDefaultAsync(c => c.Id == userId);



//        customer.PhoneNumber = request.PhoneNumber;
//        customer.FullName = request.FullName;

//        dbContext.Customers.Update(customer);
//        await dbContext.SaveChangesAsync();


//        return TypedResults.Ok();
//    }
//}
