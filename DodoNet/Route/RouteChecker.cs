using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet
{
    /// <summary>
    /// checker all connected routes
    /// </summary>
    public class RouteCheck : Timer
    {
        public RouteCheck(long initialDelayMs, long periodMs)
            : base(initialDelayMs, periodMs)
        {
        }

        public override void Execute(Node localNode)
        {
            foreach (Route route in localNode.RouteTable.GetArray())
            {
                TimeSpan diff = DateTime.Now.Subtract(route.LastActivity);

                if (diff.TotalSeconds > DodoConfig.SecsSocketNoActivity)
                {
                    localNode.RouteTable.RemoveRoute(route);
                }
            }
        }
    }
}
