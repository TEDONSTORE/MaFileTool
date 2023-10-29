using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace maFileTool.Services
{
    public static class AsyncHelpers
    {
        /// <summary>
        /// Synchronously execute's an async Task method which has a void return value.
        /// </summary>
        /// <param name="task">The Task method to execute.</param>
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var syncContext = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncContext);

            syncContext.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    syncContext.InnerException = e;
                    throw;
                }
                finally
                {
                    syncContext.EndMessageLoop();
                }
            }, null);

            syncContext.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        /// Synchronously execute's an async Task<T> method which has a T return type.
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">The Task<T> method to execute.</param>
        /// <returns>The result of awaiting the given Task<T>.</returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var syncContext = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncContext);
            T result = default(T);

            syncContext.Post(async _ =>
            {
                try
                {
                    result = await task();
                }
                catch (Exception e)
                {
                    syncContext.InnerException = e;
                    throw;
                }
                finally
                {
                    syncContext.EndMessageLoop();
                }
            }, null);

            syncContext.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);

            return result;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
            private readonly Queue<Tuple<SendOrPostCallback, object>> items =
                new Queue<Tuple<SendOrPostCallback, object>>();
            private bool done;

            public Exception InnerException { get; set; }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }

                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }

                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}