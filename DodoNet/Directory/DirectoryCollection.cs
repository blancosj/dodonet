using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;

using DodoNet.Overlay;
using DodoNet.Session;

namespace DodoNet
{
    /// <summary>
    /// Almacena las direcciones y sus correspondientes a identificadores
    /// </summary>
    public class DirectoryCollection : IDisposable
    {
        // referencias
        Node localNode;

        // tablas
        Dictionary<NodeId, DirectoryEntry> tableInner = new Dictionary<NodeId, DirectoryEntry>();

        // control concurrencia 
        object sync = new object();

        public int Count 
        { 
            get 
            {
                return tableInner.Count;
            } 
        }

        #region Directory Entry

        public class DirectoryEntry
        {
            NodeBind nodeBind;
            public NodeBind NodeBind { get { return nodeBind; } }

            NodeBind seedBind;
            public NodeBind SeedBind { get { return seedBind; } }

            bool routedBySeed = false;
            public bool RoutedBySeed { get { return routedBySeed; } set { routedBySeed = value; } }

            public DirectoryEntry() { }

            public DirectoryEntry(NodeBind seedBind, NodeBind nodeBind)
            {
                this.seedBind = seedBind;
                this.nodeBind = nodeBind;
            }
        }

        #endregion

        public DirectoryCollection(Node localNode)
        {
            this.localNode = localNode;
        }

        #region Resolve functions

        /// <summary>
        /// resuelve un nombre, se le pasa un objeto NodeBind sin direccion
        /// física y devuelve la dirección física correspondiente con el nodeId
        /// del objeto NodeBind pasado
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public NodeBind Resolve(NodeId nodeId)
        {
            NodeBind ret = null;
            try
            {
                if (tableInner.ContainsKey(nodeId))
                {
                    DirectoryEntry entry = new DirectoryEntry();
                    if (tableInner.TryGetValue(nodeId, out entry))
                    {
                        if (entry.RoutedBySeed)
                        {
                            ret = entry.SeedBind;
                        }
                        else
                        {
                            ret = entry.NodeBind;
                        }
                    }
                }
                else if (localNode.Sessions.ExistsByNodeId(nodeId))
                {
                    // buscamos en las sesiones propias y sino en las resoluciones propias
                    ISessionRequest session = localNode.Sessions.GetSession(nodeId);
                    ret = session.NodeBindApplicant;
                }
                else
                {
                    // ISessionRequest[] seeds = localNode.Seeds.CloneArray();
                    // foreach (ISessionRequest seed in seeds)
                    // IEnumerator<KeyValuePair<string, ISessionRequest>> seed = localNode.Seeds.GetArray();
                    foreach (ISessionRequest seed in localNode.Seeds.GetArray())
                    {
                        ResolveRequest rreq = new ResolveRequest(nodeId, localNode);
                        ResolveReply reply = (ResolveReply)localNode.SendRequest(rreq, seed.NodeBindRemote);
                        if (reply != null)
                        {
                            if (reply.resolvedOk)
                            {
                                DirectoryEntry entry = new DirectoryEntry(seed.NodeBindRemote, reply.resolvedNodeBind);
                                localNode.Directory.AddEntryInner(entry);
                                ret = reply.resolvedNodeBind;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Node.LogAppendLine(e);
            }
            return ret;
        }

        /// <summary>
        /// marcar como destino enrutado
        /// </summary>
        /// <param name="nodeBind"></param>
        public void MarkLikeRoutedDestiny(NodeId nodeId)
        {
            // lock (sync)
            // {
                if (tableInner.ContainsKey(nodeId))
                {
                    DirectoryEntry entry = new DirectoryEntry();
                    if (tableInner.TryGetValue(nodeId, out entry))
                    {
                        entry.RoutedBySeed = true;
                    }
                }
            // }
        }

        /// <summary>
        /// destino para enrutar
        /// </summary>
        /// <returns></returns>
        public NodeBind ResolveByRoute(NodeId nodeId)
        {
            NodeBind ret = null;
            try
            {
                DirectoryEntry entry = new DirectoryEntry();
                if (tableInner.TryGetValue(nodeId, out entry))
                {
                    ret = entry.SeedBind;
                }
                else
                {
                    foreach (ISessionRequest s in localNode.Seeds.GetArray())
                    {
                        ret = s.NodeBindRemote;
                        break;
                    }
                }
            }
            catch { }
            return ret;
        }

        /// <summary>
        /// Busca la dirección correspondiente al identificador pedido, suele ser una
        /// peticion externa
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public NodeBind ResolveOuter(NodeId nodeId)
        {
            NodeBind ret = null;

            try
            {
                ret = Resolve(nodeId);
            }
            catch (Exception e) 
            { 
                Console.WriteLine("Error en ResolveOuter {0}", e.Message); 
            }

            return ret;
        }

        #endregion

        #region Add functions

        /// <summary>
        /// añadir entrada en el directorio interno, conexiones establecidas desde nodos externos
        /// </summary>
        public void AddEntryInner(DirectoryEntry entry)
        {
            lock (sync)
            {
                if (entry.NodeBind.NodeId != null && entry.NodeBind.NodeAddress != null)
                {
                    if (!tableInner.ContainsKey(entry.NodeBind.NodeId))
                    {
                        tableInner.Add(entry.NodeBind.NodeId, entry);
                    }
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (sync)
            {
                tableInner.Clear();   
            }
        }

        #endregion

        /// <summary>
        /// fallo en el nodo siguiente
        /// </summary>
        /// <param name="nodeBind"></param>
        /// <returns></returns>
        public bool NodeFailed(NodeBind nodeFailed)
        {
            bool ret = false;
            try
            {
                lock (sync)
                {
                    ret = tableInner.Remove(nodeFailed.NodeId);
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
            return ret;
        }

        #region Remove functions

        /// <summary>
        /// elimina una entrada del directorio
        /// </summary>
        /// <param name="__address"></param>
        public bool RemoveEntryInner(NodeId nodeId)
        {
            lock (sync)
            {
                return tableInner.Remove(nodeId);
            }            
        }

        /// <summary>
        /// elimina una entrada del directorio
        /// </summary>
        /// <param name="__address"></param>
        public void RemoveEntryInner(SessionRequest session)
        {
            lock (sync)
            {
                foreach (KeyValuePair<NodeId, DirectoryEntry> de in tableInner)
                {
                    if (de.Value.NodeBind.NodeId.Equals(session.NodeBindApplicant.NodeId))
                    {
                        tableInner.Remove(session.NodeBindApplicant.NodeId);
                        // log
                        Node.LogAppendLine("quitado del directorio externo {0}", session.NodeBindApplicant.NodeId);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// eliminar una entrada del directorio a partir del IPEndPoint
        /// </summary>
        /// <param name="iep"></param>
        public void RemoveEntryInner(IPEndPoint iep)
        {
            lock (sync)
            {
                foreach (KeyValuePair<NodeId, DirectoryEntry> de in tableInner)
                {
                    DirectoryEntry entry = de.Value as DirectoryEntry;
                    if (de.Value.NodeBind.NodeAddress.ToString() == iep.ToString())
                    {
                        tableInner.Remove(entry.NodeBind.NodeId);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
