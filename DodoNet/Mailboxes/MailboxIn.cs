using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnvironmentInfo;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

using DodoNet.Http;
using DodoNet.Overlay;
using DodoNet.Session;
using DodoNet.Tcp;
using DodoNet.Tools;

namespace DodoNet
{
    /// <summary>
    /// cola de recepcion
    /// </summary>
    public class MailboxIn : IDisposable
    {
        #region Header

        // cola de entrada
        private Node localNode;
        private bool disposed = false;

        Route inRoute;

        public NodeAddress LocalNodeAddress { get { return inRoute.localNodeAddress; } }

        // subprocesos
        bool listening;
        AutoResetEvent eventListening;
        public Thread ttListening;

        uint bytesReceived = 0;
        /// <summary>
        /// Total de bytes recibidos
        /// </summary>
        public uint TotalBytesReceived { get { return bytesReceived; } }

        #endregion

        #region Constructor

        public MailboxIn(Node localNode, NodeAddress nodeAddress)
        {
            // oficina
            this.localNode = localNode;

            // creamos una ruta para crearnos una direccion de la oficina de correos
            inRoute = new Route(localNode);
            inRoute.BufferStream.HandledMessage += 
                new HttpMessageHandler(localNode.HandledReceiveMessageFromRoute);
            inRoute.Build(nodeAddress);
        }

        /// <summary>
        /// Activa el módulo de conexiones y mensajes entrantes
        /// </summary>
        public void Activate()
        {
            // comenzar a escuchar conexiones
            StartListening();
        }

        #endregion

        #region Receive data

        /// <summary>
        /// añadir un SocketState
        /// </summary>
        /// <param name="__ss"></param>
        public void BeginReceive(Route route)
        {
            // empezamos a recibir mensajes
            route.BeginReceive(route.bBufferIn, 0,
                route.bBufferIn.Length - 1,
                new AsyncCallback(ContinuationReceiveCallback), route);
        }

        internal void ContinuationReceiveCallback(IAsyncResult iar)
        {
            Route route = null;
            try
            {
                // cogemos el objeto socketstate
                route = (Route)iar.AsyncState;

                if (route != null && route.Connected)
                {
                    // recogemos los bytes del buffer a leer
                    int numBytesReceived = route.EndReceive(iar);
                    // variable global que almacena el numero de bytes totales recibidos
                    bytesReceived += (uint)numBytesReceived;
                    // marcamos la fecha/hora de la ultima
                    route.UpdateActivity();
                    // comprobamos el numero de bytes recibidos
                    if (numBytesReceived > 0)
                    {
                        route.BufferStream.Write(route.bBufferIn, 0, numBytesReceived);

                        // volvemos a leer
                        route.BeginReceive(route.bBufferIn, 0,
                            route.bBufferIn.Length,
                            new AsyncCallback(ContinuationReceiveCallback), route);
                    }
                    else
                    {
                        Node.LogAppendLine("the other end of the socket has closed gracefully {0}", route.RemoteEndPoint);
                        route.Failed = true;
                    }
                }
            }
            catch (BadProtocolException)
            {
                route.Failed = true;
                Node.LogAppendLine("Bad Protocol Exception");
            }
            catch (ObjectDisposedException err)
            {
                route.Failed = true;
                Node.LogAppendLine("EndReceive closing disposed socket {0} {1}", err.Message, err.ObjectName);
            }
            catch (SocketException err)
            {
                route.Failed = true;
                Node.LogAppendLine("Error en el socket: {0}\n{1}", err.ErrorCode, err.Message);
            }
            catch (Exception err)
            {
                route.Failed = true;
                Node.LogAppendLine(err);
            }
            finally
            {
                if (route.Failed)
                    localNode.RouteTable.RemoveRoute(route);
            }
        }

        /// <summary>
        /// informe de estadisiticas en protocolo http
        /// </summary>
        /// <param name="route"></param>
        protected void HttpMessageResponseState(Route route)
        {
            try
            {
                string content = string.Format("<html><p><h1>Nodo <b>{0}</b> de DodoNet</h1></p>",
                    localNode.localBind);

                content += string.Format("<p>Dia/Hora del informe {0}</p>", DateTime.Now);
                content += string.Format("<p>Dia/Hora de inicio del nodo {0}</p>", localNode.InitiatedNode);

                int per = (localNode.localEvents.CurInUseThreads * 100) / localNode.localEvents.CurActiveThreads;
                if (localNode.localEvents.CurActiveThreads == localNode.localEvents.CurInUseThreads)
                {
                    content += string.Format(
                        "<div style=\"border-width:medium; font-size: 24px; font-weight: bold; background-color:#FF0000; color:#FFFFFF\">Estado: {0} {1}% de uso Num. hebras: {2} de {3}</div>",
                        "Ocupado", per, localNode.localEvents.CurInUseThreads, localNode.localEvents.CurActiveThreads);
                }
                else if (per > 60 && per < 100)
                {
                    content += string.Format(
                        "<div style=\"border-width:medium; font-size: 24px; font-weight: bold; background-color:#FF6600; color:#FFFFFF\">Estado: {0} {1}%  de uso Num. hebras: {2} de {3}</div>",
                        "", per, localNode.localEvents.CurInUseThreads, localNode.localEvents.CurActiveThreads);
                }
                else
                {
                    content += string.Format(
                        "<div style=\"border-width:medium; font-size: 24px; font-weight: bold; background-color:#009900; color:#FFFFFF\">Estado: {0} {1}%  de uso Num. hebras: {2} de {3}</div>",
                        "", per, localNode.localEvents.CurInUseThreads, localNode.localEvents.CurActiveThreads);
                }

                content += "<link rel='stylesheet' href='css/2col_leftNav.css' type='text/css' />";

                // content += "<br><h2>Lista de discos duros del servidor:</h2>";

                // foreach (ComputerInfo.InfoDrive infoDrive in ComputerInfo.GetInfoDrives())
                // {
                //     long perDisc = (infoDrive.freeSpace * 100) / infoDrive.size;
                //     content += string.Format("<p>Disco: {0}, {1}, {2} Espacio libre: {3:N} Mb <b>[{4}]</b>%</p>", 
                //         infoDrive.unit,
                //         infoDrive.fileSystem, infoDrive.description, 
                //         infoDrive.freeSpace / 1000000, perDisc);
                // }

                content += string.Format("<h2>Rutas conectados: {0}</h2>",
                    localNode.RouteTable.Count);

                foreach (Route r in localNode.RouteTable.GetArray())
                {
                    content += string.Format(
                        "<p>** Extremo local: {0} Extremo remoto: {1} Ultima actividad: {2}</p>",
                        r.LocalEndPoint, r.RemoteEndPoint, r.LastActivity);
                }

                content += string.Format("<h2>Sesiones activas: {0}</h2>",
                    localNode.Sessions.Count);

                foreach (ISessionRequest v in localNode.Sessions.GetArray())
                {
                    SessionRequest ss = (SessionRequest)v;
                    content += string.Format(
                        "<p>** Sesion: {0} Extremo remoto: {1} Ultima actividad: {2}</p>",
                        ss.NodeBindApplicant.NodeId, ss.NodeBindApplicant.NodeAddress, ss.NodeBindRemote.NodeAddress);
                }

                #region Processes

                content += "<br><h2>Procesos activos:</h2>\r\n";

                Thread[] at = localNode.localEvents.Threads;
                foreach (Thread tt in at)
                {
                    try
                    {
                        if (tt != null)
                        {
                            try
                            {
                                tt.Suspend();

                                content += string.Format("<p><u>Hebra: {0}-{1}</u></p>",
                                    tt.ManagedThreadId,
                                    tt.Name);

                                StackTrace st = new StackTrace(tt, true);

                                int x = 0;
                                int y = 0;
                                for (x = 0; x < st.FrameCount; x++)
                                {
                                    y++;
                                    StackFrame sf = st.GetFrame(x);
                                    content += string.Format("<p>({0:000})\t {1} Line: {2} File:{3}</p>",
                                        y, sf.GetMethod().Name, sf.GetFileLineNumber(), sf.GetFileName());
                                }
                            }
                            catch { }
                        }
                    }
                    catch (Exception err)
                    {
                        content += string.Format("Error: {0}", err.Message);
                    }
                    finally
                    {
                        try
                        {
                            if (tt != null)
                                tt.Resume();
                        }
                        catch { }
                    }
                }

                #endregion

                content += "</html>";

                string msg =
                    string.Format(
                    "HTTP/1.1 200 OK\r\n" +
                    "Content-Type: text/html\r\n" +
                    "Content-Length: {0}\r\n" +
                    "Conection: Close\r\n" +
                    "\r\n{1}",
                    content.Length, content);

                byte[] tmp = Encoding.ASCII.GetBytes(msg);
                route.Send(tmp, 0, tmp.Length);
            }
            catch (Exception err)
            {
                try
                {
                    string content = string.Format(
                        "<html>Error: {0}</html>",
                        err.Message);
                    string msg =
                        string.Format(
                        "HTTP/1.1 200 OK\r\n" +
                        "Content-Type: text/html\r\n" +
                        "Content-Length: {0}\r\n" +
                        "Conection: Close\r\n" +
                        "\r\n{1}",
                        content.Length, content);

                    Node.LogAppendLine(err);

                    byte[] tmp = Encoding.ASCII.GetBytes(msg);
                    route.Send(tmp, 0, tmp.Length);
                }
                catch { }
            }
            finally
            {
                try
                {
                    localNode.RouteTable.RemoveRoute(route);
                }
                catch { }
            }
        }

        #endregion

        #region Listen

        public void StartListening()
        {
            // var ctrl para aceptar clientes
            listening = true;
            eventListening = new AutoResetEvent(false);
            ttListening = new Thread(new ThreadStart(BeginAcceptNonBlock));
            ttListening.Start();
        }

        /// <summary>
        /// para de aceptar clientes. Recuerda antes de llamar a esta instruccion 
        /// cerrar el socket
        /// </summary>
        private void StopListening()
        {
            if (listening)
            {
                listening = false;
                eventListening.Set();
                ttListening.Join();
                // eventListening.Close();
            }
        }

        /// <summary>
        /// hebra que empieza a escuchar
        /// </summary>
        public void BeginAcceptNonBlock()
        {
            try
            {
                while (listening)
                {
                    IAsyncResult ar = inRoute.BeginAccept(new AsyncCallback(EndAcceptCallbackNonBlock));
                    eventListening.WaitOne();
                }
            }
            catch (SocketException err)
            {
                Node.LogAppendLine("error grave no al recibir la petición de conexión r:{0} socket error:{1}",
                    err.Message, err.ErrorCode);
            }
            catch (Exception err)
            {
                Node.LogAppendLine("error grave no al recibir la petición de conexión r:{0}",
                    err.Message);
            }
        }

        /// <summary>
        /// procesar datos de entrada
        /// </summary>
        /// <param name="__iar"></param>
        public void EndAcceptCallbackNonBlock(IAsyncResult iar)
        {
            try
            {
                Route route = inRoute.EndAccept(localNode, iar);

                if (route != null)
                {
                    // agregamos ruta nueva
                    localNode.RouteTable.AddRoute(route);

                    if (localNode.RouteTable.Count < DodoConfig.MaxConnections - 1)
                    {
                        // recibir datos
                        BeginReceive(route);

                        Node.LogAppendLine(
                            "nueva entrante ruta r:{0}, l:{1}",
                            route.RemoteEndPoint, route.LocalEndPoint);
                    }
                    else
                    {
                        Node.LogAppendLine(
                            "¡¡¡Demasiadas rutas!!! última solicitud:{0} total:{1}",
                            route.RemoteEndPoint, localNode.RouteTable.Count);

                        localNode.RouteTable.RemoveRoute(route);
                    }
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine("error grave no al recibir la petición de conexión r:{0}",
                    err.Message);
            }
            finally
            {
                if (eventListening != null) eventListening.Set();
            }
        } 

        #endregion

        #region Miembros de IDisposable

        public void Dispose()
        {
            if (!disposed)
            {
                inRoute.Dispose();
                StopListening();
                disposed = true;
            }
        }

        #endregion
    }
}
