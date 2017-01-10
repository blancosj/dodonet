using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DodoNet.Overlay
{
    public class RequestAsyncResult : IAsyncResult, IDisposable
    {
        private object state;
        private ManualResetEvent asyncWaitHandle;
        
        public bool isCompleted;
        public bool success;
        public Exception exception;

        AsyncCallback callback;
        public AsyncCallback Callback { get { return callback; } }

        public OverlayRequest Request { get; set; }
        public OverlayReply Reply { get; set; }

        #region Constructor

        public RequestAsyncResult(OverlayRequest request, AsyncCallback callback, object state)
        {
            asyncWaitHandle = new ManualResetEvent(false);
            this.Request = request;
            this.callback = callback;
            this.state = state;
        }

        #endregion

        #region Miembros de IAsyncResult

        public object AsyncState { get { return state; } }

        public bool CompletedSynchronously { get { return false; } }

        public WaitHandle AsyncWaitHandle { get { return asyncWaitHandle; } }

        public bool IsCompleted { get { return isCompleted; } }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            asyncWaitHandle.Close();
            asyncWaitHandle = null;
        }

        #endregion
    }
}
