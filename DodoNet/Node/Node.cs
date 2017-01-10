using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tt = System.Threading;

using DodoNet.Overlay;
using DodoNet.Session;
using DodoNet.Http;
using DodoNet.Logs;
using DodoNet.Web;

namespace DodoNet
{
    public class Node : IDisposable
    {
        #region Properties

        public NodeBind localBind;
        public ILocalEvents localEvents;

        public RequestReplyTable RequestReplyTable { get; set; }
        public RequestReplyWebTable RequestReplyWebTable { get; set; }

        public NodeInfo localInfo = new NodeInfo();

        public int numErrors = 0;
        public int numReceivedRequest = 0;
        public int numSentReply = 0;

        private int counterMsg;

        DateTime initiatedNode = DateTime.Now;
        public DateTime InitiatedNode { get { return InitiatedNode; } }

        // variables de control
        public DateTime lastKeepAliveCheck;

        // coleciones propiedades
        protected AppCollection apps;
        public AppCollection Apps { get { return apps; } }

        // sesiones
        SessionCollection sessions;
        public SessionCollection Sessions { get { return sessions; } }

        // routing tables
        RouteCollection routeTable;
        public RouteCollection RouteTable { get { return routeTable; } }

        // directorio
        DirectoryCollection directory;
        public DirectoryCollection Directory { get { return directory; } }

        // conexiones a nodos
        SessionCollection seeds;
        public SessionCollection Seeds { get { return seeds; } }

        public Hashtable pendingSessions = new Hashtable();

        public MailboxIn mailboxIn;
        public MailboxOut mailboxOut;

        public WebApplication WebApplicationDefault;

        /// <summary>
        /// direcciones bloqueadas
        /// </summary>
        public List<NodeId> blockedAddresses = new List<NodeId>();

        public static FileLog log;

        #endregion

        #region statics

        public static void LogAppendLine(string text, params object[] args)
        {
            log.AppendLine(text, args);
        }

        public static void LogAppendLine(Exception err)
        {
            log.AppendLine(err);
        }

        #endregion

        #region Constructors

        public Node()
        {
            var nodeAddress = new NodeAddress();
            var nodeId = new NodeId();

            localBind = new NodeBind(nodeId, nodeAddress);

            _Initialize();
        }
        
        public Node(NodeBind localBind)
        {
            this.localBind = localBind;

            _Initialize();
        }

        private void _Initialize()
        {
            mailboxIn = new MailboxIn(this, localBind.NodeAddress);
            mailboxIn.Activate();

            routeTable = new RouteCollection(this);
            mailboxOut = new MailboxOut(this);

            RequestReplyTable = new RequestReplyTable(this);
            RequestReplyWebTable = new RequestReplyWebTable(this);

            apps = new AppCollection();   
        }

        public void Activate(ILocalEvents iLocalEvents)
        {
            this.localEvents = iLocalEvents;
            
            localEvents.Activate(this);
        }

        #endregion

        #region Scheduling

        /// <summary>
        /// Schedule a continuation to execute.
        /// </summary>
        public void ScheduleContinuation(Continuation cont)
        {
            ScheduleLocalMessage(0, new LocalMessage(cont));
        }

        /// <summary>
        /// activate timer
        /// </summary>
        /// <param name="t"></param>
        public void SetTimer(Timer t)
        {
            ScheduleLocalMessage(t.NextDelayMs, new LocalMessage(t));
        }

        /// <summary>
        /// Cancel a pending timer.
        /// </summary>
        public void CancelTimer(Timer t)
        {
            // Debug.Assert(t != null);
            LocalMessage msg = new LocalMessage(t);
            // por si fuera periodico indicamos que ya no
            // para que no vuelva a ser lanzado
            t.Periodic = false;
            localEvents.RemoveEvent(msg);
            msg.Dispose();
        }

        /// <summary>
        /// throw local message
        /// </summary>
        /// <param name="delayMs"></param>
        /// <param name="localMessage"></param>
        private void ScheduleLocalMessage(long delayMs, LocalMessage localMessage)
        {
            localEvents.RaiseEvent(delayMs, localMessage);
        }

        #endregion

        #region Routing

        /// <summary>
        /// lets route message
        /// </summary>
        /// <param name="message"></param>
        internal void ResumeRouting(Message message)
        {
            message.Hops++;
            message.AddToPath(localBind);

            // lets route message
            RouterCont routerCont = new RouterCont(message);
            routerCont.Execute(this);
        }

        #endregion

        #region Sessions

        public SessionReply InitializeSession(SessionRequest session, NodeBind dest)
        {
            return null;
        }

        public void InitiatedSession(SessionRequest session)
        {
        }

        public void ReconnectSession(SessionRequest session)
        {
            try
            {
                // comprobamos que no esté conectado
                if (session.State != SessionStates.connected)
                {
                    TimeSpan diff = DateTime.Now.Subtract(session.LastAttemptReconnection);
                    if (diff.TotalSeconds > DodoConfig.SecsLapsedForReconnect)
                    {
                        // guardamos la hora/fecha del intento
                        session.LastAttemptReconnection = DateTime.Now;
                        // cambiamos el estado
                        session.State = SessionStates.reconnecting;
                        // inicializamos la sesion
                        InitializeSession(session, session.NodeBindRemote);
                    }
                }
            }
            catch (Exception err)
            {
                LogAppendLine("error en InitiatedSession {0}, {1}", err.Source, err.Message);
            }
        }

        public void FinalizeSession(NodeId nodeId, bool failed)
        {
        }

        internal bool NodeFailed(NodeBind nodeFailed)
        {
            bool ret = false;
            try
            {
                ret |= directory.NodeFailed(nodeFailed);
            }
            catch { }
            return ret;
        }

        #endregion

        #region Apps

        /// <summary>
        /// get the application from application Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetApp<T>()
        {
            T ret = default(T);
            object tmp = apps.GetApplication(typeof(T));
            if (tmp != null)
                ret = (T)tmp;
            return ret;
        }

        /// <summary>
        /// Register an application that lives on top of the overlay
        /// </summary>
        public void RegisterApp(App app, Object key)
        {
            apps.Add(app, key);
        }

        /// <summary>
        /// Unregister an application that lives on top of the overlay
        /// </summary>
        public void UnregisterApp(App app, Object key)
        {
            apps.Remove(app, key);
        }

        #endregion

        #region wire methods

        /// <summary>
        /// send message to other node
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="dest"></param>
        /// <param name="callback"></param>
        internal void SendMessageAsync(NodeBind dest, Message msg, RouteContCallback callback)
        {
            // tasks before sending
            msg.MessageSourceHook(this);
            // send by wire
            mailboxOut.BeginSend(localBind, dest, msg, callback);
        }

        /// <summary>
        /// when receiving a message
        /// </summary>
        /// <param name="msg"></param>
        internal void ReceiveMessageAsync(NodeBind src, Message msg, Route route)
        {
            // callback message sent
            msg.ReceiveCallback(this, route, src);
            // metemos el mensaje en la cola local de eventos
            localEvents.RaiseEvent(0, msg);
        }

        #endregion

        #region overlay methods

        public void HandledReceiveMessageFromRoute(Route route, IHttpMessage message)
        {
            Message msg = null;

            if (message.ContentType == HttpConst.ContentTypeDodoNetObject)
                msg = new DodoMessage();
            else
                msg = new WebMessage();

            msg.Deserialization(this, route, message);

            // lanzamos el evento
            ReceiveMessageAsync(msg.Source, msg, route);
        }

        public IAsyncResult BeginSendRequest(HttpRequest request, 
            AsyncCallback callback, object state)
        {
            var dest = new NodeBind(request.Url);

            request.Async = new RequestAsyncResult(request, callback, state);

            // create transporter message 
            var m = new WebMessage()
            {
                Source = localBind,
                Destiny = dest,
                CloseConnection = false
            };

            m.Payload = request;

            ResumeRouting(m);

            return (IAsyncResult)request.Async;
        }

        public HttpReply SendRequest(HttpRequest request)
        {
            HttpReply ret = null;
            RequestAsyncResult rar = BeginSendRequest(request, null, null) as RequestAsyncResult;
            rar.AsyncWaitHandle.WaitOne();
            ret = (HttpReply)rar.Reply;
            return ret;
        }

        public IAsyncResult BeginSendRequest(OverlayRequest request, NodeBind dest, 
            AsyncCallback callback, object state)
        {
            request.Async = new RequestAsyncResult(request, callback, state);

            // create transporter message 
            var m = new DodoMessage(request)
            {
                Source = localBind,
                Destiny = dest
            };

            ResumeRouting(m);

            return (IAsyncResult)request.Async;
        }

        public void EndSendRequest(IAsyncResult ar)
        {
            OverlayReply ret = null;
            RequestAsyncResult async = (RequestAsyncResult)ar;
            if (async.success)
                ret = async.Reply;
            else
                throw async.exception;
        }

        public OverlayReply SendRequest(OverlayRequest request, NodeBind dest)
        {
            OverlayReply ret = null;
            RequestAsyncResult rar = BeginSendRequest(request, dest, null, null) as RequestAsyncResult;
            rar.AsyncWaitHandle.WaitOne(2000);
            ret = rar.Reply;
            return ret;
        }

        public void SendReply(OverlayReply reply, NodeBind dest)
        {
            numSentReply++;
            RequestReplyTable.AddReply(dest, reply);

            // create transporter message 
            var m = new DodoMessage(reply)
            {
                Source = localBind,
                Destiny = dest
            };            

            // put in wire
            ResumeRouting(m);
        }

        public void SendReply(HttpReply reply, NodeBind dest, bool closeConnection)
        {
            numSentReply++;
            RequestReplyTable.AddReply(dest, reply);

            WebMessage m = new WebMessage()
            {
                Source = localBind, 
                Destiny = dest, 
                Payload = reply, 
                CloseConnection = closeConnection
            };

            // put in wire
            ResumeRouting(m);
        }

        /// <summary>
        /// mandar keepalive a una dirección física
        /// </summary>
        /// <param name="dest"></param>
        public void SendKeepAlive(NodeBind dest)
        {
        }

        /// <summary>
        /// mandar keep-alive para verificar si la sesión esta conectada al seed
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="auth"></param>
        public void SendKeepAlive(NodeBind dest, string auth)
        {
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            mailboxIn.Dispose();
            mailboxOut.Dispose();
            localEvents.Dispose();
        }

        #endregion
    }
}
