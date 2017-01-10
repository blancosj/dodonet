using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Amib.Threading;

namespace DodoNet
{
    public class LocalEvents : ILocalEvents
    {
        const int TicksPerMillisecond = 10000;

        private MessageQueue queue;
        private SmartThreadPool m_STP;
        private Node localNode;

        public long numThrowedEvents;
        public long numHandledEvents;

        public int CurInUseThreads { get { return m_STP.InUseThreads; } }
        public int CurActiveThreads { get { return m_STP.ActiveThreads; } }
        public int CurQueuedWorks { get { return m_STP.InUseThreads; } }

        Thread overlayThread;

        public Thread[] Threads { get { return m_STP.WorkerThreads; } }

        volatile bool exit = false;

        public LocalEvents(int minThreads, int maxThreads)
        {
            queue = new MessageQueue();

            // Create the thread pool
            STPStartInfo stbStartInfo = new STPStartInfo();
            stbStartInfo.MinWorkerThreads = minThreads;
            stbStartInfo.MaxWorkerThreads = maxThreads;

            m_STP = new SmartThreadPool(stbStartInfo);
        }

        public void RaiseEvent(long ms, Message message)
        {
            QueueEntry p = new QueueEntry();
            p.message = message;
            p.fireAt = DateTime.Now.Add(new TimeSpan(TicksPerMillisecond * ms));
            queue.Add(p);

            numThrowedEvents++;
        }

        public void Activate(Node localNode)
        {
            this.localNode = localNode;

            overlayThread = new Thread(new ThreadStart(this.EventLoop));
            overlayThread.Start();
        }

        public DateTime GetTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// This is the main method of the Overlay thread. It reads messages from the
        /// MessageQueue and processes them.
        /// </summary>
        public void EventLoop()
        {
            try
            {
                while (!exit)
                {
                    QueueEntry p = queue.Remove();
                    if (p != null)
                    {
                        ThreadLocalEvent tle = new ThreadLocalEvent();
                        p.message.threadLocalEvent = tle;

                        IWorkItemResult result = m_STP.QueueWorkItem(
                            new WorkItemCallback(RaiseEventByThreadPool), p);

                        tle.WorkItemResult = result;
                    }
                }
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// método llamado por la hebra correspondiente del pool
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public object RaiseEventByThreadPool(object state)
        {
            QueueEntry p = null;
            try
            {
                numHandledEvents++;

                p = state as QueueEntry;

                if (p != null)
                {
                    ThreadLocalEvent tle = (ThreadLocalEvent)p.message.threadLocalEvent;
                    tle.SetCurrentThread();
                    localNode.ResumeRouting(p.message);
                }
            }
            catch (OutOfMemoryException) { }
            catch (ThreadAbortException) { }
            catch (Exception) { }
            finally
            {
                try
                {
                    if (p != null)
                    {
                        p.message.threadLocalEvent.IsCompleted = true;
                        p.message = null;
                    }
                }
                catch { }
            }
            return null;
        }

        public int GetQueueLength()
        {
            return queue.GetQueueLength();
        }

        /// <summary>
        /// quitar un elemento
        /// </summary>
        /// <param name="message"></param>
        public void RemoveEvent(Message message)
        {
            queue.Remove(message);
        }

        public void Dispose()
        {
            exit = true;

            m_STP.Shutdown(true, 1000);
            m_STP.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            queue.Shutdown();
        }
    }
}
