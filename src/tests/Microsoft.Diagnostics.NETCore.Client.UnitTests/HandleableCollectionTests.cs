using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Diagnostics.NETCore.Client
{
    public class HandleableCollectionTests
    {
        // Generous timeout to allow APIs to respond on slower or more constrained machines
        private static readonly TimeSpan DefaultPositiveVerificationTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan DefaultNegativeVerificationTimeout = TimeSpan.FromSeconds(2);

        private readonly ITestOutputHelper _outputHelper;

        public HandleableCollectionTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task HandleableCollectionThrowsWhenDisposedTest()
        {
            var collection = new HandleableCollection<int>();

            AddRangeAndVerifyItems(collection, endInclusive: 9);

            HandleableCollection<int>.Handler handler = (int item, out bool removeItem) =>
            {
                removeItem = false;
                return 20 == item;
            };

            using var cancellation = new CancellationTokenSource(DefaultPositiveVerificationTimeout);

            Task handleTask = Task.Run(() => collection.Handle(handler, DefaultPositiveVerificationTimeout));
            Task handleAsyncTask = collection.HandleAsync(handler, cancellation.Token);

            // Task.Delay intentionally shorter than default timeout to check that Handle*
            // calls did not complete quickly.
            Task delayTask = Task.Delay(TimeSpan.FromSeconds(1));
            Task completedTask = await Task.WhenAny(delayTask, handleTask, handleAsyncTask);

            // Check that the handle tasks didn't complete
            Assert.Equal(delayTask, completedTask);

            collection.Dispose();

            // Incomplete calls from prior to disposal should throw ObjectDisposedException
            await Assert.ThrowsAsync<ObjectDisposedException>(() => handleTask);
            await Assert.ThrowsAsync<ObjectDisposedException>(() => handleAsyncTask);

            // New calls should throw ObjectDisposedException
            Assert.Throws<ObjectDisposedException>(
                () => collection.Add(10));

            Assert.Throws<ObjectDisposedException>(
                () => collection.ClearItems());

            Assert.Throws<ObjectDisposedException>(
                () => collection.Handle(DefaultPositiveVerificationTimeout));

            Assert.Throws<ObjectDisposedException>(
                () => collection.Handle(handler, DefaultPositiveVerificationTimeout));

            await Assert.ThrowsAsync<ObjectDisposedException>(
                () => collection.HandleAsync(cancellation.Token));

            await Assert.ThrowsAsync<ObjectDisposedException>(
                () => collection.HandleAsync(handler, cancellation.Token));

            Assert.Throws<ObjectDisposedException>(
                () => ((IEnumerable)collection).GetEnumerator());

            Assert.Throws<ObjectDisposedException>(
                () => ((IEnumerable<int>)collection).GetEnumerator());
        }

        [Fact]
        public async Task HandleableCollectionDefaultHandlerTest()
        {
            await HandleableCollectionDefaultHandlerTestCore(useAsync: false);
        }

        [Fact]
        public async Task HandleableCollectionDefaultHandlerTestAsync()
        {
            await HandleableCollectionDefaultHandlerTestCore(useAsync: true);
        }

        /// <summary>
        /// Tests that the default handler handles one item at a time and
        /// removes each item after each successful handling.
        /// </summary>
        private async Task HandleableCollectionDefaultHandlerTestCore(bool useAsync)
        {
            using var collection = new HandleableCollection<int>();
            Assert.Empty(collection);

            var shim = new HandleableCollectionApiShim<int>(collection, useAsync);

            AddRangeAndVerifyItems(collection, endInclusive: 14);

            int expectedCollectionCount = collection.Count();
            for (int item = 0; item < 15; item++)
            {
                int handledItem = await shim.Handle(DefaultPositiveVerificationTimeout);
                expectedCollectionCount--;

                Assert.Equal(item, handledItem);
                Assert.Equal(expectedCollectionCount, collection.Count());
            }

            Assert.Empty(collection);

            await shim.Handle(DefaultNegativeVerificationTimeout, expectTimeout: true);
        }

        private static void AddAndVerifyItems<T>(HandleableCollection<T> collection, IEnumerable<(T, int)> itemsAndCounts)
        {
            // Pairs of (item to be added, expected collection count after adding item)
            foreach ((T item, int count) in itemsAndCounts)
            {
                collection.Add(item);
                Assert.Equal(count, collection.Count());
            }
        }

        private static void AddRangeAndVerifyItems(HandleableCollection<int> collection, int endInclusive)
        {
            if (endInclusive < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(endInclusive));
            }

            IList<(int, int)> itemsAndCounts = new List<(int, int)>();
            for (int item = 0; item <= endInclusive; item++)
            {
                itemsAndCounts.Add((item, item + 1));
            }
            AddAndVerifyItems(collection, itemsAndCounts);
        }

        private class HandleableCollectionApiShim<T>
        {
            private HandleableCollection<T> _collection;
            private readonly bool _useAsync;

            public HandleableCollectionApiShim(HandleableCollection<T> collection, bool useAsync)
            {
                _collection = collection;
                _useAsync = useAsync;
            }

            public async Task<T> Handle(TimeSpan timeout, bool expectTimeout = false)
            {
                if (_useAsync)
                {
                    using var cancellation = new CancellationTokenSource(timeout);
                    if (expectTimeout)
                    {
                        await Assert.ThrowsAsync<TaskCanceledException>(() => _collection.HandleAsync(cancellation.Token));
                        return default;
                    }
                    else
                    {
                        return await _collection.HandleAsync(cancellation.Token);
                    }
                }
                else
                {
                    if (expectTimeout)
                    {
                        Assert.Throws<TimeoutException>(() => _collection.Handle(timeout));
                        return default;
                    }
                    else
                    {
                        return _collection.Handle(timeout);
                    }
                }
            }

            public async Task<T> Handle(HandleableCollection<T>.Handler handler, TimeSpan timeout)
            {
                if (_useAsync)
                {
                    using var cancellation = new CancellationTokenSource(timeout);
                    return await _collection.HandleAsync(handler, cancellation.Token);
                }
                else
                {
                    return _collection.Handle(handler, timeout);
                }
            }
        }
    }
}
