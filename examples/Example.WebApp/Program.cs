using Example.WebApp;
using Example.WebApp.Probes;
using Example.WebApp.Services;
using X.Spectator.Base;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddHostedService<CityHostedService>()
    .AddSingleton<LibraryService>()
    .AddSingleton<PublishingHouseService>()
    .AddSingleton<LibraryServiceProbe>(CreateLibraryServiceProbe)
    .AddSingleton<IStateEvaluator<SystemState>, SystemStateEvaluator>()
    .AddSingleton<SystemSpectator>(CreateSystemSpectator);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static LibraryServiceProbe CreateLibraryServiceProbe(IServiceProvider ctx)
{
    var minimumBookCount = 20;
    var libraryService = ctx.GetService<LibraryService>();
            
    return new LibraryServiceProbe(libraryService, minimumBookCount);
}

static SystemSpectator CreateSystemSpectator(IServiceProvider ctx)
{
    var stateEvaluator = ctx.GetService<IStateEvaluator<SystemState>>();
    var libraryServiceProbe = ctx.GetService<LibraryServiceProbe>();
    var retentionPeriod = TimeSpan.FromMinutes(5);
    var checkHealthPeriod = TimeSpan.FromMilliseconds(500);
    var spectator = new SystemSpectator(checkHealthPeriod, stateEvaluator, retentionPeriod, SystemState.Normal);
            
    spectator.AddProbe(libraryServiceProbe);
            
    spectator.Start();
            
    return spectator;
}