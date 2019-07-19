using Example.API.App_Code.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.Spectator.Base;

namespace Example.API.App_Code.Probes
{
    public class AServiceProbe : IProbe
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
