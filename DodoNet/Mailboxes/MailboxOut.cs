using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.EnvironmentInfo;

using DodoNet.Tcp;
using DodoNet.Http;

using Amib.Threading;
using Amib.Threading.Internal;

namespace DodoNet
{
    /// <summary>
    /// Cola de envio
    /// </summary>
    public class MailboxOut : IDisposable
    {
        #region subclasses

        internal class SendItemThread
        {
            internal NodeBind source;
            internal NodeBind dest;
            internal Message msg;
            internal RouteContCallback cb;

            internal SendItemThread(NodeBind source, NodeBind dest, Message msg, RouteContCallback cb)
            {
                this.source = source;
                this.dest = dest;
                this.msg = msg;
                this.cb = cb;
            }
        }

        #endregion

        #region Header

        private Thread threadProcSendPackets;
        public Thread ThreadProcSendPackets { get { return threadProcSendPackets; } }

        public bool disposed;
        public bool sending;
        public OutQueue outQueue;
        public Node localNode;

        // threads
        private int MAX_THREADS = 64;

        // config
        int bandwidthMax = DodoConfig.bandwidthMax;

        /// <summary>
        /// Limite máximo de ancho de banda en Kilo bytes. 1024 bytes = 1 KB
        /// </summary>
        public int BandwidthMax { set { bandwidthMax = value; } get { return bandwidthMax; } }

        // performance counters
        private uint totalBytesSent = 0;

        DateTime intervalLastMeasure = DateTime.MinValue;
        double intervalSentBytes = 0;
        TimeSpan intervalTotalWait = TimeSpan.Zero;

        public int numPreparedItems = 0;
        public int numSentItems = 0;
        public int numPacketToSend = 0;
        public int numThreadCalls = 0;
        public int numSenderThreadLaps = 0;
        public int numSendPacketCalls = 0;

        public int numMaxRoutes = 0;

        private SmartThreadPool m_STP;

        IWorkItemsGroup wgKeepAlive;
        IWorkItemsGroup wgSendFrames;

        #endregion

        #region Properties

        /// <summary>
        /// Total de bytes enviados
        /// </summary>
        public uint TotalBytesSent { get { return totalBytesSent; } }

        #endregion

        #region Constructor

        public MailboxOut(Node localNode)
        {
            this.localNode = localNode;

            STPStartInfo s_info = new STPStartInfo();
            s_info.MinWorkerThreads = 1;
            s_info.MaxWorkerThreads = MAX_THREADS;
            m_STP = new SmartThreadPool(s_info);

            wgKeepAlive = m_STP.CreateWorkItemsGroup(50);
            wgSendFrames = m_STP.CreateWorkItemsGroup(50);

            sending = true;
            outQueue = new OutQueue(localNode);
            threadProcSendPackets = new Thread(new ThreadStart(ProcSendPacket));
            threadProcSendPackets.Name = string.Format("SenderPackets {0}", threadProcSendPackets.GetHashCode());
            threadProcSendPackets.Start();
        }

        #endregion

        #region Send Messages

        public void BeginSendKeepAlive(NodeBind source, NodeBind dest, RouteContCallback cb)
        {
            wgKeepAlive.QueueWorkItem(new WorkItemCallback(BeginSendKeepAlive_tt),
                new object[] { source, dest, cb }, WorkItemPriority.Highest);
        }

        /// <summary>
        /// mandar keepalive
        /// </summary>
        /// <param name="dest"></param>
        internal object BeginSendKeepAlive_tt(object state)
        {
            bool success = true;
            Route route = null;
            Exception exception = null;

            // recoger argumentos
            object[] states = state as object[];

            NodeBind source = states[0] as NodeBind;
            NodeBind dest = states[1] as NodeBind;
            SendCtrlCallback cb = states[2] as SendCtrlCallback;

            try
            {
                route = localNode.RouteTable.GetRoute(dest.NodeAddress);
                Packet packet = new Packet(TypePacket.KeepAlive);
                AddPacketQueueOut(route, packet, true);
            }
            catch (Exception err)
            {
                exception = err;
                success = false;
                // llamamos al método para
                // localNode.NodeFailed(dest);
            }
            finally
            {
                try
                {
                    // llamamos al callback
                    if (cb != null)
                        cb(route, success, exception);
                }
                catch { }
            }
            return null;
        }

        public void BeginSend(NodeBind source, NodeBind dest, Message msg, RouteContCallback cb)
        {
            wgSendFrames.QueueWorkItem(new WorkItemCallback(BeginSend_tt),
                (object)new SendItemThread(source, dest, msg, cb), 
                WorkItemPriority.Normal);

            //-- BeginSend_tt((object)new SendItemThread(source, dest, msg, cb));
        }

        /// <summary>
        /// envia un mensaje troceado en paquetes
        /// </summary>
        internal object BeginSend_tt(object state)
        {
            Route route = null;
            bool success = true;
            Exception exception = null;

            SendItemThread tmp = (SendItemThread)state;

            NodeBind source = tmp.source;
            NodeBind dest = tmp.dest;
            Message msg = tmp.msg;
            RouteContCallback cb = tmp.cb;

            try
            {
                // firewall
                if (localNode.blockedAddresses.IndexOf(dest.NodeId) > -1)
                    throw new Exception("Dirección bloqueada");

                // conseguir la ruta para mandar los datos al destino
                route = localNode.RouteTable.GetRoute(dest.NodeAddress);

                byte[] buf = new byte[8192];
                int bytesRead;

                IHttpMessage httpMsg = msg.Serialize();

                using (var sourceStream = new HttpSenderStream(httpMsg))
                {
                    sourceStream.Position = 0;
                    // copy all data from in to out via the buffer layer
                    while ((bytesRead = sourceStream.Read(buf, 0, buf.Length)) > 0 
                        && !msg.Cancelled)
                    {
                        route.Send(buf, 0, bytesRead);
                        totalBytesSent += (uint)bytesRead;
                    }

                    if (msg.Cancelled)
                    {
                        Node.LogAppendLine("Envio cancelado {0}");
                    }
                }
            }
            catch (Exception err)
            {
                exception = err;
                success = false;
            }
            finally
            {
                try
                {
                    // llamamos al callback
                    if (cb != null)
                        cb(localNode, msg, route, success, exception);
                }
                catch { }
            }
            return null;
        }

        #endregion

        #region Send Items

        /// <summary>
        /// Añade un paquete a la cola de salida de paquetes. Si se completa la cola de salida, espera hasta
        /// estar de nuevo disponible
        /// </summary>
        /// <param name="__sp"></param>
        /// <returns></returns>
        public bool AddPacketQueueOut(Route route, Packet packet, bool ctrl)
        {
            ItemOutQueue item = null;

            try
            {
                bool ret = false;

                if (route != null)
                {
                    // contador
                    Interlocked.Increment(ref numPacketToSend);

                    // creamos el item de la cola de salida
                    item = new ItemOutQueue(route, packet, outQueue);

                    // contador de items preparados
                    numPreparedItems++;

                    // enviar item
                    SendItem(item);

                    // comprobamos que no haya habido error al enviar el paquete
                    // - como hemos tenido que esperar para enviar el paquete 
                    //   si hubo error al enviarlo lo lanzamos aqui
                    if (item.exception != null)
                        throw item.exception;
                }
                else
                {
                    Exception err = new Exception("Paquete sin dirección");
                    Node.LogAppendLine("#@ Error Enviar el paquete sin dirección {0}", err.Message);
                    throw err;
                }

                return ret;
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                if (item != null)
                    item.Dispose();
            }
        }

        /// <summary>
        /// llamado desde la hebra q va a enviar los mensajes de la cola
        /// </summary>
        public void ProcSendPacket()
        {
            while (sending)
            {
                // contador
                Interlocked.Increment(ref numSenderThreadLaps);

                ItemOutQueue item = null;
                try
                {
                    item = outQueue.NextItem();

                    IWorkItemResult result = m_STP.QueueWorkItem(
                        new WorkItemCallback(SendItem), item);
                }
                catch (Exception err)
                {
                    Node.LogAppendLine("#@ Error Enviar el paquete", err.Message);

                    if (item != null)
                        item.exception = new DodoException(err, "Error al enviar el paquete");

                    /*
                    if (item != null)
                        item.Dispose();
                    */
                }
            }
        }

        /// <summary>
        /// enviar paquetes de salida
        /// </summary>
        /// <param name="__packet"></param>
        /// <param name="__dest"></param>
        private object SendItem(object param)
        {
            // contador
            Interlocked.Increment(ref numSendPacketCalls);

            ItemOutQueue item = null;
            try
            {
                item = (ItemOutQueue)param;

                // calculamos cuanto tenemos que esperar para mantener el ancho de banda
                DateTime wait = AcpTransmissionInterval();

                TimeSpan tsWaiting = (TimeSpan)wait.Subtract(DateTime.Now);
                if (tsWaiting.Ticks > 0)
                    Thread.Sleep(tsWaiting);

                item.route.Send(item.packet.GetBytes(), item.idx, item.packet.GetNumBytes());

                // marcamos la fecha/hora de la ultima keepalive
                item.route.UpdateActivity();

                // actualizar contadores
                UpdatePerformanceBytes(item.packet.GetNumBytes());

                // actualizamos los contadores
                numSentItems++;
                numPreparedItems--;
            }
            catch (ObjectDisposedException err)
            {
                item.exception = new DodoException(err, "Error al enviar un paquete a {0} Error: {1} Nombre objeto: {2}", item.dest,
                    err.Message, err.ObjectName);
                item.sendFailed = false;
            }
            catch (SocketException err)
            {
                item.exception = new DodoException(err, "Error al enviar un paquete a {0} socketError: {1}",
                    item.dest.IPEndPoint, err.ErrorCode);
                item.sendFailed = false;
            }
            catch (Exception err)
            {
                item.exception = new DodoException(err, "Error al enviar un paquete a {0} Error: {1}", item.dest,
                    err.Message);
                item.sendFailed = false;
            }
            finally
            {
                if (item != null)
                {
                    try
                    {
                        if (item.sendFailed)
                        {
                            localNode.RouteTable.RemoveRoute(item.route);

                            Node.LogAppendLine("No se pudo enviar mensaje por la ruta {0}", item.route.remoteNodeAddress);
                        }

                        // terminamos de usar la ruta
                        item.route.InUse = false;
                        // fin de la espera
                        item.waitingEnd = DateTime.Now.TimeOfDay.ToString();
                        // debloqueamos hebra
                        item.packetSent.Set();
                        // if some thread is waiting for this route
                        item.outQueue.WakeUp();
                        // destruimos el objeto
                        //-- item.Dispose();
                    }
                    catch (Exception err)
                    {
                        Node.LogAppendLine(err);
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// calcular el tiempo que hay q esperar para mantener el ancho de banda
        /// </summary>
        /// <returns></returns>
        DateTime AcpTransmissionInterval()
        {
            DateTime ret = DateTime.MinValue;
            DateTime curTime = DateTime.Now;
            try
            {
                if (totalBytesSent > 0 && bandwidthMax > 0)
                {
                    TimeSpan interval = DateTime.Now.Subtract(intervalLastMeasure);
                    double speed = (intervalSentBytes / 1024) / interval.TotalSeconds;
                    if (speed > bandwidthMax && bandwidthMax > 0)
                    {
                        double waitInSecs = ((intervalSentBytes / 1024) / (double)bandwidthMax) - interval.TotalSeconds;
                        ret = curTime.AddSeconds(waitInSecs);
                    }
                    else
                    {
                        ret = curTime;
                    }

                    intervalLastMeasure = DateTime.Now;
                    intervalSentBytes = 0;
                }
            }
            catch (DivideByZeroException) { }

            return ret;
        }

        #endregion

        #region Counters

        /// <summary>
        /// actualizacion de los contadores de numero de bytes
        /// </summary>
        /// <param name="bytes"></param>
        public void UpdatePerformanceBytes(int bytes)
        {
            int total = bytes + DodoConfig.IPHeaderOverhead;

            // contamos el numero de bytes enviados en un intervalo hasta que pase el updatePerformanceCounters
            totalBytesSent += (uint)total;

            intervalSentBytes += total;
        }

        #endregion

        #region Miembros de IDisposable

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                sending = false;

                outQueue.Dispose();

                threadProcSendPackets.Join();

                m_STP.Dispose();
            }
        }

        #endregion
    }

    /// <summary>
    /// para crear colas de salida
    /// </summary>
    public class OutQueue
    {
        private Queue list;

        private int countItems;
        private bool working;
        private object sync = new object();
        private Node localNode;

        private int countAdded = 0;
        private int countRemoved = 0;

        private Hashtable debug = new Hashtable();

        public OutQueue(Node localNode)
        {
            lock (sync)
            {
                list = new Queue();

                countItems = 0;
                working = true;

                this.localNode = localNode;
            }
        }

        public void WakeUp()
        {
            lock (sync)
            {
                // wake-up anyone waiting on an empty queue
                Monitor.Pulse(sync);
            }
        }

        /// <summary>
        /// numero mensajes en la cola
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                return countItems;
            }
        }

        public void Add(ItemOutQueue item, bool first)
        {
            // contador
            Interlocked.Increment(ref countAdded);

            lock (sync)
            {
                try
                {
                    list.Enqueue(item);

                    // aumentamos el contador de items
                    countItems++;
                }
                catch (Exception err)
                {
                    Node.LogAppendLine("#@ Error al añadir el paquete a la cola de salida {0}",
                        err.Message);
                    item.packetSent.Set();
                    throw err;
                }
                finally
                {
                    // wake-up anyone waiting on an empty queue
                    Monitor.Pulse(sync);
                }
            }
        }

        /// <summary>
        /// obtener el siguiente paquete o esperar hasta q haya uno
        /// </summary>
        /// <returns></returns>
        public ItemOutQueue NextItem()
        {
            // contador
            Interlocked.Increment(ref countRemoved);

            lock (sync)
            {
                ItemOutQueue ret = null;
                try
                {
                    while (working)
                    {
                        if (list.Count > 0)
                        {
                            for (int x = 0; x < list.Count; x++)
                            {
                                ret = list.Peek() as ItemOutQueue;
                                if (!ret.route.InUse)
                                {
                                    list.Dequeue();
                                    // marcamos la ruta
                                    ret.route.InUse = true;
                                    // disminuimos el contador
                                    countItems--;
                                    break;
                                }
                                else
                                    ret = null;
                            }
                        }

                        if (ret == null)
                            // if we don't find anything to send, just wait for a signal
                            Monitor.Wait(sync);
                        else
                            break;
                    }
                    return ret;
                }
                catch (Exception err)
                {
                    Node.LogAppendLine("#@ Error Enviar el paquete sin dirección", err.Message);
                    return ret;
                }
            }
        }

        public void Dispose()
        {
            lock (sync)
            {
                working = false;
                list.Clear();
                WakeUp();
            }
        }
    }

    /// <summary>
    /// elemento de la cola de salida de mensajes
    /// </summary>
    public class ItemOutQueue : IDisposable
    {
        public Route route;
        public NodeAddress dest;
        public Packet packet;
        public ManualResetEvent packetSent; // manual reset event para envios sincronos 
        public Exception exception;
        public int idx = 0;	// posicion actual de cursor
        public bool sendFailed; // hubo fallo
        public OutQueue outQueue;

        // tiempos
        public string waitingStart;
        public string waitingEnd;

        public ItemOutQueue(Route route, Packet packet, OutQueue outQueue)
        {
            this.route = route;
            this.dest = route.remoteNodeAddress;
            this.packet = packet;
            this.packetSent = new ManualResetEvent(false);
            this.outQueue = outQueue;
        }

        public void Dispose()
        {
            try
            {
                // si tiene un manual reset event, lo desbloquemos
                if (packetSent != null)
                    packetSent.Set();

                packetSent.Close();
                packet.Dispose();
                packet = null;
            }
            catch { }
        }
    }
}

