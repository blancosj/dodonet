using System;
using System.Collections.Generic;
using System.Text;

using DodoNet.Web;

namespace DodoNet.Http
{
    public interface IHttpRender
    {
        void printl(string text, params object[] args);
        void print(string text, params object[] args);
        void printobj(IWebObject obj);
        void printfile(string file);
    }
}
