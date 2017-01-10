using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Http
{
    public class HttpConst
    {
        // first field of head
        public const string HttpVersion = "HTTP/1.1";
        public const string HttpOldVersion = "HTTP/1.0";

        // field names
        public const string ContentDisposition = "Content-Disposition";
        public const string Host = "Host";
        public const string Accept = "Accept";
        public const string AcceptLanguage = "Accept-Language";
        public const string AcceptEncoding = "Accept-Encoding";
        public const string AcceptCharset = "Accept-Charset";
        public const string KeepAlive = "Keep-Alive";
        public const string Connection = "Connection";
        public const string UserAgent = "User-Agent";
        public const string Referer = "Referer";
        public const string Cookie = "Cookie";
        public const string ContentType = "Content-Type";
        public const string IfUnmodifiedSince = "If-Unmodified-Since";
        public const string IfModifiedSince = "If-Modified-Since";
        public const string IfNoneMatch = "If-None-Match";
        public const string ContentLength = "Content-Length";
        public const string TransferEncoding = "Transfer-Encoding";
        public const string ContentTransferEncoding = "Content-Transfer-Encoding";
        public const string Authorization = "Authorization";
        public const string Server = "Server";
        public const string Location = "Location";

        // fields to serialize Message in Head fields
        public const string MessageSource = "Message-Source";
        public const string MessageDestiny = "Message-Destiny";
        public const string MessageId = "Message-Id";
        public const string MessageHops = "Message-Hops";
        public const string MessagePath = "Message-Path";

        // content types
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";
        public const string MultiPart = "multipart/form-data";
        public const string ContentTypeDodoNetObject = "application/dodonet";

        // parameter names
        public const string Charset = "charset";
        public const string Name = "name";
        public const string Filename = "filename";
        public const string Boundary = "boundary";

        public static readonly Dictionary<Codes, string> CodeReasons =
            new Dictionary<Codes, string>() {
                {Codes.OK, "Ok"}, 
                {Codes.ACCEPTED, "Accepted"}, 
                {Codes.NOT_FOUND, "Not Found"}, 
                {Codes.NOT_MODIFIED, "Not Modified"},
                {Codes.MOVED_PERMANENTLY, "Moved Permanently"},
                {Codes.FORBIDDEN, "Forbidden"},
                {Codes.TEMPORARY_REDIRECT, "Temporary Redirect"},
                {Codes.EXPECTATION_FAILED, "Expectation Failed"},
                {Codes.UNAUTHORIZED, "Unauthorized"},
                {Codes.SERVER_ERROR, "Internal Server Error"}
            };
    }
}
