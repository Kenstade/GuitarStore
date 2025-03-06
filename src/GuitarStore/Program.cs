using FluentValidation;
using GuitarStore;
using GuitarStore.Common.Caching;
using GuitarStore.Common.Core;
using GuitarStore.Common.Events;
using GuitarStore.Common.OpenApi;
using GuitarStore.Common.Web;
using GuitarStore.Data.Extensions;
using GuitarStore.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPostgresDbContext(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
builder.Services.AddScoped<IUserContextProvider, UserContextProvider>();
builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>(includeInternalTypes: true);

builder.Services.AddOpenApi(options => options.AddBearerTokenAuthentication());

builder.Services.AddTransient<INotifier, Notifier>();
//event handlers registration
foreach (var type in typeof(Program).Assembly.GetTypes()
    .Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() 
    == typeof(INotificationHandler<>)) && !x.IsAbstract && !x.IsInterface))
{
    builder.Services.AddTransient(type.GetInterfaces().First(x => x.GetGenericTypeDefinition() 
    == typeof(INotificationHandler<>)), type);
}

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("openapi/v1.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseDataSeeders().GetAwaiter().GetResult();

app.MapEndpoints(); 

app.Run();
