using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet;
using DodoNet.Http;

namespace DodoOpenId.OpenId
{
    public class DiscoverRequest
    {
        public static readonly string Google_Discover_OpenID_Endpoint = "https://www.google.com/accounts/o8/id";

        public XrdsDocument XrdsDocument { get; private set; }

        public XrdElement MainElement { get; private set; }

        public Uri ProviderEndpoint 
        {
            get
            {
                return MainElement.ServiceUris.FirstOrDefault().Uri;
            }
        }

        public DiscoverRequest() { }

        public void Request(Node localNode, string url)
        {
            var req = new HttpRequest(localNode);
            req.Accept.Add("application/xrds+xml");
            req.Url = url;

            using (var reply = localNode.SendRequest(req))
            {
                XrdsDocument = new XrdsDocument(reply.Body.GetPlainText());
                MainElement = XrdsDocument.XrdElements.FirstOrDefault();

                if (MainElement == null)
                    throw new NullReferenceException("Documento XRDS sin elementos");
            }
        }
    }
}
