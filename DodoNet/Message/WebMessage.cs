using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using DodoNet.Http;
using DodoNet.Overlay;

namespace DodoNet
{
    public class WebMessage : Message
    {
        bool disposed = false;

        public bool CloseConnection { get; set; }

        #region Constructor

        public WebMessage() { }

        #endregion

        #region Serialization

        public override void Deserialization(Node localNode, Route route, IHttpMessage httpMessage)
        {
            Source = new NodeBind(null, route.remoteNodeAddress);
            Destiny = localNode.localBind;
            Payload = (OverlayMessage)httpMessage;
        }

        public override IHttpMessage Serialize()
        {
            return (IHttpMessage)Payload;
        }

        #endregion

        #region Wire

        public override NodeBind GetNextHop(Node localNode)
        {
            return Destiny;
        }

        public override void ReceiveCallback(Node localNode, Route route, NodeBind src)
        {
            if (Payload is HttpReply)
            {
                var reply = (HttpReply)Payload;
                var msg = localNode.RequestReplyWebTable.Dequeue(route);
                var request = (HttpRequest)msg.Payload;
                reply.Request = request;

                switch (reply.Code)
                {
                    case Codes.TEMPORARY_REDIRECT:
                        request.Url = reply.Location;
                        localNode.ResumeRouting(msg);
                        // desechamos este mensaje directamente
                        Dispose();
                        break;
                }
            }
        }

        public override void SendCallback(Node localNode, Route route, NodeBind dest, bool success, Exception exception)
        {
            if (Payload is HttpRequest)
            {
                localNode.RequestReplyWebTable.Enqueue(route, this);
            }

            if (CloseConnection)
                localNode.RouteTable.RemoveRoute(route);
        }

        #endregion

        #region Hooks

        public override void MessageArrivedHook(Node localNode)
        {
            if (Payload is HttpReply)
            {
                var reply = (HttpReply)Payload;
                var request = reply.Request;

                switch (reply.Code)
                {
                    case Codes.NOT_FOUND:
                        request.Async.success = false;
                        request.Async.exception = new HttpException(reply, request.Url);
                        break;
                    case Codes.BAD:
                        request.Async.success = false;
                        request.Async.exception = new HttpException(reply);
                        break;
                    default:
                        request.Async.success = true;
                        break;
                }

                request.Async.Reply = reply;

                // liberamos el ManualResetEvent
                ((ManualResetEvent)request.Async.AsyncWaitHandle).Set();

                // llamamos al callback de SendRequest si existe
                if (request.Async.Callback != null)
                {
                    request.Async.Callback(request.Async);
                }
            }
            else if (Payload is HttpRequest)
            {
                try
                {
                    Payload.ArrivedAtDestination(localNode, this);
                }
                catch (EndResponseException) { }
            }
        }

        #endregion

        public override void Dispose()
        {
            if (!disposed)
            {
                ((IDisposable)Payload).Dispose();
                base.Dispose();

                disposed = true;
            }
        }
    }
}
