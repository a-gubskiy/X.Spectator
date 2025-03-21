using System;
using System.Threading;
using System.Threading.Tasks;
using Example.App.Probes;
using Example.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using X.Spectator.Base;

namespace Example.App;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services
                    .AddHostedService<CityHostedService>()
                    .AddSingleton<LibraryService>()
                    .AddSingleton<PublishingHouseService>()
                    .AddSingleton<LibraryServiceProbe>(CreateLibraryServiceProbe)
                    .AddSingleton<IStateEvaluator<HealthStatus>, SystemStateEvaluator>()
                    .AddSingleton<SystemSpectator>(CreateSystemSpectator);
            })
            .Build();

        await host.RunAsync();
    }

    private static LibraryServiceProbe CreateLibraryServiceProbe(IServiceProvider ctx)
    {
        var minimumBookCount = 20;
        var libraryService = ctx.GetService<LibraryService>();
            
        return new LibraryServiceProbe(libraryService, minimumBookCount);
    }

    private static SystemSpectator CreateSystemSpectator(IServiceProvider ctx)
    {
        var stateEvaluator = ctx.GetService<IStateEvaluator<HealthStatus>>();
        var libraryServiceProbe = ctx.GetService<LibraryServiceProbe>();
        var retentionPeriod = TimeSpan.FromMinutes(5);
        var checkHealthPeriod = TimeSpan.FromMilliseconds(500);
        var spectator = new SystemSpectator(checkHealthPeriod, stateEvaluator, retentionPeriod, HealthStatus.Healthy);
            
        spectator.AddProbe(libraryServiceProbe);

        spectator.StartAsync(CancellationToken.None);
            
        return spectator;
    }
}