using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Overlay
{
    /// <summary>
    /// This class is an abstract continuation: a piece of code that is executed later by the
    /// event-loop. Users should create a subclass that overrides the Execute method.
    /// </summary>
    public abstract class Continuation : OverlayMessage
    {
        /// <summary>
        /// This method is called when the continuation fires.
        /// </summary>
        public abstract void Execute(Node localNode);

        public override void ArrivedAtDestination(Node localNode, Message m)
        {
            this.Execute(localNode);
        }
    }
}
