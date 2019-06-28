using System;
using System.Threading.Tasks;
using X.Spectator.Base;

namespace X.Spectator
{
    public class Probe : IProbe
    {
        private readonly Func<Task<bool>> _func;
        
        public string Name { get; }

        public Probe(string name, Func<Task<bool>> func)
        {
            Name = name;
            _func = func;
        }

        public async Task<bool> Ready()
        {
            try
            {
                return await _func();
            }
            catch
            {
                return false;
            }
        } 
            
    }
}