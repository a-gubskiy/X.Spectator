using System.Collections;

namespace X.Spectator.Base;

public class ThreadSafeList<T> : IList<T>, IReadOnlyCollection<T>
{
    private readonly List<T> _list;
    private readonly ReaderWriterLockSlim _lock;

    public ThreadSafeList()
    {
        _list = [];
        _lock = new ReaderWriterLockSlim();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public void Add(T item)
    {
        _lock.EnterWriteLock();
        _list.Add(item);
        _lock.ExitWriteLock();
    }

    public void Clear()
    {
        _lock.EnterWriteLock();
        _list.Clear();
        _lock.ExitWriteLock();
    }

    public bool Contains(T item)
    {
        _lock.EnterWriteLock();
        var contains = _list.Contains(item);
        _lock.ExitWriteLock();
        
        return contains;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _lock.EnterReadLock();
        _list.CopyTo(array, arrayIndex);
        _lock.ExitReadLock();
    }

    public bool Remove(T item)
    {
        _lock.EnterWriteLock();
        var result = _list.Remove(item);
        _lock.ExitWriteLock();

        return result;
    }

    public int Count => _list.Count;

    public bool IsReadOnly => false;

    public int IndexOf(T item)
    {
        _lock.EnterWriteLock();
        var indexOf = _list.IndexOf(item);
        _lock.ExitWriteLock();
        
        return indexOf;
    }

    public void Insert(int index, T item)
    {
        _lock.EnterWriteLock();
        _list.Insert(index, item);
        _lock.ExitWriteLock();
    }

    public void RemoveAt(int index)
    {
        _lock.EnterWriteLock();
        _list.RemoveAt(index);
        _lock.ExitWriteLock();
    }

    public T this[int index]
    {
        get
        {
            _lock.EnterReadLock();
            var value = _list[index];
            _lock.ExitReadLock();

            return value;
        }
        set
        {
            _lock.EnterWriteLock();
            _list[index] = value;
            _lock.ExitWriteLock();
        }
    }

    public int RemoveAll(Predicate<T> match)
    {
        try
        {
            _lock.EnterWriteLock();

            return _list.RemoveAll(match);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}