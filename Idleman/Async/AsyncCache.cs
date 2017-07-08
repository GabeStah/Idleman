using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idleman.Async
{
    /// <summary>
    /// One important aspect of a task is that it may be handed out to multiple consumers, all of whom may await it, 
    /// register continuations with it, get its result or exceptions (in the case of Task<TResult>), and so on. 
    /// This makes Task and Task<TResult> perfectly suited to be used in an asynchronous caching infrastructure. 
    /// Here’s an example of a small but powerful asynchronous cache built on top of Task<TResult>.
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/consuming-the-task-based-asynchronous-pattern"/>
    /// <example>
    /// <code>
    /// private AsyncCache<string,string> m_webPages = new AsyncCache<string,string>(DownloadStringAsync); 
    /// 
    /// private async  void btnDownload_Click(object sender, RoutedEventArgs e)   
    /// {  
    ///     btnDownload.IsEnabled = false;  
    ///     try  
    ///     {  
    ///         txtContents.Text = await m_webPages["http://www.microsoft.com"];  
    ///     }  
    ///     finally { btnDownload.IsEnabled = true; }  
    /// }   
    /// </code>
    /// </example>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class AsyncCache<TKey, TValue>
    {
        private readonly Func<TKey, Task<TValue>> _valueFactory;
        private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _map;

        public AsyncCache(Func<TKey, Task<TValue>> valueFactory)
        {
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            _map = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>();
        }

        public Task<TValue> this[TKey key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                return _map.GetOrAdd(key, toAdd =>
                    new Lazy<Task<TValue>>(() => _valueFactory(toAdd))).Value;
            }
        }
    }
}
