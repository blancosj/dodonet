using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet;
using DodoNet.Overlay;

namespace DodoOpenId.OpenId
{
    public class CheckerAssociations : Timer
    {
        public OpenIdApp App { get; private set; }

        public CheckerAssociations(OpenIdApp app, long initialDelayMs, long periodMs)
            : base(initialDelayMs, periodMs)
        {
            this.App = app;
        }

        public override void Execute(Node localNode)
        {
            lock (App.Sync)
            {
                foreach (var item in App.CacheAssociations)
                {
                    if (item.Value.IsExpired())
                        App.CacheAssociations.Remove(item.Key);
                }
            }
        }
    }
}
