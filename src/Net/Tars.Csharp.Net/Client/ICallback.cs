using System;

namespace Tars.Csharp.Net.Client
{
    public interface ICallback<T>
    {
        void OnCompleted(T result);

        void OnException(Exception ex);

        void OnExpired();
    }
}