# X.Spectator

[![Sponsor on GitHub](https://img.shields.io/badge/Sponsor_on_GitHub-ff7f00?logo=github&logoColor=white&style=for-the-badge)](https://github.com/sponsors/a-gubskiy)
[![Subscribe on X](https://img.shields.io/badge/Subscribe_on_X-000000?logo=x&logoColor=white&style=for-the-badge)](https://x.com/andrew_gubskiy)
[![NuGet Downloads](https://img.shields.io/nuget/dt/X.Spectator?style=for-the-badge&label=NuGet%20Downloads&color=004880&logo=nuget&logoColor=white)](https://www.nuget.org/packages/X.Spectator)

**X.Spectator** is a lightweight and extensible **monitoring and health evaluation framework** for .NET applications.  
It provides a clean, event-driven model for observing system health, diagnostics, and performance — built around the concept of **Probes**, **Spectators**, and **State Evaluators**.

> Designed for developers who need a flexible, composable monitoring layer that integrates seamlessly with modern .NET health checks and diagnostics.


## ✨ Key Features

- **Native Integration** — fully aligned with .NET's built-in `HealthStatus` model.  
- **Probes & Spectators** — modular design for collecting, evaluating, and reacting to system metrics.  
- **Event-Driven Monitoring** — trigger state changes and health checks dynamically.
- **Flexible Extensions** — implement custom probes, evaluators, and journal strategies.  
- **Asynchronous Support** — works both in synchronous and background modes.  


## 🧠 Core Concepts

### **Probe**
Represents a single measurable system indicator (e.g., CPU usage, API latency, cache size).  
Each probe implements the `IProbe` interface and returns a `ProbeResult` containing:
- Probe name
- Execution timestamp
- `HealthCheckResult` value (with status, description, exception, and diagnostic data)

```csharp
public interface IProbe
{
    string Name { get; }
    Task<ProbeResult> Check();
}
```


### **Spectator**

An `ISpectator` instance aggregates multiple probes, polls them periodically, and raises events:
- **StateChanged** — when system health transitions
- **HealthChecked** — after each probe cycle

```csharp
public interface ISpectator<TState>
{
    event EventHandler<StateEventArgs<TState>> StateChanged;
    event EventHandler<HealthCheckEventArgs> HealthChecked;
    void AddProbe(IProbe probe);
    void CheckHealth();
}
```

Built-in implementations include:
- `SpectatorBase<TState>` — synchronous monitoring base class
- `AutomatedSpectator<TState>` — asynchronous, background monitoring (implements `IHostedService`)

### **State Evaluator**

Implements custom logic for aggregating probe results into a system-wide state.

```csharp
public interface IStateEvaluator<TState>
{
    TState Evaluate(TState currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal);
}
```

### **Journal**

A time-series record of probe snapshots used for retrospective analysis and state evaluation.

```csharp
public record JournalRecord
{
    public DateTime Time { get; init; }
    public IReadOnlyCollection<ProbeResult> Values { get; init; }
}
```


## ⚙️ Installation

Install via NuGet:

```bash
dotnet add package X.Spectator
```

Or update your project file:

```xml
<PackageReference Include="X.Spectator" Version="2.2.4" />
```

NuGet Package: https://www.nuget.org/packages/X.Spectator/


## 🧩 Example Usage

```csharp
// Create a state evaluator
var stateEvaluator = new MyHealthStatusEvaluator();

// Create an automated spectator
var spectator = new AutomatedSpectator<HealthStatus>(
    checkHealthPeriod: TimeSpan.FromSeconds(10),
    retentionPeriod: TimeSpan.FromMinutes(5),
    stateEvaluator: stateEvaluator,
    initialState: HealthStatus.Healthy
)
{
    Name = "AppSpectator"
};

// Add probes
spectator.AddProbe(new MemoryUsageProbe());

// Subscribe to events
spectator.StateChanged += (s, e) =>
{
    Console.WriteLine($"State changed to: {e.State}");
};

// Start monitoring (implements IHostedService)
await spectator.StartAsync(CancellationToken.None);
```


## 🧪 Recent Improvements

- Replaced custom enums with native .NET `HealthStatus`
- `ProbeResult` now wraps `HealthCheckResult` for full integration with .NET health checks
- Improved asynchronous monitoring model with `IHostedService` support
- Enhanced XML documentation and unit tests
- Support for .NET 8.0 and .NET 9.0

Full changelog: https://github.com/a-gubskiy/X.Spectator/releases


## 🤝 Contributing

Contributions are welcome!  
If you want to improve or extend X.Spectator, please follow the standard GitHub flow:

1. Fork the repository
2. Create a branch (`feature/YourFeature`)
3. Commit and push your changes
4. Open a Pull Request


## 🧭 Learn More

- 📖 Article: [X.Spectator 2.0 on Medium](https://medium.com/@andrew_gubskiy)
- 📦 [NuGet Package](https://www.nuget.org/packages/X.Spectator)
- 💻 [GitHub Repository](https://github.com/a-gubskiy/X.Spectator)


X.Spectator continues to evolve alongside .NET — bridging classic monitoring patterns with modern health diagnostics for high-reliability applications.