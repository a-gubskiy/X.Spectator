using System;
using System.Threading.Tasks;
using Example.API.Services;
using X.Spectator.Base;

namespace Example.API.Probes
{
    public interface ILibraryServiceProbe : IProbe
    {
    }

    public class LibraryServiceProbe : ILibraryServiceProbe
    {
        private readonly ILibraryService _service;

        public string Name => "Library Service Probe";

        public LibraryServiceProbe(LibraryService service)
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
                if (_service.TotalBookCount > 20)
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
