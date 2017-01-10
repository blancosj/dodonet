using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

using DodoNet;
using DodoNet.Http;
using DodoNet.Overlay;

namespace DodoNet.Http.Serialization
{
    public class HttpSerializer
    {
        public Node LocalNode { get; set; }
        public Route Route { get; set; }
        public IHttpMessage HttpMessage { get; set; }

        public HttpSerializer(Node localNode, Route route, IHttpMessage HttpMessage)
        {
            this.LocalNode = localNode;
            this.Route = route;
            this.HttpMessage = HttpMessage;
        }

        public IHttpMessage Serialize(Message message)
        {
            IHttpMessage ret = null;

            if (message.Payload is OverlayRequest)
            {
                var tmp = new HttpRequest()
                {
                    Host = message.Destiny.ToString()
                };

                ret = (HttpRequest)tmp;
            }
            else if (message.Payload is OverlayReply)
            {
                var tmp = new HttpReply();

                ret = (HttpReply)tmp;
            }

            // head
            ret.Url = message.Payload.GetType().FullName;

            ret.AddField(new HttpField(HttpConst.MessageSource, message.Source));
            ret.AddField(new HttpField(HttpConst.MessageDestiny, message.Destiny));
            ret.AddField(new HttpField(HttpConst.MessagePath, message.Path));
            ret.AddField(new HttpField(HttpConst.MessageHops, message.Hops));

            // body
            ret.CreateBody();
            ret.Body.CreateStream();

            if (message.Payload != null)
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ret.Body.ContentStream, message.Payload);
                ret.Body.ContentType = HttpConst.ContentTypeDodoNetObject;
            }
            else
                ret.Body.ContentType = "";

            return ret;
        }
    }
}
