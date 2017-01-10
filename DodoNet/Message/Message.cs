using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using DodoNet.Http;
using DodoNet.Overlay;

namespace DodoNet
{
    public abstract class Message : IDisposable
    {
        public NodeBind Source { get; set; }
        public NodeBind Destiny { get; set; }

        public int Hops { get; set; }
        public string Path { get; set; }

        public bool success = false;
        public int retryCount = 0;

        public Exception exception;

        public SerializationException errorSerialization;	// indica si hubo errores de serialización

        public IThreadLocalEvent threadLocalEvent;   // hebra que procesa el mensaje

        public volatile bool Cancelled = false;

        OverlayMessage payload;
        public OverlayMessage Payload 
        {
            get
            {
                return payload;
            }

            set
            {
                if (payload is HttpReply)
                {
                    Console.WriteLine(((HttpReply)payload).Request.Url);
                }
                payload = value;
            }
        }

        #region Contructors

        public Message() { }

        public Message(OverlayMessage payload)
        {
            this.Payload = payload;
        }

        #endregion

        #region Wire

        /// <summary>
        /// After each time DodoNet tries to send this message to another node,
        /// this callback notifies the message whether the send was successful or not.
        /// </summary>
        public virtual void SendCallback(Node localNode, Route route, NodeBind dest, bool success, Exception exception)
        {
            this.success = success;
            this.exception = exception;
            this.retryCount++;

            if (dest.NodeId != null)
            {
                if (success)
                {
                    localNode.RouteTable.LinkRoute(route, dest.NodeId);
                }
                else
                {
                    // borramos la entrada en el directorio de direcciones si 
                    // tiene identificador de nodo, se ha intentado enviar
                    // el mensaje pero no ha podido
                    if (dest.NodeId != null)
                        localNode.Directory.RemoveEntryInner(dest.NodeId);

                    localNode.RouteTable.UnlinkRoute(route, dest.NodeId);
                    localNode.Directory.MarkLikeRoutedDestiny(dest.NodeId);
                }
            }
        }

        /// <summary>
        /// cada vez que llega un mensaje
        /// </summary>
        /// <param name="localNode"></param>
        /// <param name="src"></param>
        public virtual void ReceiveCallback(Node localNode, Route route, NodeBind src)
        {
            // marcamos el local endpoint del mensaje
            src.NodeAddress = route.remoteNodeAddress;
            // vincular un identificador a una ruta
            localNode.RouteTable.LinkRoute(route, src.NodeId);
        }

        public void CancelSending()
        {
            Cancelled = true;
        }

        // Add a node to the message path
        public void AddToPath(NodeBind nextHop)
        {
            Path += nextHop + ", ";
        }

        public virtual NodeBind GetNextHop(Node localNode)
        {
            var ret = Destiny;

            if (Destiny.NodeAddress != null &&
                Destiny.NodeAddress == localNode.localBind.NodeAddress)
                ret = localNode.localBind;

            if (Destiny.NodeId != null &&
                Destiny.NodeId == localNode.localBind.NodeId)
                ret = localNode.localBind;

            return ret;
        }

        #endregion

        #region Serialization

        public virtual void Deserialization(Node localNode, Route route, IHttpMessage httpMessage) { }

        /// <summary>
        /// serialize to http message to be sent
        /// </summary>
        /// <returns></returns>
        public virtual IHttpMessage Serialize() { return null; }

        #endregion

        #region Hooks

        public virtual void MessageSourceHook(Node localNode) { }

        public virtual void MessageSourceHook(Node localNode, Exception err) { }

        public virtual void MessageArrivedHook(Node localNode) 
        {
            Payload.ArrivedAtDestination(localNode, this);
        }

        #endregion

        /// <summary>
        /// si hubo algún error en la serialización al llamar a esta funcíon
        /// lanza una exception de tipo SerializationException
        /// </summary>
        public void VerifyState()
        {
            if (errorSerialization != null)
                throw errorSerialization;
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
