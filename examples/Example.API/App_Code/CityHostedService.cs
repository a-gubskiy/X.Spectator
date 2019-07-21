using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Example.API.Services;
using Microsoft.Extensions.Hosting;
using X.Spectator.Base;


namespace Example.API
{
    public class CityHostedService : IHostedService
    {
        private System.Timers.Timer _timer;
        private readonly ILibraryService _library;
        private readonly IPublishingHouseService _publishingHouse;
        private readonly SystemSpectator _spectator;

        public CityHostedService(
            ILibraryService library, 
            IPublishingHouseService publishingHouse, 
            SystemSpectator spectator)
        {
            _library = library;
            _publishingHouse = publishingHouse;
            _spectator = spectator;
            
            _spectator.StateChanged += SpectatorOnStateChanged;
            _spectator.HealthChecked += SpectatorOnHealthChecked;
        }

        private void SpectatorOnHealthChecked(object sender, HealthCheckEventArgs e)
        {
            Trace.WriteLine($"Health checked: [{string.Join(", ", e.Results)}]");
        }

        private void SpectatorOnStateChanged(object sender, StateEventArgs<SystemState> e)
        {
            Trace.WriteLine($"State changed: {e.State}");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            double interval = TimeSpan.FromSeconds(30).Milliseconds;
            
            _timer = new System.Timers.Timer(interval);
            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = false;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _library.Read();

            if (_spectator.State == SystemState.Danger)
            {
                _library.AddBooks(_publishingHouse.PublishBooks());
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Stop();
        }
    }
}