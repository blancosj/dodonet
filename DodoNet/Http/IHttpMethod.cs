using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoNet.Http
{
    public interface IHttpMethod
    {
        bool MustProcess();

        void Process(WebApplication app);
    }
}
