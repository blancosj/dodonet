using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet
{
    public delegate void RouteContCallback(Node localNode, Message msg, Route route, bool success, Exception exception);
    public delegate void SendCtrlCallback(Route route, bool success, Exception exception);
}
