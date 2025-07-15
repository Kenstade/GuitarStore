using MusicStore.Modules.Catalog;
using MusicStore.Modules.ShoppingCart;
using BuildingBlocks.Web.OpenApi;
using BuildingBlocks.Core.Caching;
using BuildingBlocks.Core.Dapper;
using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.ErrorHandling;
using Hangfire;
using BuildingBlocks.Core.Hangfire;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Monitoring;
using BuildingBlocks.Core.Security;
using BuildingBlocks.Web.Extensions;
using BuildingBlocks.Web.MinimalApi;
using MusicStore.Api.Extensions;
using MusicStore.Modules.Customer;
using MusicStore.Modules.Identity;
using MusicStore.Modules.Order;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddModulesSettingsFile(
    builder.Environment.ContentRootPath,
    builder.Environment.EnvironmentName);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMessageBus(cfg => cfg
    .AddCatalogModuleConsumers()
    .AddOrdersModuleConsumers(builder.Configuration)
    .AddCustomerModuleConsumers()
    .AddShoppingCartModuleConsumers());

builder.Services.AddDistributedCache(builder.Configuration);
builder.Services.AddMonitoring(builder.Configuration);
builder.Services.AddCustomIdentity(builder.Configuration);
builder.Services.AddCustomHangfire(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddDbConnectionFactory(builder.Configuration);

builder.Services.RegisterEfCoreInterceptors();

builder.Services
    .AddCatalogModule(builder.Configuration)
    .AddShoppingCartModule(builder.Configuration)
    .AddOrdersModule(builder.Configuration)
    .AddIdentityModule(builder.Configuration)
    .AddCustomerModule(builder.Configuration);

builder.Services.AddProblemDetails(options => options.AddCustomProblemDetails());

builder.Services.AddOpenApi(options => options.AddKeycloakAuthentication(builder.Configuration));

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseCatalogModule(builder.Configuration)
   .UseShoppingCartModule()
   .UseOrdersModule(builder.Configuration)
   .UseIdentityModule(builder.Configuration)
   .UseCustomerModule();

app.MapEndpoints();

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("openapi/v1.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHangfireDashboard();

app.UseMonitoring();

await app.RunAsync();
