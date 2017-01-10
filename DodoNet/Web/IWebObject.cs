using System;
using System.Collections.Generic;
using System.Text;

using DodoNet.Overlay;
using DodoNet.Http;

namespace DodoNet.Web
{
    public interface IWebObject
    {
        string Name { get; }
        string Key { get; }
        string Render();
        void GetReply(Node localNode, HttpContext context);
    }
}
