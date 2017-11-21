using System;
using System.Threading;
using Tars.Csharp.Net.Core;

namespace Tars.Csharp.Net.Client.Tickets
{
    public class Ticket<T>
    {
        private CountdownEvent latch = new CountdownEvent(1);
        private volatile bool expired = false;
        protected long timeout = 1000;

        public long StartTime { get; } = DateTime.Now.Ticks;
        public Request Request { get; protected set; } = null;
        public T Rresponse { get; protected set; } = default(T);
        public ICallback<T> Callback { get; set; }
        public int TicketNumber { get; protected set; } = -1;
        public static ITicketListener TicketListener { protected get; set; }

        public Ticket(Request request, long timeout)
        {
            Request = request;
            TicketNumber = request.TicketNumber;
            this.timeout = timeout;
        }

        public bool Await(TimeSpan timeout)
        {
            bool status = latch.Wait(timeout);
            CheckExpired();
            return status;
        }

        public void Await()
        {
            latch.Wait();
            CheckExpired();
        }

        public void Expired()
        {
            expired = true;
            if (Callback != null) Callback.OnExpired();
            CountDown();
            if (TicketListener != null) TicketListener.OnResponseExpired(this);
        }

        public void CountDown()
        {
            latch.Signal();
        }

        public bool IsDone()
        {
            return latch.IsSet;
        }

        public void NotifyResponse(T response)
        {
            Rresponse = response;
            if (Callback != null) this.Callback.OnCompleted(response);
            if (TicketListener != null) TicketListener.OnResponseReceived(this);
        }

        protected void CheckExpired()
        {
            if (expired) throw new Exception("", new System.IO.IOException("The operation has timed out."));
        }
    }
}