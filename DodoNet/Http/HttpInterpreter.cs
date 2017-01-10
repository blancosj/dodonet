using System;
using System.Collections.Generic;
using System.Text;

using DodoNet.JScript;

namespace DodoNet.Http
{
    /// <summary>
    /// Interface para crear un interpretador de paginas
    /// </summary>
    public abstract class HttpInterpreter
    {
        public abstract string Extension { get; }

        protected HttpContext context;

        public HttpInterpreter(HttpContext context)
        {
            this.context = context;
        }
    
        public abstract void Interpret();
    }
}
