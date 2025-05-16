using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Hangfire;
using GuitarStore.Modules.Identity.BackgroundJobs;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.Identity.Extensions;

internal static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseBackgroundJobs(this IApplicationBuilder app, IConfiguration configuration)
    {
        var hangfireOptions = configuration.GetOptions<HangfireOptions>($"{IdentityModule.ModuleName}:{nameof(HangfireOptions)}");
        
        app.ApplicationServices
            .GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<ProcessOutboxMessageJob>("outbox-processor", job => 
                job.ProcessAsync(), hangfireOptions.OutboxSchedule);
        
        return app;
    }
}