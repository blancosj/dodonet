using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Overlay
{
    public abstract class Timer : Continuation
    {
        long initialDelayMs;        // Initial delay
        long nextDelayMs;           // Delay for next execution of timer

        bool periodic;
        long periodMs;              // Steady-state period

        /// <summary>
        /// Constructor for one-shot timers
        /// </summary>
        /// <param name="delayMs"></param>
        public Timer(long delayMs)
        {
            this.initialDelayMs = delayMs;
            this.nextDelayMs = delayMs;
            this.periodic = false;
        }

        /// <summary>
        /// Constructor for periodic timers
        /// </summary>
        /// <param name="initialDelayMs"></param>
        /// <param name="periodMs"></param>
        public Timer(long initialDelayMs, long periodMs)
        {
            this.initialDelayMs = initialDelayMs;
            this.nextDelayMs = initialDelayMs;
            this.periodMs = periodMs;
            this.periodic = true;
        }

        /// <summary>
        /// The length of time to wait before next executing the timer.
        /// </summary>
        public virtual long NextDelayMs
        {
            get { return nextDelayMs; }
        }

        /// <summary>
        /// To make a non-periodic timer into a periodic one, set Periodic to true (before or during
        /// the timer's execution).
        /// 
        /// To cancel a periodic or non-periodic timer before the timer's execution, use Node.CancelTimer().
        /// 
        /// To cancel a periodic timer during the timer's execution (i.e., to make it non-periodic)
        /// set Periodic to false.
        /// </summary>
        public bool Periodic
        {
            get { return periodic; }
            set { periodic = value; }
        }

        public override void ArrivedAtDestination(Node localNode, Message m)
        {
            this.Execute(localNode);
            if (this.periodic)
            {
                this.nextDelayMs = this.periodMs;
                localNode.SetTimer(this);
            }
        }
    }
}
