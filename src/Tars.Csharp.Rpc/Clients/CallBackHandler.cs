using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tars.Csharp.Codecs.Tup;

namespace Tars.Csharp.Rpc.Clients
{
    public interface ICallBackHandler<T>
    {
        Task<RequestPacket> AddCallBack(T key, int timeout);

        void SetResult(T key, RequestPacket result);
    }

    public class CallBackHandler<T> : ICallBackHandler<T>
    {
        private ConcurrentDictionary<T, TaskCompletionSource<RequestPacket>> results = new ConcurrentDictionary<T, TaskCompletionSource<RequestPacket>>();

        public Task<RequestPacket> AddCallBack(T key, int timeout)
        {
            var source = new TaskCompletionSource<RequestPacket>();
            var tokenSource = new CancellationTokenSource();
            tokenSource.Token.Register(() => source.TrySetCanceled(tokenSource.Token));
            results.AddOrUpdate(key, source, (x, y) => source);
            tokenSource.CancelAfter(timeout);
            return source.Task;
        }

        public void SetResult(T key, RequestPacket result)
        {
            if (results.TryRemove(key, out TaskCompletionSource<RequestPacket> source))
            {
                source.SetResult(result);
            }
        }
    }
}