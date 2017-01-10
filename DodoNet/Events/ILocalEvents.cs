using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DodoNet
{
    public interface ILocalEvents : IDisposable
    {
        int CurInUseThreads { get; }
        int CurActiveThreads { get; }
        int CurQueuedWorks { get; }

        Thread[] Threads { get; }

        /// <summary>
        /// Insert the Message onto the event queue.
        /// </summary>
        void RaiseEvent(long ms, Message Message);

        /// <summary>
        /// Remove the Message in event queue
        /// </summary>
        void RemoveEvent(Message message);

        /// <summary>
        /// The current time, according to the event queue.
        /// </summary>
        DateTime GetTime();

        void Activate(Node localNode);
    }
}
