using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Overlay
{
    [Serializable]
    public abstract class OverlayMessage
    {
        /// <summary>
        /// Called when a message has arrived at its destination.
        /// Must be overriden in derived classes.
        /// </summary>
        public abstract void ArrivedAtDestination(Node overlayNode, Message m);
    }
}
