namespace Example.WebApp.Services;

public class LibraryService
{
    private volatile int _totalBookCount;
        
    /// <summary>
    /// Current book count in library
    /// </summary>
    public int TotalBookCount => _totalBookCount;

    public LibraryService() => _totalBookCount = 10;

    /// <summary>
    /// Get book from library
    /// </summary>
    public void Read()
    {
        _totalBookCount -= 1;
    }

    /// <summary>
    /// Add new books to library
    /// </summary>
    /// <param name="count"></param>
    public void AddBooks(int count)
    {
        _totalBookCount += count;
    }
}