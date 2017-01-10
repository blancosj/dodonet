using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace DodoNet
{
    public class NodeAddress
    {
        public Uri Uri { get; private set; }

        private IPEndPoint _ipEndPoint;
        public IPEndPoint IPEndPoint 
        {
            get
            {
                return _ipEndPoint;
            }

            set
            {
                _ipEndPoint = value;

                var ub = new UriBuilder(
                    Uri == null ? Uri.UriSchemeHttp : Uri.Scheme,
                    value.Address.ToString(),
                    value.Port);

                Uri = ub.Uri;
            }
        }
        public bool IsSecureLayer { get { return Uri.Scheme == Uri.UriSchemeHttps; } }   
        
        public X509Certificate2 Certificate { get { return null; } }     

        public static implicit operator NodeAddress(string value)
        {
            return new NodeAddress(value);
        }

        public static bool operator == (NodeAddress n1, NodeAddress n2)
        {
            return n1.Equals(n2);
        }

        public static bool operator !=(NodeAddress n1, NodeAddress n2)
        {
            return (!(n1==n2));
        }

        public NodeAddress()
        {
            IPEndPoint = new IPEndPoint(IPAddress.Any,
                IPEndPoint.MinPort);
        }

        public NodeAddress(IPEndPoint ipEndPoint)
        {
            this.IPEndPoint = ipEndPoint;
        }

        public NodeAddress(string address)
        {
            Uri u = new Uri(address);

            IPAddress ip = null;

            if (u.HostNameType == UriHostNameType.Dns)
            {
                var ips = Dns.GetHostAddresses(u.Host);
                ip = ips.FirstOrDefault();                
            }                
            else
                IPAddress.TryParse(u.Host, out ip);

            IPEndPoint ipEndPoint = null;
            if (ip != null)
                ipEndPoint = new IPEndPoint(ip,
                    u.Port < IPEndPoint.MinPort ? IPEndPoint.MinPort : u.Port);

            this.IPEndPoint = ipEndPoint;

            var ub = new UriBuilder(u.Scheme,
                ipEndPoint.Address.ToString(),
                ipEndPoint.Port);

            Uri = ub.Uri;
        }

        public Uri GetHtmlAddress()
        {
            UriBuilder ret = new UriBuilder();
            if (IsSecureLayer)
                ret.Scheme = Uri.UriSchemeHttps;
            else
                ret.Scheme = Uri.UriSchemeHttp;

            ret.Host = IPEndPoint.Address.ToString();
            ret.Port = IPEndPoint.Port;
            return ret.Uri;
        }

        /// <summary>
        /// devuelve una direccion acp que conserva solo puerto
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <returns></returns>
        public NodeAddress SubstIP(IPAddress newIP)
        {
            NodeAddress ret = new NodeAddress(
                new IPEndPoint(newIP, IPEndPoint.Port));
            return ret;
        }

        public override bool Equals(object obj)
        {
            bool ret = false;

            if (obj != null)
            {
                NodeAddress tmp = obj as NodeAddress;
                ret = IPEndPoint.Equals(tmp.IPEndPoint);
            }

            return ret;
        }

        public override int GetHashCode()
        {
            return this.IPEndPoint.GetHashCode();
        }

        public override string ToString()
        {
            return Uri.ToString();
        }
    }
}
