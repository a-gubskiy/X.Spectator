using System.Threading.Tasks;

namespace X.Spectator.Base
{
    public interface IProbe
    {
        string Name { get; }
        
        Task<ProbeResult> Ready();
    }
}