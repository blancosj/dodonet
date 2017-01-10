using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet;
using DodoNet.Http;
using DodoNet.Web;

namespace DodoOpenId.OpenId
{
    public class OpenIdApp : App, IDisposable
    {
        public Node AppNode { get; private set; }

        object sync = new object();

        public object Sync
        {
            get { return sync; }
            set { sync = value; }
        }

        public Dictionary<string, KeyAssociation> CacheAssociations { get; private set; }

        public void InitApp(Node appNode)
        {
            AppNode = appNode;

            CacheAssociations = new Dictionary<string, KeyAssociation>();

            AppNode.ScheduleContinuation(
                new CheckerAssociations(this, 30, 5000));
        }

        public bool RequestAndCheck()
        {
            bool ret = false;

            HttpContext context = WebApplication.CurrentContext;
            HttpRequest request = context.Request;

            string invalidateHandle = request.UriArgs["openid.invalidate_handle"];

            if (invalidateHandle == null)
            {
                var discover = new DiscoverRequest();
                discover.Request(context.OverlayNode, DiscoverRequest.Google_Discover_OpenID_Endpoint);

                var auth = new WebAuth();

                var ub = new UriBuilder(context.Request.Uri);

                lock (sync)
                {
                    if (!CacheAssociations.ContainsKey(auth.ToStringHandle()))
                        CacheAssociations.Add(auth.ToStringHandle(), 
                            new KeyAssociation(auth.ToStringHandle()));
                }

                var realm = ub.Uri.ToString();
                var authReq = new AuthenticationRequest(discover, realm,
                    auth.ToStringHandle(), realm, AppNode);

                context.Reply.RedirectTo(authReq.UrlRedirectTo());
            }
            else
            {
                lock (sync)
                {
                    ret = CacheAssociations.ContainsKey(invalidateHandle);
                    if (ret)
                        CacheAssociations.Remove(invalidateHandle);
                }
            }

            return ret;
        }

        public override void NewNeighbour(NodeBind nodeBind) { }

        public override void NeighbourRemoved(NodeBind nodeBind, bool failure) { }

        public override void MessageReceived(DodoNet.Overlay.OverlayMessage msg) { }

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
