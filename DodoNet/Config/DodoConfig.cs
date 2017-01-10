using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet
{
    /// <summary>
    /// Descripción breve de DodoConfig.
    /// </summary>
    public class DodoConfig
    {
        public static int IPHeaderOverhead = 42 + 12;
        public static string Protocol = "acp";
        public static int Port = 80;
        public static string SecuredProtocol = "acps";
        public static int SecuredPort = 443;
        public static short DefaultProtocol = Protocols.Prot_FV_Compress;
        public static int SizeHeader = 8;
        public static int SizeHeaderReply = 14;
        // public static int SizePacket = 768;
        public static int SizePacket = 8192;
        public static int DefaultReplyTimeout = 50000;
        public static long KeepAliveTimeout = 15000;
        public static long TimerPeriod = 9000;
        public static int NumMaxPacketsInQueue = 500;
        public static int SecsLapsedForReconnect = 30;
        public static int bandwidthMax = 1048576;
        public static int msecsMaxInactiveFrames = 60000;
        public static int SecsSocketNoActivity = 36000;
        public static int MaxConnections = 450;

        public static Int32 iLocalEventMinThreadsInPool = 5;
        public static Int32 iLocalEventMaxThreadsInPool = 30;
    }
}
