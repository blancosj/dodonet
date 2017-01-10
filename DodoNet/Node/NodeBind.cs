using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet
{
    public class NodeBind
    {
        public NodeAddress NodeAddress { get; set; }
        public NodeId NodeId { get; set; }

        public static implicit operator NodeBind(string value)
        {
            Uri tmp = new Uri(value);

            NodeId nodeId = null;
            NodeAddress nodeAddress = null;

            if (!string.IsNullOrEmpty(tmp.UserInfo))
                nodeId = tmp.UserInfo;

            if (!string.IsNullOrEmpty(tmp.Host))
                nodeAddress = (new UriBuilder(tmp.Scheme, tmp.Host, tmp.Port)).Uri.ToString();

            return new NodeBind(nodeId, nodeAddress);
        }

        public NodeBind(string url)
        {
            var uri = new Uri(url);

            if (uri.Scheme == Uri.UriSchemeHttp ||
                uri.Scheme == Uri.UriSchemeHttps)
            {
                this.NodeId = new NodeId(uri.Host);
                this.NodeAddress = new NodeAddress(url);
            }
            else
                throw new Exception("Error url");
        }

        public NodeBind(NodeId id, NodeAddress address)
        {            
            this.NodeAddress = address;
            this.NodeId = id;
        }

        public NodeBind Clone()
        {
            return (NodeBind)this.MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            bool ret = false;

            NodeBind tmp = obj as NodeBind;
            if (tmp != null)
            {
                ret = NodeAddress == tmp.NodeAddress;
                ret |= NodeId == tmp.NodeId;
            }

            return ret;
        }

        public override int GetHashCode()
        {
            int ret = 0;
            ret += NodeId == null ? 0 : NodeId.GetHashCode();
            ret += NodeAddress == null ? 0 : NodeAddress.GetHashCode();
            return ret;
        }

        public override string ToString()
        {
            string ret = string.Empty;

            UriBuilder ub = new UriBuilder();

            if (NodeAddress != null)
            {
                ub.Scheme = NodeAddress.Uri.Scheme;
                ub.Host = NodeAddress.Uri.Host;
                ub.Port = NodeAddress.Uri.Port;
            }

            if (NodeId != null)
                ub.UserName = NodeId.Id;

            return ub.ToString();
        }
    }
}
