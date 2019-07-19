using System;
using System.Threading.Tasks;
using Example.API.Services;
using X.Spectator.Base;

namespace Example.API.Probes
{
    public interface IAServiceProbe : IProbe
    {
    }

    public class AServiceProbe : IAServiceProbe
    {
        private readonly IServiceA _service;

        public string Name => "A Service Probe";

        public AServiceProbe(IServiceA service)
        {
            _service = service;
        }

        public Task<ProbeResult> Check()
        {
            var result = new ProbeResult
            {
                Time = DateTime.UtcNow,
                ProbeName = this.Name,
                Success = false
            };

            try
            {
                if (_service.Count > 5)
                {
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Data = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
