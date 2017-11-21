using System;
using System.Collections.Generic;
using System.Threading;

namespace Tars.Csharp.Net.Core
{
    public abstract class Request
    {
        public const int DefaultTicktNumber = -1;

        private static int ticketGenerator = 0;
        [NonSerialized]
        private Dictionary<string, string> distributedContext = new Dictionary<string, string>(8);

        public int TicketNumber { get; set; } = DefaultTicktNumber;

        //transient
        public Session Session { get; protected set; }

        //transient
        public InvokeStatus InvokeStatus { get; set; }

        //transient
        public long ProcessTime { get; set; }

        //transient
        public long BornTime { get; protected set; }

        public Dictionary<string, string> DistributedContext
        {
            get => distributedContext;
            set
            {
                if (value == null) distributedContext.Clear();
                else distributedContext = value;
            }
        }

        public Request(Session session)
        {
            ResetBornTime();
            Session = session;
            TicketNumber = Interlocked.Increment(ref ticketGenerator);
            ExportDistributedContext(distributedContext);
        }

        public void ResetBornTime()
        {
            BornTime = DateTime.Now.Ticks;
        }

        public void Init()
        {
        }

        private void ExportDistributedContext(Dictionary<String, String> map)
        {
        }
    }
}