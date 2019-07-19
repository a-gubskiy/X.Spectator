using System;

namespace Example.API.Services
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
