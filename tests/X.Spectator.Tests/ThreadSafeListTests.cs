using System;
using Xunit;
using X.Spectator.Base;

namespace X.Spectator.Tests
{
    public class ThreadSafeListTests
    {
        [Fact]
        public void BasicOperations()
        {
            var list = new ThreadSafeList<int>();
            Assert.Empty(list);

            list.Add(1);
            list.Add(2);
            list.Add(3);
            Assert.Equal(3, list.Count);

            Assert.Contains(2, list);
            Assert.Equal(1, list.IndexOf(2));
            Assert.Equal(2, list[1]);

            list.Insert(1, 42);
            Assert.Equal(42, list[1]);

            list.RemoveAt(1);
            Assert.Equal(2, list[1]);

            var arr = new int[list.Count];
            list.CopyTo(arr, 0);
            Assert.Equal(new[] { 1, 2, 3 }, arr);

            Assert.False(list.IsReadOnly);

            var removed = list.Remove(2);
            Assert.True(removed);
            Assert.Equal(2, list.Count);

            var removedCount = list.RemoveAll(i => i > 1);
            Assert.Equal(1, removedCount);
            Assert.Single(list);

            list.Clear();
            Assert.Empty(list);
        }
    }
}

