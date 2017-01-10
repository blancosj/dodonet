using System;
using System.Collections.Generic;
using System.Text;

namespace DodoNet.Web
{
    public class WebConfig
    {
        public static string HttpEncodingDefault = Encoding.Default.BodyName;
        public static string[] DefaultFiles = new string[] { "index.aspx", "default.aspx", "index.aspx", "default.htm" };
        public static int SessionIdLength = 42;
        public static int SessionIdSaltLength = 46;
        public static string SessionIdAnonymous = "anonymous";
        public static int DefaultTimeOutSessionInactiveInSecs = 600;        
        public static string ServerVersion = string.Format("DodoNet/{0}", System.EnvironmentInfo.LibInfo.GetVersionThisModule());
        public static byte[] ReturnLine = new byte[]{13, 10};
        public static int BufferSize = 4096;
    }
}
