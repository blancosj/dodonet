using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;

namespace DodoNet.Web
{
    public interface IWebSession : IDisposable
    {
        int TimeOut { get; set; }
        WebAuth Auth { get; }
        DateTime Started { get; }
        Dictionary<string, object> Vars { get; set; }
        WebApplication App { get; set; }
        void AddHttpObject(HttpContext context, IWebObject obj);

        bool IsValid();
        void UpdateActivity();
        double GetInactivityMs();
    }
}
