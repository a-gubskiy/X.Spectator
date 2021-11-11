using System.Timers;
using X.Spectator.Base;

namespace Example.WebApp.Services;

/// <summary>
/// Example service
/// </summary>
public class CityHostedService : IHostedService
{
    private readonly System.Timers.Timer _timer;
    private readonly LibraryService _library;
    private readonly PublishingHouseService _publishingHouse;
        
    private readonly SystemSpectator _spectator;

    public CityHostedService(
        LibraryService library, 
        PublishingHouseService publishingHouse, 
        SystemSpectator spectator)
    {
        _library = library;
        _publishingHouse = publishingHouse;
        _spectator = spectator;

        var interval = TimeSpan.FromSeconds(5);
            
        _timer = new System.Timers.Timer(interval.TotalMilliseconds);
        _timer.Elapsed += TimerOnElapsed;
            
        _spectator.StateChanged += SpectatorOnStateChanged;
        _spectator.HealthChecked += SpectatorOnHealthChecked;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    private void SpectatorOnHealthChecked(object sender, HealthCheckEventArgs e)
    {
        Console.WriteLine($"Health checked: [{string.Join(", ", e.Results)}]");
    }

    private void SpectatorOnStateChanged(object sender, StateEventArgs<SystemState> e)
    {
        Console.ForegroundColor = e.State == SystemState.Danger ? ConsoleColor.Red : ConsoleColor.Green;
        Console.WriteLine($"State changed to: {e.State}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _timer.Start();
    }

    private void TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        GetBookFromLibrary();

        if (_spectator.State == SystemState.Danger)
        {
            GetNewBooksFromPublishingHouse();
        }
    }

    private void GetNewBooksFromPublishingHouse()
    {
        var booksCount = _publishingHouse.PublishBooks();
        _library.AddBooks(booksCount);
            
        Console.WriteLine($"{booksCount} new books were delivered to the library");
    }

    private void GetBookFromLibrary()
    {
        _library.Read();
            
        Console.WriteLine($"One book was taken from the library. {_library.TotalBookCount} books left");
    }

    public async Task StopAsync(CancellationToken cancellationToken) => _timer.Stop();
}