using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace X.Spectator.Base;

public interface IHealthStatusSpectator : ISpectator<HealthStatus>;