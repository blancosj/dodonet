using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

using DodoNet.Http;
using DodoNet.Overlay;
using DodoNet.Tcp;

namespace DodoNet
{
    /// <summary>
    /// ruta con un origen
    /// </summary>
    public class Route : IDisposable
    {
        #region Header

        public HttpReceiverStream BufferStream { get; private set; }

        public Socket socket;

        public readonly object Sync = new object();

        public IPEndPoint LocalEndPoint
        { 
            get 
            {
                IPEndPoint ret = null;
                try
                {
                    ret = (IPEndPoint)socket.LocalEndPoint;
                }
                catch { }

                return ret;
            } 
        }
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                IPEndPoint ret = null;
                try
                {
                    ret = (IPEndPoint)socket.RemoteEndPoint;
                }
                catch { }

                return ret;
            }
        }
        public int IdUnique { get { return socket.RemoteEndPoint.GetHashCode(); } }

        public NodeAddress localNodeAddress;
        public NodeAddress remoteNodeAddress;
        private DateTime dtLastActivity;

        public DateTime LastActivity { get { return dtLastActivity; } }
        public int Handle { get { return (int)socket.Handle; } }

        // variables para ctrl de recepcion de bytes
        public byte[] bBufferIn;
        public int HeaderSizePending;
        public byte[] bHeaderPending;
        public Packet PacketPending;
        public NodeBind nodeBind;

        // propiedades de control
        bool inUse = false;
        public bool InUse { get { return inUse; } set { inUse = value; } }

        bool failed = false;
        public bool Failed { get { return failed; } set { failed = value; } }

        ItemOutQueue currentItem;
        public ItemOutQueue CurrentItem { get { return currentItem; } set { currentItem = value; } }

        ManualResetEvent connectDone = new ManualResetEvent(false);
        public ManualResetEvent ConnectDone { get { return connectDone; } set { connectDone = value; } }

        private int protocol = Protocols.Prot_Undefined;
        public int Protocol { get { return protocol; } set { protocol = value; } }

        Stream st;
        SslStream ssls;

        public X509Certificate2 Certificate { get { return localNodeAddress.Certificate; } }

        Node localNode;
        
        public bool Connected
        {
            get
            {
                if (socket != null)
                    return socket.Connected;
                else
                    return false;
            }
        }

        #endregion

        #region Constructor

        public Route(Node localNode)
        {            
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.localNode = localNode;

            BufferStream = new HttpReceiverStream(localNode, this);

            InitCommon();
            UpdateActivity();
        }

        public Route(Node localNode, Socket socket)
        {            
            this.socket = socket;
            this.localNode = localNode;

            st = new NetworkStream(socket) as Stream;

            localNodeAddress = new NodeAddress((IPEndPoint)socket.LocalEndPoint);
            remoteNodeAddress = new NodeAddress((IPEndPoint)socket.RemoteEndPoint);

            BufferStream = new HttpReceiverStream(localNode, this);

            InitCommon();
            UpdateActivity();
        }

        #endregion

        private void InitCommon()
        {
            bBufferIn = new byte[TcpConfig.SizeReceivedBuffer];
            bHeaderPending = new byte[DodoConfig.SizeHeader];

            TcpConfig.ConfigSocket(socket);
        }

        // actualizamos la variable que mantiene informado de la hora/dia
        // de la ultima actividad conocida
        public void UpdateActivity()
        {
            dtLastActivity = DateTime.Now;
        }

        /// <summary>
        /// contruimos entrada de la ruta
        /// </summary>
        public void Build(NodeAddress nodeAddress)
        {
            try
            {
                socket.Bind((EndPoint)nodeAddress.IPEndPoint);
                socket.Listen(TcpConfig.Backlog);               

                localNodeAddress = nodeAddress;
                localNodeAddress.IPEndPoint = (IPEndPoint)socket.LocalEndPoint;
            }
            catch (NullReferenceException err)
            {
                Node.LogAppendLine(err);
                throw err;
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
                throw err;
            }
        }

        #region Connect

        public IAsyncResult BeginConnect(NodeAddress remoteNodeAddress, AsyncCallback callback, object state)
        {
            this.remoteNodeAddress = remoteNodeAddress;

            return socket.BeginConnect((EndPoint)remoteNodeAddress.IPEndPoint, callback, state);
        }

        public void EndConnect(IAsyncResult asyncResult)
        {
            socket.EndConnect(asyncResult);

            if (socket.Connected)
            {
                st = new NetworkStream(socket) as Stream;

                localNodeAddress = new NodeAddress((IPEndPoint)socket.LocalEndPoint);
                // remoteNodeAddress = new NodeAddress((IPEndPoint)socket.RemoteEndPoint);

                if (remoteNodeAddress.IsSecureLayer) 
                    EstablishSSLAsClient();
            }
        }

        #endregion

        #region Accept

        public Socket Accept()
        {
            return socket.Accept();
        }

        /// <summary>
        /// establecer conexion SSL como servidor
        /// </summary>
        public void EstablishSSLAsServer(Route parent)
        {
            ssls = new SslStream(st);
            ssls.AuthenticateAsServer(parent.Certificate, false, SslProtocols.Default, false);
            st = ssls as Stream;
        }

        /// <summary>
        /// establecer conexion SSL como cliente
        /// </summary>
        public void EstablishSSLAsClient()
        {
            ssls = new SslStream(st, false, new RemoteCertificateValidationCallback(CertificateValidationCallback));
            ssls.AuthenticateAsClient("");
            st = ssls as Stream;
        }

        bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors != SslPolicyErrors.None;
        }

        public IAsyncResult BeginAccept(AsyncCallback cb)
        {
            IAsyncResult ret = null;
            if (socket != null) ret = socket.BeginAccept(cb, this);
            return ret;
        }

        public Route EndAccept(Node localNode, IAsyncResult ar)
        {
            Route ret = null;
            try
            {
                if (socket != null)
                {
                    ret = new Route(localNode, socket.EndAccept(ar));
                    
                    ret.BufferStream.HandledMessage +=
                        new HttpMessageHandler(localNode.HandledReceiveMessageFromRoute);

                    if (localNodeAddress.IsSecureLayer) ret.EstablishSSLAsServer(this);
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
            return ret;
        }

        #endregion

        #region Receive

        public int Receive(byte[] buffer, int offset, int count)
        {
            return st.Read(buffer, offset, count);
        }

        public IAsyncResult BeginReceive(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return st.BeginRead(buffer, offset, count, callback, state);
        }

        public int EndReceive(IAsyncResult asyncResult)
        {
            return st.EndRead(asyncResult);
        }

        #endregion

        #region Send

        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            return st.BeginWrite(buffer, offset, size, callback, state);
        }

        public void EndSend(IAsyncResult asyncResult)
        {
            st.EndWrite(asyncResult);
        }

        public void Send(byte[] buffer, int offset, int count)
        {
            st.Write(buffer, offset, count);
        }

        #endregion

        #region Miembros de IDisposable
                
        public void Dispose()
        {
            try
            {
                InUse = false;
                if (socket != null)
                {
                    if (socket.Connected) socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }

                if (st != null)
                {
                    st.Close();
                    st.Dispose();
                }

                if (PacketPending != null) PacketPending.Dispose();

                BufferStream.Dispose();
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
        }

        #endregion
    }
}


