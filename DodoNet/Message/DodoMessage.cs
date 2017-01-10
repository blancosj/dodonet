using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using DodoNet.Http;
using DodoNet.Overlay;

namespace DodoNet
{
    public class DodoMessage : Message
    {
        public DodoMessage() { }

        public DodoMessage(OverlayMessage msg)
            : base(msg) { }

        #region Serialization

        public override void Deserialization(Node localNode, Route route, IHttpMessage httpMessage)
        {
            Source = httpMessage.Head.GetField(HttpConst.MessageSource).Value;
            Destiny = httpMessage.Head.GetField(HttpConst.MessageDestiny).Value;
            Path = httpMessage.Head.GetField(HttpConst.MessagePath).Value;

            Hops = int.Parse(httpMessage.Head.GetField(HttpConst.MessageHops).Value);

            var formatter = new BinaryFormatter();
            httpMessage.Body.ContentStream.Position = 0;
            Payload = formatter.Deserialize(httpMessage.Body.ContentStream) as OverlayMessage;
        }

        public override IHttpMessage Serialize()
        {
            IHttpMessage ret = null;

            if (Payload is OverlayRequest)
            {
                var tmp = new HttpRequest()
                {
                    Host = Destiny.ToString()
                };

                ret = (HttpRequest)tmp;
            }
            else if (Payload is OverlayReply)
            {
                var tmp = new HttpReply();

                ret = (HttpReply)tmp;
            }

            // head
            ret.Url = Payload.GetType().FullName;

            ret.AddField(new HttpField(HttpConst.MessageSource, Source));
            ret.AddField(new HttpField(HttpConst.MessageDestiny, Destiny));
            ret.AddField(new HttpField(HttpConst.MessagePath, Path));
            ret.AddField(new HttpField(HttpConst.MessageHops, Hops));

            // body
            ret.CreateBody();
            ret.Body.CreateStream();

            if (Payload != null)
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ret.Body.ContentStream, Payload);
                ret.Body.ContentType = HttpConst.ContentTypeDodoNetObject;
            }
            else
                ret.Body.ContentType = "";

            return ret;
        }

        #endregion

        #region Hooks

        public override void MessageArrivedHook(Node localNode)
        {
            // Check if the arriving message is a request message
            if (Payload is OverlayRequest)
            {
                localNode.RequestReplyTable.HandleArrivedAtDestination(this);
            }
            else if (Payload is OverlayReply)
            {
                localNode.RequestReplyTable.ReceivedReply((OverlayReply)Payload);
            }
            else
            {
                Payload.ArrivedAtDestination(localNode, this);
            }
        }

        public override void MessageSourceHook(Node localNode)
        {
            // If the message is a request-reply message, let the table handle it
            // Debug.Assert(overlayMsg != null);
            if (Payload is OverlayRequest)
            {
                localNode.RequestReplyTable.HandleRequestAtSource(this);
            }
        }

        public override void MessageSourceHook(Node localNode, Exception err)
        {
            // If the message is a request-reply message, let the table handle it
            // Debug.Assert(overlayMsg != null, errAtSource.Message, errAtSource.StackTrace);

            if (Payload is OverlayRequest)
            {
                localNode.RequestReplyTable.HandleExceptionAtSource((OverlayRequest)Payload, err);
            }
        }

        #endregion
    }
}
