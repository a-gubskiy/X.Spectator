using System;
using System.Threading.Tasks;
using Example.API.Services;
using X.Spectator.Base;

namespace Example.API.Probes
{
    public interface IBServiceProbe : IProbe
    {
    }

    public class BServiceProbe : IBServiceProbe
    {
        private readonly IServiceB _service;

        public string Name => "A Service Probe";

        public BServiceProbe(IServiceB service)
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
                if (_service.ConnectedToServer == true)
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
