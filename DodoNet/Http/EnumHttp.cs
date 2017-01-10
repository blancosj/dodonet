using System;
using System.Collections.Generic;
using System.Text;

namespace DodoNet.Http
{
    public enum MessageTypes
    {
        POST,
        GET,
        PUT,
        DELETE,
        RESPONSE, 
        UNKNOWN
    }

    public enum CharsetTypes
    {
        ISO_8859_1,
        UTF8
    }

    public enum ContentEncodings
    {
        GZIP,
        DEFLATE,
        BZIP2,
        NORMAL
    }

    public enum TransferEncodings
    {
        NORMAL,
        CHUNKED        
    }

    public enum Codes : short
    {
        // http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html

        OK = 200,
        ACCEPTED = 202,
        NOT_FOUND = 404,
        NOT_MODIFIED = 304,
        NOT_ACCEPTABLE = 406,
        NOT_CONTENT = 204,
        MOVED_PERMANENTLY = 301,
        TEMPORARY_REDIRECT = 302,
        BAD = 400,
        UNAUTHORIZED = 401,
        FORBIDDEN = 403,
        EXPECTATION_FAILED = 417,
        SERVER_ERROR = 500
    }

    /*

                    case Codes.OK:
                        nameCode = "OK";
                        break;
                    case Codes.NOT_FOUND:
                        nameCode = "Not Found";
                        break;
                    case Codes.MOVED_PERMANENTLY:
                        nameCode = "Moved Permanently";
                        break;
                    case Codes.FORBIDDEN:
                        nameCode = "Forbidden";
                        break;
                    case Codes.TEMPORARY_REDIRECT:
                        nameCode = "Temporary Redirect";
                        break;
                    case Codes.EXPECTATION_FAILED:
                        nameCode = "Expectation Failed";
                        break;
                    case Codes.UNAUTHORIZED:
                        nameCode = "Unauthorized";
                        break;
                    case Codes.SERVER_ERROR:
                        nameCode = "Internal Server Error";
                        break;
                    case Codes.NOT_MODIFIED:
                        nameCode = "Not Modified";
                        break;
     */
}
