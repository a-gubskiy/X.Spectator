using System;

namespace Example.API.Services
{
    public interface ILibraryService
    {
        int TotalBookCount { get; }

        bool Read();

        void AddBooks(int count);
    }

    public class LibraryService : ILibraryService
    {
        private volatile int _totalBookCount;
        
        public int TotalBookCount => _totalBookCount;
        
        public bool Read()
        {
            _totalBookCount -= 1;
            return true;
        }

        public void AddBooks(int count)
        {
            _totalBookCount += count;
        }
    }
}
