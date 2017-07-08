using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idleman
{
    /// <summary>
    /// A simple data structure built on top of tasks that enables asynchronous methods to be used as producers and consumer.
    /// See: https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/consuming-the-task-based-asynchronous-pattern
    /// </summary>
    /// <example>
    /// <code>
    /// private static AsyncProducerConsumerCollection&lt;int&gt; m_data = …;  
    /// …  
    /// private static async  Task ConsumerAsync()  
    /// {  
    ///     while(true)  
    ///     {  
    ///         int nextItem = await m_data.Take();  
    ///         ProcessNextItem(nextItem);  
    ///     }  
    /// }  
    /// …  
    /// private static void Produce(int data)  
    /// {  
    ///     m_data.Add(data);  
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T"></typeparam>
    public class AsyncProducerConsumerCollection<T>
    {
        private readonly Queue<T> _mCollection = new Queue<T>();
        private readonly Queue<TaskCompletionSource<T>> _mWaiting =
            new Queue<TaskCompletionSource<T>>();

        public void Add(T item)
        {
            TaskCompletionSource<T> tcs = null;
            lock (_mCollection)
            {
                if (_mWaiting.Count > 0) tcs = _mWaiting.Dequeue();
                else _mCollection.Enqueue(item);
            }
            tcs?.TrySetResult(item);
        }

        public Task<T> Take()
        {
            lock (_mCollection)
            {
                if (_mCollection.Count > 0)
                {
                    return Task.FromResult(_mCollection.Dequeue());
                }
                else
                {
                    var tcs = new TaskCompletionSource<T>();
                    _mWaiting.Enqueue(tcs);
                    return tcs.Task;
                }
            }
        }
    }
}
