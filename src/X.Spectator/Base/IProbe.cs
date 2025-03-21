using JetBrains.Annotations;

namespace X.Spectator.Base;

[PublicAPI]
public interface IProbe
{
    /// <summary>
    /// Probe name
    /// </summary>
    string Name { get; }
        
    /// <summary>
    /// Execute probe
    /// </summary>
    /// <returns></returns>
    Task<ProbeResult> Check();
}