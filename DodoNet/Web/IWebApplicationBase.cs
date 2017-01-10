using System;
using System.Collections.Generic;
using System.Text;

using DodoNet.Http;

namespace DodoNet.Web
{
    /// <summary>
    /// clase basica para crear una aplicación servidor html
    /// </summary>
    public interface IWebApplicationBase : IDisposable
    {
        /// <summary>
        /// página de inicio
        /// </summary>        
        string Home { set; get; }
        /// <summary>
        /// dominio virtual (si nulo es cualquier dominio)
        /// </summary>
        string Host { set; get; }
        /// <summary>
        /// lista de extensiones que la aplicación procesa
        /// </summary>
        Dictionary<string, IWebApplicationBase> ExtList { set; get; }
        /// <summary>
        /// obtener la respuesta
        /// </summary>
        /// <param name="overlayNode">nodo recepcionador del mensaje</param>
        /// <param name="message">petición</param>
        /// <returns></returns>
        void GetReply(HttpContext context);
    }
}
