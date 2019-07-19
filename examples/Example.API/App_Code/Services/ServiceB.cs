using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.API.App_Code.Services
{
    public interface IServiceB
    {
        bool ConnectedToServer { get; }
    }

    public class ServiceB : IServiceB
    {
        public bool ConnectedToServer => throw new NotImplementedException();
    }
}
