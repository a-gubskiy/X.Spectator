using System;

namespace Example.API.Services
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
