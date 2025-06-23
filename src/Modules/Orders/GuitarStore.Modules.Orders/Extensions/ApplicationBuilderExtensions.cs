using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Hangfire;
using GuitarStore.Modules.Orders.BackgroundJobs;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.Orders.Extensions;

internal static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseBackgroundJobs(this IApplicationBuilder app, IConfiguration configuration)
    {
        var hangfireOptions = configuration.GetOptions<HangfireOptions>($"{Constants.ModuleName}:{nameof(HangfireOptions)}");
        
        app.ApplicationServices
            .GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<ProcessOutboxMessagesJob>($"{Constants.ModuleName}-outbox-processor", job => 
                job.ProcessAsync(), hangfireOptions.OutboxSchedule);
        
        return app;
    }
}