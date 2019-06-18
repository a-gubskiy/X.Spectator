using System;
using X.Monitor.Base;

namespace X.Monitor
{
    public class Probe : IProbe
    {
        private readonly Func<bool> _func;
        
        public string Name { get; }

        public Probe(string name, Func<bool> func)
        {
            Name = name;
            _func = func;
        }

        public bool GetState()
        {
            try
            {
                return _func();
            }
            catch
            {
                return false;
            }
        } 
            
    }
}