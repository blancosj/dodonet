using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace DodoNet.Utility.Dns
{
    public class CacheMXRecords : IDisposable
    {
        public static IPAddress s_dnsServer01 = IPAddress.Parse("208.67.222.222");
        public static IPAddress s_dnsServer02 = IPAddress.Parse("208.67.220.220");

        Dictionary<string, MXRecord[]> cache;
        Dictionary<string, SmtpClient> smtpClients;
        object sync = new object();

        public CacheMXRecords()
        {
            cache = new Dictionary<string, MXRecord[]>();
            smtpClients = new Dictionary<string, SmtpClient>();
        }

        public MXRecord[] ResolveDomain(string domain)
        {
            lock (sync)
            {
                MXRecord[] ret = new MXRecord[0];

                if (cache.ContainsKey(domain))
                {
                    ret = cache[domain];
                }
                else
                {
                    ret = Resolver.MXLookup(domain, s_dnsServer01);
                    cache.Add(domain, ret);
                }
                return ret;
            }
        }

        public SmtpClient GetSmtpClient(string domain)
        {
            lock (sync)
            {
                SmtpClient ret = new SmtpClient();

                foreach (MXRecord mxr in ResolveDomain(domain))
                {
                    if (smtpClients.ContainsKey(domain))
                    {
                        ret = smtpClients[domain];
                    }
                    else
                    {
                        ret = new SmtpClient(mxr.DomainName);
                        smtpClients.Add(domain, ret);
                    }                    
                    break;
                }

                return ret;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (sync)
            {
                cache.Clear();
            }
        }

        #endregion
    }
}
