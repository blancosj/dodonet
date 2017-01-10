using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

using DodoNet;
using DodoNet.Http;
using DodoNet.JScript;
using DodoNet.Logs;
using DodoNet.Session;

namespace LightApp
{
    /// <summary>
    /// Descripción breve de Server.
    /// </summary>
    public class Server : EvaluatorJScript, IDisposable
    {
        // nodo
        Node node;					        
        public Node Node { get { return node; } set { node = value; } }

        Service service;				        // servicio
        public Service Service { get { return service; } set { service = value; } }

        public string BaseDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        private bool disposed;				    // var ctrl metodo Dispose

        public FileLog Log { get { return Service.Log; } }

        //-- public Type GetHtmlType { get { return typeof(IWebApplicationBase); } }

        public Server(bool modeService, Service service)
        {
            try
            {
                this.service = service;

                disposed = false;

                Node.log = Service.Log;
                Service.Log.AppendLine("Creando el proceso del servidor");

                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "run.js", FileMode.Open, FileAccess.Read);

                if (fs != null)
                {
                    StringBuilder run = new StringBuilder();
                    object ret = null;
                    byte[] buffer = new byte[4096];
                    int n = 0;
                    while ((n = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        string temp = Encoding.Default.GetString(buffer, 0, n);
                        run.Append(temp);
                    }

                    fs.Dispose();

                    try
                    {
                        Service.Log.AppendLine("** Inicio run.js");
                        ret = DoEval(run.ToString());
                        Service.Log.AppendLine("** OK");
                    }
                    catch (Exception err)
                    {
                        Service.Log.AppendLine("** Error al ejecutar run.js " + err.Message + "\r\n" + err.StackTrace);
                        throw err;
                    }
                }

                Service.Log.AppendLine("Nodo creado OK");
            }
            catch (Exception err)
            {
                string jErrorMessage = "";

                Microsoft.JScript.JScriptException jerr = err as Microsoft.JScript.JScriptException;

                if (jerr != null)
                {
                    jErrorMessage = string.Format("Error en run.js en la línea {0}", jerr.Line);

                    Node.LogAppendLine(jErrorMessage);

                    jErrorMessage = "\r\n\r\n" + jErrorMessage;
                }

                if (!modeService)
                    MessageBox.Show("Error al arrancar el Server\r\n\r\n" + err.Message + jErrorMessage, 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);    

                Service.Log.AppendLine(err);

                if (node != null)
                    node.Dispose();
                throw err;
            }
        }

        /// <summary>
        /// carga una libreria en memoria
        /// </summary>
        /// <param name="pathLib"></param>
        /// <returns></returns>
        public Assembly LoadAssembly(string pathLib)
        {
            return Assembly.LoadFile(pathLib);
        }

        /// <summary>
        /// crea un objeto
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object CreateObject(Assembly assembly, string typeName)
        {
            Type tt = assembly.GetType(typeName);
            return Activator.CreateInstance(tt);
        }

        public void Alert(string text)
        {
            MessageBox.Show(text);
        }

        #region Log

        public void LogAppendLine(string text, params object[] args)
        {
            Service.Log.AppendLine(text, args);
        }

        public void LogAppend(string text, params object[] args)
        {
            Service.Log.Append(text, args);
        }

        public void LogAppendLine(string text)
        {
            Service.Log.AppendLine(text);
        }

        public void LogAppend(string text)
        {
            Service.Log.Append(text);
        }

        #endregion

        #region Miembros de IDisposable

        public void Dispose()
        {
            try
            {
                if (!disposed)
                {
                    Service.Log.AppendLine("Destruyendo todos los nodos");

                    // AcpNode.DisposeListNode();

                    node.Dispose();
                    node = null;

                    disposed = true;
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
        }

        #endregion
    }
}
