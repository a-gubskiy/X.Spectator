using Example.API.App_Code.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.Spectator.Base;

namespace Example.API.App_Code.Probes
{
    public class BServiceProbe : IProbe
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
