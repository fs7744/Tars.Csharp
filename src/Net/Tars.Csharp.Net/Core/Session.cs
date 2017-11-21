using System;
using System.Threading;
using Tars.Csharp.Net.Protocol;

namespace Tars.Csharp.Net.Core
{
    public abstract class Session
    {
        public SessionStatus Status { get; set; } = SessionStatus.NotConnected;

        public long LastOperationTime { get; protected set; } = DateTime.Now.Ticks;

        public bool IsKeepAlive { get; set; } = true;

        protected CountdownEvent connectLatch = new CountdownEvent(1);

        public void UpdateLastOperationTime()
        {
            LastOperationTime = DateTime.Now.Ticks;
        }

        public void FinishConnect()
        {
            connectLatch.Signal();
        }

        public bool WaitToConnect(int connectTimeout)
        {
            try
            {
                return connectLatch.Wait(connectTimeout);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public abstract void Close();

        public abstract void AsyncClose();

        protected abstract void Read();

        protected abstract void Accept();

        public abstract void Write(Request request);

        public abstract void Write(Response response);

        //public abstract void SetChannel(SelectableChannel channel);

        public abstract String GetRemoteIp();

        public abstract int GetRemotePort();

        public abstract IProtocolFactory GetProtocolFactory();
    }
}