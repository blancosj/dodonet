using System;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Collections;
using DodoNet.Overlay;

namespace DodoNet.Http
{
    public interface IHttpMessage
    {
        MessageTypes Method { get; set; }
        string Version { get; set; }
        string Url { get; set; }

        HttpHead Head { get; set; }
        Stream HeadStream { get; set; }
        void PrepareSend();

        HttpBody Body { get; set; }
        bool IsCompleted { get; set; }
        int ContentLength { get; set; }
        string ContentType { get; set; }

        void AddField(HttpField field);

        NameValueCollection Form { get; set; }
        NameValueCollection<FileAttached> Files { get; set; }

        TransferEncodings TransferEncoding { get; set; }

        Encoding ContentTransferEncoding { get; set; }
        string GetBoundaryStart();
        string GetBoundaryEnd();
        HttpBody CreateBody();
        List<HttpBody> BodyParts { get; set; }
    }
}
