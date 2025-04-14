using GuitarStore.Modules.Catalog;
using GuitarStore.Modules.ShoppingCart;
using GuitarStore.Modules.Customers;
using GuitarStore.Modules.Ordering;
using GuitarStore.Modules.Identity;
using Microsoft.AspNetCore.Http.Features;
using BuildingBlocks.Web.OpenApi;
using BuildingBlocks.Web;
using Scalar.AspNetCore;
using BuildingBlocks.Core.Caching;
using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Core.Messaging.Outbox;
using Hangfire;
using BuildingBlocks.Core.Hangfire;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Web.MinimalApi;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
builder.Services.AddMessageBroker(typeof(CatalogModule).Assembly, typeof(ShoppingCartModule).Assembly);

builder.Services.AddPostgresDbContext<MessageDbContext>(builder.Configuration);
builder.Services.AddDistributedCache(builder.Configuration);

builder.Services
    .AddCatalogModule(builder.Configuration)
    .AddShoppingCartModule(builder.Configuration)
    .AddCustomersModule(builder.Configuration)
    .AddOrdersModule(builder.Configuration)
    .AddIdentityModule(builder.Configuration);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextProvider, UserContextProvider>();

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
    options.AddBearerTokenAuthentication());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.OpenApiRoutePattern = "/openapi/{documentName}.json";
    });

    app.UseHangfireDashboard(options: new DashboardOptions
    {
        Authorization = [],
        DarkModeEnabled = true,
    });
}
app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseBackgroundJobs(builder.Configuration);

app.UseCatalogModule()
   .UseShoppingCartModule();

app.MapEndpoints();

app.Run();
