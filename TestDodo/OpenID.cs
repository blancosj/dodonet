namespace TestDodo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DodoNet;
    using DodoNet.Http;
    using DodoNet.Overlay;

    using DodoOpenId.OpenId;

    public class OpenID
    {
        Node node = new Node();

        public OpenID()
        {
            node.Activate(new LocalEvents(1, 1));
            
            var discover = new DiscoverRequest();
            discover.Request(node, DiscoverRequest.Google_Discover_OpenID_Endpoint);

            string realm = "http://www.poweq.com";
            var auth = new AuthenticationRequest(discover, realm, "12345678", realm, node);

            auth.PrepareSend();

            /*
            var reply = node.SendRequest(auth.Request);

            Console.WriteLine("Begin.");
            Console.Write(reply.Body.GetPlainText());
            Console.WriteLine("End.");
            */

            node.BeginSendRequest(auth.Request,
                delegate(IAsyncResult result)
                {
                    node.EndSendRequest(result);

                    var reply = (HttpReply)((RequestAsyncResult)result).Reply;

                    Console.WriteLine("Begin.");
                    Console.Write(reply.Body.GetPlainText());
                    Console.WriteLine("End.");

                }, null);

            return;
            /*
            var req = new HttpRequest(node);
            req.Accept.Add("application/xrds+xml");
            req.Url = "https://www.google.com/accounts/o8/id";

            int x = 1;
            for (x = 0; x < 1; x++)
            {
                // node.BeginSendRequest(req, cb, x);

                try
                {
                    var reply = node.SendRequest(req);

                    Console.Write("{0}\r\n{1}", x,
                        reply.Body.GetPlainText());

                    var xrds = new XrdsDocument(reply.Body.GetPlainText());

                    var req2 = new HttpRequest(node);
                    req2.Accept.Add("application/xrds+xml");
                    req2.Url = "https://www.google.com/accounts/o8/id";

                    ServiceElement ser = null;

                    foreach (var i in xrds.XrdElements)
                    {
                        foreach (ServiceElement s in i.Services)
                        {
                            ser = s;
                            break;
                        }
                    }

                    req = new HttpRequest(node);
                    req.Url = ser.UriElements.FirstOrDefault().Uri.ToString();

                    var reqLogin = new AuthenticationRequest(req, xrds);

                    reqLogin.Ns = "http://specs.openid.net/auth/2.0";
                    reqLogin.ClaimedId = "http://specs.openid.net/auth/2.0/identifier_select";
                    reqLogin.Indentity = "http://specs.openid.net/auth/2.0/identifier_select";
                    reqLogin.ReturnTo = "http://wwww.poweq.com";
                    reqLogin.Realm = "http://wwww.poweq.com";
                    reqLogin.Mode = "checkid_setup";
                    reqLogin.AssocHandle = "ABSmpf6DNMw";
                    reqLogin.PrepareSend();

                    reply = node.SendRequest(reqLogin.Request);
                   
                }
                catch (Exception err)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(err.Message);
                    Console.ResetColor();
                }
                
            }
            */
            Console.Read();
        }

        public void cb(IAsyncResult ar)
        {
            try
            {
                node.EndSendRequest(ar);

                var r = (RequestAsyncResult)ar;

                Console.Write("{0}\r\n{1}", ar.AsyncState,
                    ((HttpReply)r.Reply).Body.GetPlainText());
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(err.Message);
                Console.ResetColor();
            }
        }
    }
}
