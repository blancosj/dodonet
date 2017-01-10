using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Amib.Threading;

namespace DodoNet
{
    public class ThreadLocalEvent : IThreadLocalEvent
    {
        private object syncRoot;
        public object SyncRoot { get { return syncRoot; } }

        private IWorkItemResult workItemResult;
        public IWorkItemResult WorkItemResult
        {
            get { return workItemResult; }
            set { workItemResult = value; }
        }

        private Thread thread;

        private bool isCompleted = false;
        public bool IsCompleted
        {
            get { return isCompleted; }
            set
            {
                // sincronizamos este flag
                lock (syncRoot)
                {
                    isCompleted = value;
                }
            }
        }

        public ThreadLocalEvent()
        {
            isCompleted = false;
            syncRoot = new object();
        }

        public void Cancel()
        {
            try
            {
                // sincronizamos este flag
                lock (syncRoot)
                {
                    if (thread != null && !isCompleted)
                    {
                         workItemResult.Cancel();
                    }
                }
            }
            catch { }
        }

        public void SetCurrentThread()
        {
            thread = Thread.CurrentThread;
        }

        #region IDisposable Members

        public void Dispose()
        {
            thread = null;
        }

        #endregion
    }
}
