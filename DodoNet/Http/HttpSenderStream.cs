using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;
using DodoNet.Compression.GZip;

namespace DodoNet.Http
{
    class HttpSenderStream : Stream, IDisposable
    {
        IHttpMessage message;

        long position;

        public HttpSenderStream()
        {
        }

        public HttpSenderStream(IHttpMessage message)
            : this()
        {
            this.message = message;
            this.message.PrepareSend();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { return message.HeadStream.Length + message.Body.Length; }
        }

        public override long Position
        {
            get
            {
                return position;
            }

            set
            {
                if (position < message.HeadStream.Length)
                {
                    message.HeadStream.Position = position;
                    if (message.Body != null)
                        message.Body.Position = 0;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long previousPosition = position;
            int total = count - offset;

            if (position < message.HeadStream.Length)
            {
                int read = ((int)(message.HeadStream.Length - position));

                if (total < read)
                {
                    read = total;
                }

                int hr = message.HeadStream.Read(buffer, offset, read);
                offset += hr;
                total -= hr;
                position += hr;
            }

            if (offset < count && message.Body != null && message.Body.HasContent)
            {
                int br = message.Body.ContentStream.Read(buffer, offset, total);
                position += br;
            }

            return ((int)(position - previousPosition));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion
    }
}
