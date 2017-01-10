using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using DodoNet;

namespace DodoNet.Session
{
    /// <summary>
    /// Sirve para administrar las sesiones conectadas a un Nodo
    /// </summary>
    public class SessionCollection : IDisposable
    {
        #region Head

        Dictionary<string, ISessionRequest> tableById = new Dictionary<string, ISessionRequest>();
        Dictionary<string, ISessionRequest> tableByNodeId = new Dictionary<string, ISessionRequest>();

        object sync = new object();
        public object Sync { get { return sync; } }

        bool disposed = false;

        #endregion

        #region Properties

        public ISessionRequest this[string key]
        {
            get
            {
                return tableByNodeId[key];
            }

            set
            {
                lock (sync)
                {
                    tableByNodeId[key] = value;
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
                    ret = tableById.Count;
                }
                catch (Exception err)
                {
                    Node.LogAppendLine(err);
                }
                return ret;
            }
        }

        #endregion

        #region Constructor

        public SessionCollection()
        {
        }

        #endregion

        public bool ExistsById(string key)
        {
            bool ret = false;
            try
            {
                ret = tableById.ContainsKey(key);
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
            return ret;
        }

        public bool ExistsByNodeId(NodeId nodeId)
        {
            bool ret = false;
            try
            {
                ret = tableByNodeId.ContainsKey(nodeId.Id);
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
            return ret;
        }

        public ISessionRequest GetSession(NodeId nodeId)
        {
            ISessionRequest ret = null;
            try
            {
                if (tableByNodeId.ContainsKey(nodeId.Id))
                {
                    ret = tableByNodeId[nodeId.Id];
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
            return ret;
        }

        public ISessionRequest GetSession(string auth)
        {
            ISessionRequest ret = null;
            try
            {
                if (tableById.ContainsKey(auth))
                {
                    ret = tableById[auth];
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
            return ret;
        }

        public ISessionRequest[] GetArray()
        {
            lock (sync)
            {
                ISessionRequest[] ret = new ISessionRequest[tableById.Count];
                tableById.Values.CopyTo(ret, 0);
                return ret;
            }
        }

        public void AddSession(ISessionRequest session)
        {
            try
            {
                lock (sync)
                {
                    if (!tableById.ContainsKey(session.Auth))
                    {
                        tableById.Add(session.Auth, session);
                        tableByNodeId.Add(session.NodeBindApplicant.NodeId.Id, session);
                    }
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
        }

        public void AddSession(NodeId nodeId, ISessionRequest session)
        {
            try
            {
                lock (sync)
                {
                    if (!tableById.ContainsKey(session.Auth))
                    {
                        tableById.Add(session.Auth, session);

                        if (tableById[session.Auth].Auth != session.Auth)
                        {
                            Console.Write("Error");
                        }

                        tableByNodeId.Add(nodeId.Id, session);
                    }
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
        }

        public bool RemoveSession(ISessionRequest session)
        {

            lock (sync)
            {
                bool ret = false;
                try
                {
                    if (tableById.ContainsKey(session.Auth))
                    {
                        ret = tableById.Remove(session.Auth);
                        ret |= tableByNodeId.Remove(session.NodeBindApplicant.NodeId.Id);
                    }                    
                }
                catch (Exception err)
                {
                    Node.LogAppendLine(err);
                }
                return ret;
            }
        }

        public void RemoveSession(NodeId nodeId)
        {
            try
            {
                lock (sync)
                {
                    bool ret = false;
                    if (tableByNodeId.ContainsKey(nodeId.Id))
                    {
                        ISessionRequest session = tableByNodeId[nodeId.Id];
                        ret = tableById.Remove(session.Auth);
                        ret |= tableByNodeId.Remove(nodeId.Id);
                    }
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
        }

        public IEnumerator<KeyValuePair<string, ISessionRequest>> __GetEnumerator()
        {
            return tableById.GetEnumerator();
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
                        foreach (ISessionRequest o in tableById.Values)
                        {
                            try
                            {
                                o.Dispose(false);
                            }
                            catch { }
                        }

                        tableById.Clear();
                        tableByNodeId.Clear();
                    }
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
}
