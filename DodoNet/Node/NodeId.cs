namespace DodoNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DodoNet.Tools;

    public class NodeId
    {
        string id;
        public string Id { get { return id; } }

        #region Operators

        public static implicit operator NodeId(string value)
        {
            return new NodeId(value);
        }
                
        public static bool operator == (NodeId n1, NodeId n2)
        {
            bool ret = false;
            try
            {
                ret = n1.id.Equals(n2.id);
            }
            catch (NullReferenceException) { }
            return ret;
        }

        public static bool operator != (NodeId n1, NodeId n2)
        {
            bool ret = false;
            try
            {
                var a = (object)n1 == null ? null : n1.id;
                var b = (object)n2 == null ? null : n2.id;
                ret = a != b;
            }
            catch { }
            return ret;
        }

        #endregion

        public NodeId()
        {
            id = GenerateId();
        }

        public NodeId(string id)
        {
            this.id = id.ToLower();
        }

        private string GenerateId()
        {
            string ret = string.Empty;

            var ran = new Random(0);
            var ranNum = ran.Next();
            var ranCompound = ranNum + DateTime.Now.Ticks;
            ret = Conversion.Num2Don(ranCompound);

            return ret;
        }

        public override bool Equals(object obj)
        {
            bool ret = false;

            try
            {
                ret = id.Equals(((NodeId)obj).id);
            }
            catch
            {
            }

            return ret;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override string ToString()
        {
            return id;
        }
    }
}
