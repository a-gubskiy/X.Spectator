using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.API.App_Code.Services
{
    public interface IServiceA
    {
        int Count { get; }
    }

    public class ServiceA : IServiceA
    {
        public int Count => throw new NotImplementedException();
    }
}
