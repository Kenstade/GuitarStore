using FluentValidation;
using GuitarStore;
using GuitarStore.Common.Caching;
using GuitarStore.Common.Core;
using GuitarStore.Common.Events;
using GuitarStore.Common.OpenApi;
using GuitarStore.Common.Web;
using GuitarStore.Data.Extensions;
using GuitarStore.Identity.Extensions;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPostgresDbContext(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
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

builder.Services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
builder.Services.AddScoped<IUserContextProvider, UserContextProvider>();
builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>(includeInternalTypes: true);

builder.Services.AddOpenApi(options => options.AddBearerTokenAuthentication());

builder.Services.AddTransient<INotifier, Notifier>();
//event handlers registration
foreach (var type in typeof(Program).Assembly.GetTypes()
    .Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() 
    == typeof(IEventHandler<>)) && !x.IsAbstract && !x.IsInterface))
{
    builder.Services.AddTransient(type.GetInterfaces().First(x => x.GetGenericTypeDefinition() 
    == typeof(IEventHandler<>)), type);
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("openapi/v1.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseDataSeeders().GetAwaiter().GetResult();

app.MapEndpoints(); 

app.Run();
