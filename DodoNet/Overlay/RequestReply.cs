using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace DodoNet.Overlay
{
    /// <summary>
    /// Tabla de peticiones y respuestas
    /// </summary>
    public class RequestReplyTable : IDisposable
    {
        //**********************************************************************************************
        // Private Members
        //**********************************************************************************************
        Node localNode;
        int msgCounter;		                    // Counter for generating "unique" ids
        // Hashtable table;				        // Map from Msg ID strings to TableEntry objects
        Dictionary<string, TableEntry> table;	// Map from Msg ID strings to TableEntry objects
        object sync = new object();

        #region TableEntry

        public class TableEntry
        {
            // The following fields are populated at all nodes on a request's routing path */
            public string msgId;
            public OverlayReply reply;			// A reply. Populated only at a node that replies.
            public NodeBind replyDest;			// The destination of the reply message.
            public bool adCalled;			        // Has ArrivedAtDestination been called at this node?

            // The following fields are populated only at the source
            public OverlayRequest request;		// The original request message.
            public Message msg;				    // The original msg.
            public bool insertedTable;	            // If item is inserted in table
            public ReplyTimer replyTimer;           // Timer for doing resends / timeouts.
            public EntryGCTimer gcTimer;            // Timer for garbage-collector for request

            // Statistics
            public DateTime insertedTime;           // Time when was inserted
            public TimeSpan durationRequest;        // Duration request

            /// <summary>
            /// Constructor for use at the source node
            /// </summary>
            public TableEntry(string msgId, OverlayRequest request, Message msg)
            {
                this.insertedTable = false;
                this.msgId = msgId;
                this.request = request;
                this.msg = msg;
            }

            /// <summary>
            /// Constructor for use at intermediate nodes
            /// </summary>
            public TableEntry(string msgId)
            {
                this.msgId = msgId;
            }
        }

        #endregion

        #region Constructor

        //**********************************************************************************************
        // Constructor
        //**********************************************************************************************

        public RequestReplyTable(Node node)
        {
            this.localNode = node;
            //this.msgCounter = 0;
            this.table = new Dictionary<string, TableEntry>();
        }

        #endregion

        //**********************************************************************************************
        // Indexer
        //**********************************************************************************************

        public TableEntry this[string key]
        {
            get
            {
                TableEntry tmp = null;
                table.TryGetValue(key, out tmp);
                return tmp;
            }
            set
            {
                lock (sync)
                {
                    table[key] = value;
                }
            }
        }

        #region Requests

        /// <summary>
        /// conseguir la petición de una conversación
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetRequest<T>(string key)
        {
            T ret = default(T);
            TableEntry entry = (TableEntry)table[key];
            if (entry != null)
            {
                object tmp = entry.request;
                ret = (T)tmp;
            }
            return ret;
        }

        /// <summary>
        /// conseguir la petición de una conversación
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public OverlayRequest GetRequest(string key)
        {
            OverlayRequest ret = null;
            TableEntry entry = (TableEntry)table[key];
            if (entry != null)
            {
                ret = entry.request;
            }
            return ret;
        }

        //**********************************************************************************************
        // Requests
        //**********************************************************************************************

        /// <summary>
        /// Generate a new unique nodeId for a request message.
        /// </summary>
        public string GenerateRequestId()
        {
            string id = String.Format("{0}!{1}",
                localNode.localBind.NodeId, msgCounter);
            msgCounter++;
            return id;
        }

        /// <summary>
        /// Called by the overlay when a request message is being sent from the source.
        /// Sets up timeouts, etc.
        /// 
        /// Llamado cuando se envia la peticion
        /// </summary>
        /// <param name="request">The request message</param>
        public void HandleRequestAtSource(Message msg)
        {
            OverlayRequest request = (OverlayRequest)msg.Payload;
            string id = request.Id;
            // Get the current table entry (if any), or create a new one
            TableEntry entry;
            if (table.ContainsKey(id))
            {
                entry = table[id];
            }
            else
            {
                // Create a new entry and add it to the hash table
                entry = new TableEntry(id, request, msg);
                lock (sync)
                {
                    table.Add(id, entry);
                }
                entry.insertedTable = true;
                entry.insertedTime = DateTime.Now;
            }

            // Schedule the timer to handle retries and timeouts
            ReplyTimer timer = new ReplyTimer(request, request.TimeoutMs);
            entry.replyTimer = timer;
            localNode.SetTimer(timer);
        }

        /// <summary>
        /// llamado cuando surge un error y el mensaje no sale de su origen
        /// </summary>
        /// <param name="request"></param>		
        public void HandleExceptionAtSource(OverlayRequest request, Exception errAtSource)
        {
            string id = request.Id;

            TableEntry entry;

            if (table.ContainsKey(id))
            {
                // cogemos entrada en tabla
                entry = (TableEntry)table[id];
                // cancel timer
                localNode.CancelTimer(entry.replyTimer);

                if (request.Async != null)
                {
                    // señalamos el error
                    request.Async.success = false;
                    request.Async.exception = errAtSource;
                    // ejecutamos callback del usuario
                    ExecuteCallback(request);
                    // borramos de la tabla
                    RemoveEntry(id);
                }
                else
                {
                    // lanzamos la excepción si no hay objeto async
                    // han usado sendmessagedirect
                    // borramos de la tabla
                    RemoveEntry(id);
                    throw errAtSource;
                }
            }
        }

        /// <summary>
        /// Called by a reply message when it arrives at the original source.
        /// </summary>
        /// <param name="reply"></param>
        public void ReceivedReply(OverlayReply reply)
        {
            // Get the request's nodeId
            string id = reply.RequestId;

            // Look up the request's nodeId in the table.
            if (table.ContainsKey(id))
            {
                // Get the entry for this nodeId
                TableEntry entry = (TableEntry)table[id];
                if (entry.request == null)
                {
                    // If there is no request message, this node is not the source
                }
                else
                {
                    OverlayRequest request = entry.request;
                    // calculate duration
                    entry.durationRequest = DateTime.Now.Subtract(entry.insertedTime);

                    // Cancel the pending timer.
                    localNode.CancelTimer(entry.replyTimer);

                    if (request.Async != null)
                    {
                        // todo realizado adecuadamente
                        request.Async.success = true;

                        // comprobamos que nos hayan enviado un error
                        if (reply is OverlayError)
                        {
                            OverlayError overlayErr = (OverlayError)reply;
                            request.Async.success = false;
                            request.Async.exception = overlayErr.GetException();
                        }
                        else
                            request.Async.Reply = reply;

                        // ejecutamos el callback
                        ExecuteCallback(request);
                    }

                    // Remove the entry from the table
                    RemoveEntry(id);
                }
            }
        }

        /// <summary>
        /// se encarga de lanzar el callback. Tiene en cuenta el tipo de destino
        /// </summary>
        /// <param name="Callback"></param>
        internal void ExecuteCallback(OverlayRequest request)
        {
            // liberamos el ManualResetEvent
            ((ManualResetEvent)request.Async.AsyncWaitHandle).Set();
            // llamamos al callback de SendRequest si existe
            if (request.Async.Callback != null)
            {
                if (request.Async.Callback.Target is System.Windows.Forms.Control)
                {
                    System.Windows.Forms.Control targetControl =
                        (System.Windows.Forms.Control)request.Async.Callback.Target;
                    IAsyncResult ar = targetControl.BeginInvoke(request.Async.Callback,
                        new object[] { request.Async });
                }
                else
                {
                    request.Async.Callback(request.Async);
                }
            }
        }

        /// <summary>
        /// This method is called by a ReplyTimer when it fires.
        /// </summary>
        /// <param name="nodeId">The nodeId of the message whose timer fired</param>
        internal void ReplyTimerFired(string id)
        {
            if (table.ContainsKey(id))
            {
                // Get the table entry for this message nodeId
                // Debug.Assert( table.ContainsKey(nodeId) );
                TableEntry entry = (TableEntry)table[id];
                // Debug.Assert( entry!=null );
                OverlayRequest request = entry.request;
                Debug.Assert(request != null);

                // We have already retried this request enough times.
                // This time we just fail the request.
                // Cancel the pending timer.
                // brake sending message
                entry.msg.CancelSending();

                // Invoke the Request's callback with a null reply,
                // indicating no reply was received
                // Ha transcurrido el tiempo de espera sin respuesta, enviamos un mensaje de error.
                // lanzamos el error
                request.Async.success = false;
                request.Async.exception = new Exception(string.Format("Error en SendRequest: Se ha excedido el tiempo de espera establecido TimeOut: {0}ms", request.TimeoutMs));
                entry.reply = null;
                ExecuteCallback(request);

                // Remove the request from the RequestReplyTable
                RemoveEntry(request.Id);
            }
        }

        /// <summary>
        /// Called to unregister an entry from the table, either:
        ///  - when a reply is received
        ///  - when a timeout expires
        ///  - when state at intermediate nodes is garbage-collected
        /// </summary>
        /// <param name="request">The request message</param>
        internal void RemoveEntry(string id)
        {
            lock (sync)
            {
                // eliminar mensaje de la tabla
                table.Remove(id);
            }
        }

        #endregion

        #region ArrivedAtDestination

        //**********************************************************************************************
        // ArrivedAtDestination
        //**********************************************************************************************

        /// <summary>
        /// Called by the overlay when ArrivedAtDestination is about to be called for a request message.
        /// Sets up state to avoid calling ArrivedAtDestination multiple times.
        /// 
        /// Cuando llega al destino es llamado
        /// </summary>
        public void HandleArrivedAtDestination(Message msg)
        {
            OverlayRequest request = msg.Payload as OverlayRequest;

            try
            {
                // Get the request's nodeId
                string id = request.Id;
                // Look up the request's nodeId in the table.
                // Note: Normally, an entry for this nodeId will already exist.
                // But, sometimes during CLB routing, ArrivedAtDestination() is called
                // without ever calling CheckContinueRouting() at that node. 
                TableEntry entry;
                if (table.ContainsKey(id))
                {
                    // Get the entry for this nodeId
                    entry = (TableEntry)table[id];
                }
                else
                {
                    // Create a new entry for this nodeId and add it to the table
                    entry = new TableEntry(id, request, msg);

                    lock (sync)
                    {
                        table.Add(id, entry);
                    }
                }

                // Start a timer to garbage-collect this table entry
                if (entry.gcTimer == null)
                {
                    EntryGCTimer timer = new EntryGCTimer(request);
                    entry.gcTimer = timer;
                    localNode.SetTimer(timer);
                }

                // If ArrivedAtDestination has not already been called at this node, call it
                if (!entry.adCalled)
                {
                    entry.adCalled = true;
                    request.ArrivedAtDestination(localNode, msg);
                }
            }
            catch (Exception err)
            {
                // mandamos este error al nodo que hizo la petición
                OverlayError replyErrMsg = new OverlayError(request, err);
                localNode.SendReply(replyErrMsg, msg.Source);
                throw err;
            }
        }

        #endregion

        #region Replies

        //**********************************************************************************************
        // Replies
        //**********************************************************************************************

        /// <summary>
        /// Add a reply to the table.
        /// </summary>
        public void AddReply(NodeBind dest, OverlayReply reply)
        {
            // Get the table entry for this request nodeId (which should already exist)
            string id = reply.RequestId;
            if (table.ContainsKey(id))
            {
                // Stick this reply into the table entry
                TableEntry entry = (TableEntry)table[id];
                Debug.Assert(entry.reply == null);
                entry.reply = reply;
                entry.replyDest = dest;

                // la variable q dice si la entrada se ha insertado o no 
                // se marca cuando sale el mensaje; si el mensaje se envia
                // al mismo nodo del que salio esta varible es igual a true
                // por lo que no debemos borrar la entrada por que se necesita
                // para recibir la replica
                if (!entry.insertedTable)
                {
                    // cancelamos el timer
                    localNode.CancelTimer(entry.gcTimer);

                    // sino la eliminamos aqui habría que esperar al GC
                    // porque si el mismo nodo nos quiere enviar peticiones
                    // coincidirian con nodeId de peticiones anteriores
                    RemoveEntry(id);
                }
            }
            else
            {
                // No entry in the table.
                // This can occur if AllowDupCCR / AllowDupAD are true
                // Don't keep track of the reply
            }
        }

        #endregion

        #region Miembros de IDisposable

        public void Dispose()
        {
            lock (sync)
            {
                table.Clear();
            }
        }

        #endregion
    }

    /// <summary>
    /// Timer for implementing reply-timeouts.
    /// </summary>
    public class ReplyTimer : Timer
    {
        OverlayRequest request;

        public ReplyTimer(OverlayRequest request, long periodMs)
            : base(periodMs)
        {
            this.request = request;
        }

        /// <summary>
        /// Invoked when this timer fires (i.e., a reply is not received in time).
        /// </summary>
        public override void Execute(Node localNode)
        {
            localNode.RequestReplyTable.ReplyTimerFired(request.Id);
        }
    }

    /// <summary>
    /// Timer for garbage-collecting entries from the reply table.
    /// </summary>
    public class EntryGCTimer : Timer
    {
        const int TTL_FACTOR = 5;

        string msgId;

        /// <summary>
        /// Constructor. Determine the GC interval and record the message nodeId.
        /// </summary>
        public EntryGCTimer(OverlayRequest request)
            : base(TTL_FACTOR * request.TimeoutMs)
        {
            this.msgId = request.Id;
        }

        /// <summary>
        /// Invoked when a table entry is to be garbage-collected.
        /// </summary>
        public override void Execute(Node localNode)
        {
            Node.LogAppendLine("Desechado el mensaje de peticion: {0}", msgId);

            // cancelando evento
            RequestReplyTable.TableEntry entry = localNode.RequestReplyTable[msgId];
            if (entry != null)
                // paramos hebra de ejecución
                entry.msg.threadLocalEvent.Cancel();
            
            localNode.RequestReplyTable.RemoveEntry(msgId);
        }
    }
}
