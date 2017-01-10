using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace DodoNet
{
    public class QueueEntry
    {
        public Message message;
        public DateTime fireAt;
    }

    public class MessageQueue
    {
        // A list of ArrayList objects
        SortedList<DateTime, List<QueueEntry>> list = new SortedList<DateTime, List<QueueEntry>>();
        Dictionary<Message, QueueEntry> table = new Dictionary<Message, QueueEntry>();
        bool killed = false;

        // Add the message to the queue
        public void Add(QueueEntry p)
        {
            lock (this)
            {
                if (killed) return;

                // Add message to the list, sorted by delivery time
                List<QueueEntry> listEntry = null;
                if (!list.TryGetValue(p.fireAt, out listEntry))
                {
                    listEntry = new List<QueueEntry>();
                    list.Add(p.fireAt, listEntry);
                }
                // insertamos elemento en sub cola
                listEntry.Add(p);

                // Add the message to the table accessed by key
                Debug.Assert(!table.ContainsKey(p.message));
                table.Add(p.message, p);

                Monitor.Pulse(this);
            }
        }

        // Wait for the next event on the queue to be ready
        // Returns null if the queue was killed
        public QueueEntry Remove()
        {
            TimeSpan tsInfinite = new TimeSpan(0, 0, 0, 0, Timeout.Infinite);     // Infinite timeout            
            TimeSpan ts = tsInfinite;

            // Extract the next Message to process
            lock (this)
            {
                for (; ; )
                {
                    if (killed) return null;

                    if (list.Count > 0)
                    {
                        List<QueueEntry> listEntry = list[list.Keys[0]];

                        // We got a message. Find out when it should be processed
                        DateTime now = DateTime.Now;
                        ts = list.Keys[0].Subtract(DateTime.Now);

                        // Could probably use DateTime.CompareTo() here
                        if (ts.Ticks < 1)
                        {
                            QueueEntry p = listEntry[0];

                            // It is time to process the message
                            // Remove the entry from the list
                            listEntry.RemoveAt(0);
                            if (listEntry.Count == 0)
                                list.Remove(list.Keys[0]);

                            // Remove the entry from the table
                            Debug.Assert(table.ContainsKey(p.message));
                            table.Remove(p.message);

                            return p;
                        }

                        // Message is not ready to be processed.
                        // Wait for amount of time specified by 'ts'
                    }
                    else
                    {
                        // No messages in the queue. Wait until we are signalled.
                        ts = tsInfinite;
                    }

                    // Wait until we are signalled or the timeout expires.
                    Monitor.Wait(this, ts);
                }
            }
        }

        /// <summary>
        /// Remove an event from the event queue.
        /// </summary>
        /// <param name="Message">A Message acting as a "comparison key" that is equal to the
        /// message to be removed.</param>
        public QueueEntry Remove(Message msg)
        {
            lock (this)
            {
                if (killed) return null;

                QueueEntry p = null;
                if (table.TryGetValue(msg, out p))
                {
                    // Remove the event
                    List<QueueEntry> listEntry = list[p.fireAt];
                    listEntry.Remove(p);
                    if (listEntry.Count == 0)
                        list.Remove(p.fireAt);

                    table.Remove(msg);
                }

                // Return the event
                return p;
            }
        }

        /// <summary>
        /// Get the number of messages in the queue that are currently ready to be processed
        /// </summary>
        /// <returns></returns>
        public int GetQueueLength()
        {
            // Extract the next Message to process
            lock (this)
            {
                int ret = 0;

                foreach (KeyValuePair<DateTime, List<QueueEntry>> pair in list)
                {
                    if (killed) return 0;

                    // We got a message. Find out when it should be processed
                    DateTime now = DateTime.Now;
                    TimeSpan ts = pair.Key.Subtract(DateTime.Now);
                    // Could probably use DateTime.CompareTo() here
                    if (ts.Ticks < 0)
                    {
                        // Message _is_ ready to be processed
                        ret += pair.Value.Count;
                    }
                    else if (ts.Ticks > 0)
                    {
                        // Message is still _not_ ready to be processed
                        break;
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// Kill the queue immediately
        /// </summary>
        public void Shutdown()
        {
            lock (this)
            {
                killed = true;
                Monitor.Pulse(this);
            }
        }
    }
}
