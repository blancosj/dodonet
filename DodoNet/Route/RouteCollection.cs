using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using DodoNet.Tcp;

namespace DodoNet
{
    public class RouteCollection : IDisposable
    {
        #region Head

        Dictionary<KeyRoutePair, Route> table = new Dictionary<KeyRoutePair, Route>();
        Dictionary<NodeAddress, Route> tableRemote = new Dictionary<NodeAddress, Route>();
        Dictionary<NodeId, KeyRoutePair> tableNodeId = new Dictionary<NodeId, KeyRoutePair>();

        Dictionary<NodeAddress, object> pendingConnections = new Dictionary<NodeAddress, object>();

        object sync = new object();
        public object Sync { get { return sync; } }

        Node localNode;
        NodeAddress localNodeAddress;

        bool disposed = false;

        #endregion

        public RouteCollection(Node localNode)
        {
            this.localNode = localNode;
            this.localNodeAddress = localNode.localBind.NodeAddress;
        }

        public Route this[KeyRoutePair key]
        {
            get
            {
                return table[key];
            }

            set
            {
                lock (sync)
                {
                    table[key] = value;
                }
            }
        }

        public int Count
        {
            get
            {
                int ret = 0;
                try
                {
                    ret = table.Count;
                }
                catch (Exception err)
                {
                    Node.LogAppendLine("Count: " + err.Message);
                }
                return ret;
            }
        }

        public Route[] GetArray()
        {
            lock (sync)
            {
                Route[] ret = new Route[table.Count];
                table.Values.CopyTo(ret, 0);
                return ret;
            }
        }

        /// <summary>
        /// comprueba cual ruta podemos utilizar para enviar el item
        /// si la ruta estuviera mal o no existiera crea una ruta nueva 
        /// </summary>
        /// <param name="item"></param>
        public Route GetRoute(NodeAddress remoteNodeAddress)
        {
            Route route = null;
            object syncLocal = null;
            try
            {
                bool toconnected = false;

                lock (sync)
                {
                    if (pendingConnections.ContainsKey(remoteNodeAddress))
                    {
                        syncLocal = pendingConnections[remoteNodeAddress];
                    }
                    else
                    {
                        syncLocal = new object();
                        pendingConnections.Add(remoteNodeAddress, syncLocal);
                    }
                }

                lock (syncLocal)
                {
                    try
                    {
                        // verificacin
                        route = SearchRoute(remoteNodeAddress);
                        if (route == null)
                        {
                            toconnected = true;
                        }
                        else
                        {
                            toconnected |= !route.Connected;
                            toconnected |= route.Failed;
                        }

                        if (toconnected)
                        {
                            RemoveRoute(route);

                            route = new Route(localNode);
                            route.BufferStream.HandledMessage += 
                                new DodoNet.Http.HttpMessageHandler(localNode.HandledReceiveMessageFromRoute);

                            AsyncCallback cb = new AsyncCallback(CreateRouteCB);
                            IAsyncResult ar = route.BeginConnect(remoteNodeAddress, cb, route);

                            if (route.ConnectDone.WaitOne(TcpConfig.ConnectionTimeout, false))
                            {
                                route.EndConnect(ar);

                                if (route.Connected)
                                {
                                    // escuchamos mensajes que nos llegen por esta ruta
                                    localNode.mailboxIn.BeginReceive(route);
                                    AddRoute(route);
                                }
                                else
                                    throw new Exception(string.Format("GetRoute: Imposible conectar a {0}", remoteNodeAddress));
                            }
                            else
                                throw new Exception(string.Format("GetRoute: Imposible conectar a {0} tiempo max excedido {1}ms", remoteNodeAddress, TcpConfig.ConnectionTimeout));
                        }
                    }
                    catch (Exception err)
                    {
                        throw err;
                    }
                    finally
                    {
                        lock (sync)
                        {
                            if (pendingConnections.ContainsKey(remoteNodeAddress))
                                pendingConnections.Remove(remoteNodeAddress);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                if (route != null)
                    route.Dispose();
                throw err;
            }

            return route;
        }

        private void CreateRouteCB(IAsyncResult ar)
        {
            Route route = ar.AsyncState as Route;
            if (route != null) route.ConnectDone.Set();
        }

        public void AddRoute(Route r)
        {
            try
            {
                lock (sync)
                {
                    KeyRoutePair key = new KeyRoutePair(r.localNodeAddress, r.remoteNodeAddress);

                    table.Add(key, r);
                    tableRemote.Add(r.remoteNodeAddress, r);

                    Node.LogAppendLine("NodoId: {0} Nueva ruta local:{1} remote:{2}",
                        localNode.localBind.NodeId, r.localNodeAddress, r.remoteNodeAddress);
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine("AddRoute: " + err.Message);
            }
        }

        /// <summary>
        /// buscar una ruta a partir del identificador remoto del nodo al que pertenece
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public Route SearchRoute(NodeBind remoteNodeBind)
        {
            Route ret = null;
            try
            {
                // si existe nodeAddress
                if (remoteNodeBind.NodeAddress != null)
                    ret = SearchRoute(remoteNodeBind.NodeAddress);

                if (ret == null)
                {
                    KeyRoutePair key = null;
                    if (tableNodeId.TryGetValue(remoteNodeBind.NodeId, out key)) table.TryGetValue(key, out ret);
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine("GetRoute: " + err.Message);
            }
            return ret;
        }

        public Route SearchRoute(NodeAddress remoteNodeAddress)
        {
            Route ret = null;
            try
            {
                tableRemote.TryGetValue(remoteNodeAddress, out ret);
            }
            catch (Exception err)
            {
                Node.LogAppendLine("GetRoute: " + err.Message);
            }
            return ret;
        }

        public void RemoveRoute(Route r)
        {
            try
            {
                if (r != null)
                {
                    lock (sync)
                    {
                        Node.LogAppendLine("Eliminar ruta: {0}", r.remoteNodeAddress);

                        KeyRoutePair key = new KeyRoutePair(r.localNodeAddress, r.remoteNodeAddress);

                        if (table.ContainsKey(key))
                        {
                            table.Remove(key);
                            tableRemote.Remove(r.remoteNodeAddress);

                            // detruir ruta
                            r.Dispose();

                            // elimina r vinculos
                            UnlinkRoute(r);

                            /*
                            int y = 0;

                            StackTrace st = new StackTrace(true);

                            for (int x = 0; x < st.FrameCount; x++)
                            {
                                y++;
                                StackFrame sf = st.GetFrame(x);
                                Node.LogAppendLine("({0:000})\t {1} Line: {2} File:{3}",
                                    y, sf.GetMethod().Name, sf.GetFileLineNumber(), sf.GetFileName());
                            }

                            Node.LogAppendLine("NodoId: {0} Eliminada ruta local:{1} remote:{2}",
                                localNode.localBind.NodeId, r.localNodeAddress, r.remoteNodeAddress);
                            */
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
        }

        /// <summary>
        /// vicular una ruta a un identificador de nodo
        /// </summary>
        /// <param name="route"></param>
        /// <param name="nodeId"></param>
        public void LinkRoute(Route r, NodeId remoteNodeId)
        {
            try
            {
                lock (sync)
                {
                    KeyRoutePair key = new KeyRoutePair(r.localNodeAddress, r.remoteNodeAddress);

                    if (table.ContainsKey(key) && !tableNodeId.ContainsKey(remoteNodeId))
                    {
                        tableNodeId.Add(remoteNodeId, key);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// desvincular una ruta de un identificador de nodo
        /// </summary>
        /// <param name="route"></param>
        /// <param name="nodeId"></param>
        public void UnlinkRoute(Route r, NodeId remoteNodeId)
        {
            try
            {
                lock (sync)
                {
                    KeyRoutePair key = new KeyRoutePair(r.localNodeAddress, r.remoteNodeAddress);
                    tableNodeId.Remove(remoteNodeId);
                }
            }
            catch { }
        }

        public void UnlinkRoute(Route r)
        {
            try
            {
                lock (sync)
                {
                    KeyRoutePair key = new KeyRoutePair(r.localNodeAddress, r.remoteNodeAddress);
                    foreach (KeyValuePair<NodeId, KeyRoutePair> kvp in tableNodeId)
                    {
                        if (kvp.Value.Equals(key))
                        {
                            tableNodeId.Remove(kvp.Key);
                        }
                    }
                }
            }
            catch { }
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (!disposed)
                {
                    lock (sync)
                    {
                        // desechar todos las rutas
                        foreach (KeyValuePair<KeyRoutePair, Route> pair in table)
                        {
                            try
                            {
                                pair.Value.Dispose();
                            }
                            catch (Exception err)
                            {
                                Node.LogAppendLine("Error al destruir la ruta: {0}", err.Message);
                            }
                        }
                    }

                    table.Clear();
                    tableNodeId.Clear();

                    disposed = true;
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
        }

        #endregion
    }

    /// <summary>
    /// unique key between local and remote address
    /// </summary>
    public class KeyRoutePair : IComparable
    {
        public NodeAddress remoteEndPoint;
        public NodeAddress localEndPoint;

        public KeyRoutePair(NodeAddress localEndPoint, NodeAddress remoteEndPoint)
        {
            this.localEndPoint = localEndPoint;
            this.remoteEndPoint = remoteEndPoint;
        }

        public override int GetHashCode()
        {
            int hashVal = localEndPoint.GetHashCode();
            hashVal += remoteEndPoint.GetHashCode();
            return hashVal;
        }

        // Overridden equals (used for Hashtables)
        public override bool Equals(object obj)
        {
            if (!(obj is KeyRoutePair))
                return false;
            return Equals((KeyRoutePair)obj);
        }

        public bool Equals(KeyRoutePair key)
        {
            if (localEndPoint.Equals(key.localEndPoint) && remoteEndPoint.Equals(key.remoteEndPoint))
            {
                return true;
            }
            return false;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return GetHashCode().CompareTo(((KeyRoutePair)obj).GetHashCode());
        }

        #endregion
    }
}
