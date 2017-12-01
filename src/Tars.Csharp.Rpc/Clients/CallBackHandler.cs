using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Tars.Csharp.Rpc.Clients
{
    public interface ICallBackHandler<T>
    {
        Task<object> AddCallBack(T key, int timeout);

        void SetResult(T key, object result);
    }

    public class CallBackHandler<T> : ICallBackHandler<T>
    {
        private ConcurrentDictionary<T, TaskCompletionSource<object>> results = new ConcurrentDictionary<T, TaskCompletionSource<object>>();

        public Task<object> AddCallBack(T key, int timeout)
        {
            var source = new TaskCompletionSource<object>();
            var tokenSource = new CancellationTokenSource();
            tokenSource.Token.Register(() => source.TrySetCanceled(tokenSource.Token));
            results.AddOrUpdate(key, source, (x, y) => source);
            tokenSource.CancelAfter(timeout);
            return source.Task;
        }

        public void SetResult(T key, object result)
        {
            if (results.TryRemove(key, out TaskCompletionSource<object> source))
            {
                source.SetResult(result);
            }
        }
    }
}