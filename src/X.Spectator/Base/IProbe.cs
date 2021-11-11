using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Spectator.Base;

[PublicAPI]
public interface IProbe
{
    string Name { get; }
        
    Task<ProbeResult> Check();
}