using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Overlay
{
    public class RouterCont : Continuation
    {
        const int MAXRETRIES = 3;
        int retryCount;

        Message msg;
        NodeBind nextHop;

        public RouterCont(Message msg)
        {
            this.msg = msg;

            retryCount = 0;
        }

        public override void Execute(Node localNode)
        {
            try
            {
                // Failed to send the message. Try routing again.
                retryCount++;

                if (!msg.Cancelled)
                {
                    // Find the next hop for this message
                    nextHop = msg.GetNextHop(localNode);

                    // el numero de saltos está limitado
                    if (nextHop == localNode.localBind)
                    {
                        // Message has arrived at its destination
                        msg.MessageArrivedHook(localNode);
                    }
                    else if (nextHop != null)
                    {
                        // Send on to next hop
                        var cb = new RouteContCallback(Callback);
                        localNode.SendMessageAsync(nextHop, msg, cb);
                    }
                    else
                    {
                        msg.Dispose();
                    }
                }
            }
            catch (Exception err)
            {
                msg.MessageSourceHook(localNode, err);
            }
        }

        /// <summary>
        /// This is called from the event loop when the asynchronous send completes.
        /// </summary>
        private void Callback(Node localNode, Message msg, Route route, bool success, Exception exception)
        {
            if (success || retryCount > MAXRETRIES)
            {
                msg.SendCallback(localNode, route, nextHop, success, exception);
                msg.Dispose();
            }
            else
            {
                // comprobamos el numero de veces que se ha enviado el paquete
                Execute(localNode);
            }
        }
    }
}
