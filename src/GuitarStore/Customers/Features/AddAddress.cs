﻿using System.Security.Claims;
using FluentValidation;
using GuitarStore.Common;
using GuitarStore.Common.Extensions;
using GuitarStore.Common.Interfaces;
using GuitarStore.Customers.Models;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Customers.Features;

public record AddAddressRequest(string City, string Street, string BuildingNumber, string Apartment);
public class AddAddress : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/add-address", HandleAsync)
        .RequireAuthorization();
    private static async Task<IResult> HandleAsync(AddAddressRequest request,AppDbContext dbContext, 
        IUserContextProvider userContext, IValidator<AddAddressRequest> validator)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) return TypedResults.ValidationProblem(result.ToDictionary());

        var userId = userContext.GetUserId();

        if (await dbContext.Addresses.AnyAsync(a => a.CustomerId == userId))
            return TypedResults.BadRequest("Your already have an address");

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
