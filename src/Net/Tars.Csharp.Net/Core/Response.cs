using System;
using System.Runtime.CompilerServices;

namespace Tars.Csharp.Net.Core
{
    public abstract class Response
    {
        public int TicketNumber { get; set; } = Request.DefaultTicktNumber;

        //transient
        public Session Session { get; protected set; }

        //transient
        public bool IsAsyncMode { get; protected set; }

        private volatile bool commited = false;

        public Response(Session session)
        {
            Session = session;
        }

        public void AsyncCallStart()
        {
            IsAsyncMode = true;
        }

        public void AsyncCallEnd()
        {
            if (!IsAsyncMode) throw new Exception("The response is not async mode.");
            EnsureNotCommitted();
            Session.Write(this);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void EnsureNotCommitted()
        {
            if (commited) throw new Exception("Not allowed after response has committed.");
            commited = true;
        }
    }
}