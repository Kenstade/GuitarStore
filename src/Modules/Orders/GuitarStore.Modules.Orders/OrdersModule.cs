﻿using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Orders.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.Orders;
public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<OrdersDbContext>(configuration);
        
        services.AddValidatorsFromAssembly(typeof(OrdersModule).Assembly, includeInternalTypes: true);
        
        services.AddMinimalApiEndpoints(typeof(OrdersModule).Assembly);
        
        return services;
    }

    public static IApplicationBuilder UseOrdersModule(this IApplicationBuilder app)
    {
        app.UseMigration<OrdersDbContext>();
        
        return app;
    }
}
