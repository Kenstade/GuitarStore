using GuitarStore.Modules.Catalog;
using GuitarStore.Modules.ShoppingCart;
using Microsoft.AspNetCore.Http.Features;
using BuildingBlocks.Web.OpenApi;
using Scalar.AspNetCore;
using BuildingBlocks.Core.Caching;
using BuildingBlocks.Core.Exceptions;
using Hangfire;
using BuildingBlocks.Core.Hangfire;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Monitoring;
using BuildingBlocks.Core.Security;
using BuildingBlocks.Web.MinimalApi;
using GuitarStore.Api.Extensions;
using GuitarStore.Modules.Identity;
using GuitarStore.Modules.Orders;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddModulesSettingsFile(
    builder.Environment.ContentRootPath,
    builder.Environment.EnvironmentName);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMessageBus(
    typeof(CatalogModule).Assembly,
    typeof(ShoppingCartModule).Assembly,
    typeof(OrdersModule).Assembly);

builder.Services.AddDistributedCache(builder.Configuration);
builder.Services.AddMonitoring(builder.Configuration);

builder.Services
    .AddCatalogModule(builder.Configuration)
    .AddShoppingCartModule(builder.Configuration)
    .AddOrdersModule(builder.Configuration)
    .AddIdentityModule(builder.Configuration);

builder.Services.AddCustomIdentity(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

builder.Services.AddCustomHangfire(builder.Configuration);

builder.Services.AddOpenApi(options => 
    options.AddKeycloakAuthentication(builder.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.OpenApiRoutePattern = "/openapi/{documentName}.json";
        options.Servers = [];
    });

    app.UseHangfireDashboard();
}

app.UseSerilogRequestLogging();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseCatalogModule(builder.Configuration)
   .UseShoppingCartModule()
   .UseOrdersModule()
   .UseIdentityModule(builder.Configuration);

app.MapEndpoints();

await app.RunAsync();
