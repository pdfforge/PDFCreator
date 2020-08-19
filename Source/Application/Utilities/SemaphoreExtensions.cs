using System;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Utilities
{
    public static class SemaphoreExtensions
    {
        public static async Task RunSynchronized(this SemaphoreSlim semaphore, Func<Task> func)
        {
            await semaphore.WaitAsync();

            try
            {
                await func();
            }
            finally
            {
                semaphore.Release(1);
            }
        }

        public static async Task<T> RunSynchronized<T>(this SemaphoreSlim semaphore, Func<Task<T>> func)
        {
            await semaphore.WaitAsync();

            try
            {
                return await func();
            }
            finally
            {
                semaphore.Release(1);
            }
        }

        public static async Task RunSynchronized(this SemaphoreSlim semaphore, Action action)
        {
            await semaphore.WaitAsync();

            try
            {
                action();
            }
            finally
            {
                semaphore.Release(1);
            }
        }
    }
}
