using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Overlay
{
    public class RequestReplyWebTable
    {
        public Dictionary<Route, Queue<WebMessage>> Table { get; private set; }
        public Node LocalNode { get; private set; }

        object sync = new object();

        public RequestReplyWebTable(Node localNode)
        {
            this.LocalNode = localNode;

            Table = new Dictionary<Route, Queue<WebMessage>>();
        }

        public void Enqueue(Route route, WebMessage message)
        {
            if (!Table.ContainsKey(route))
            {
                lock (sync)
                {
                    if (!Table.ContainsKey(route))
                    {
                        Table.Add(route, new Queue<WebMessage>());
                    }
                }
            }

            Table[route].Enqueue(message);
        }

        public WebMessage Dequeue(Route route)
        {
            WebMessage ret = null;

            if (Table[route] != null)
            {
                ret = Table[route].Dequeue();
            }

            return ret;
        }
    }
}
