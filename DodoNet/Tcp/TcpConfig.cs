using System;
using System.Net.Sockets;

namespace DodoNet.Tcp
{
    /// <summary>
    /// TcpConfig
    /// </summary>
    public class TcpConfig
    {
        public static int ConnectionTimeout = 30000;
        public static int SizeReceivedBuffer = 1048576;
        public static int SizeSendBuffer = 1048576;
        public static int SendTimeout = 5000;
        public static int RequestTimeout = 5000;
        public static int ReceiveTimeout = 5000;
        public static int UseKeepAlive = 0;
        public static int Backlog = 10;
        public static bool Blocking = true;

        public static void ConfigSocket(Socket s)
        {
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, TcpConfig.SizeReceivedBuffer);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, TcpConfig.SizeSendBuffer);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, TcpConfig.UseKeepAlive);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, TcpConfig.ReceiveTimeout);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, TcpConfig.SendTimeout);
            s.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, false);

            /*
                This is a temporary condition and later calls to the same routine may 
                complete normally. The socket is marked as non-blocking (non-blocking operation mode), 
                and the requested operation is not complete at this time. 
            */
            s.Blocking = TcpConfig.Blocking;
        }
    }
}
