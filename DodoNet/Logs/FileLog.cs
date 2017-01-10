using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;

using log4net;
using log4net.Layout;
using log4net.Appender;
using log4net.Config;

namespace DodoNet.Logs
{
    /// <summary>
    /// Descripción breve de FileLog.
    /// </summary>
    public class FileLog
    {
        public string FileFullName { get { return fileLog.File; } }

        public static readonly ILog _logger = log4net.LogManager.GetLogger("log");
        public static readonly object static_sync = new object();
        RollingFileAppender fileLog = null;

        public FileLog(string fileName)
        {
            lock (static_sync)
            {
                fileLog = new RollingFileAppender();                
                fileLog.Layout = new PatternLayout("%date [%thread] %-5level %logger [%property{NDC}] - %message%newline");
                fileLog.File = Path.ChangeExtension(fileName, "log");
                fileLog.AppendToFile = true;
                fileLog.ActivateOptions();

                fileLog.MaxFileSize = 20 * 1024 * 1024; // 20MB                

                // configurar
                BasicConfigurator.Configure(fileLog);

                _logger.Info("******************************************************");
                _logger.InfoFormat("{0}\tHead log \t", DateTime.Now);
                _logger.Info(".");
                _logger.Info("Features host computer...");
                _logger.InfoFormat("Version:         \t\t\t{0} {1}", AppDomain.CurrentDomain.FriendlyName, System.Environment.Version);
                _logger.InfoFormat("Command line:    \t\t\t{0}", System.Environment.CommandLine);
                _logger.InfoFormat("Machine name:    \t\t\t{0}", System.Environment.MachineName);
                _logger.InfoFormat("User name:       \t\t\t{0}", System.Environment.UserName);
                _logger.InfoFormat("Memory workset:  \t\t\t{0}", System.Environment.WorkingSet);
                _logger.InfoFormat("Operating system:\t\t\t{0}", System.EnvironmentInfo.ComputerInfo.GetOperatingSystem());

                _logger.Info(".");
                _logger.Info("Referencias:");

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    _logger.InfoFormat("\t\t{0} {1}", assembly.GetName().Name.PadRight(30, char.Parse(" ")), assembly.GetName().Version);
                }

                _logger.Info("******************************************************");
                _logger.Info(".");
            }
        }

        /// <summary>
        /// agregar una linea nueva
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        public void AppendLine(string text, params object[] args)
        {
            _logger.InfoFormat(text, args);
        }

        /// <summary>
        /// escribe a partir del ultimo caracter
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        public void Append(string text, params object[] args)
        {
            _logger.InfoFormat(text, args);
        }

        /// <summary>
        /// Escribe la información de un error
        /// </summary>
        /// <param name="__e"></param>
        public void AppendLine(Exception __e)
        {
            _logger.Fatal(__e.Message, __e);
            // GetStackTrace();
        }

        /// <summary>
        /// Escribe la información de un error
        /// </summary>
        /// <param name="__e"></param>
        public void AppendLine(Exception __e, string message, params object[] args)
        {
            _logger.Fatal(string.Format(message, args), __e);
            // GetStackTrace();
        }

        private string GetStackTrace()
        {
            string ret = "";
            int y = 0;

            StackTrace st = new StackTrace(true);

            for (int x = 0; x < st.FrameCount; x++)
            {
                y++;
                StackFrame sf = st.GetFrame(x);
                _logger.WarnFormat(string.Format("({0:000})\t {1} Line: {2} File:{3}",
                    y, sf.GetMethod().Name, sf.GetFileLineNumber(), sf.GetFileName()));
            }

            return ret;
        }

        #region ILog Members

        public void Debug(object message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Debug(object message, Exception exception)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DebugFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DebugFormat(string format, object arg0)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Info(object message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Info(object message, Exception exception)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InfoFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InfoFormat(string format, object arg0)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Warn(object message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Warn(object message, Exception exception)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(string format, object arg0)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Error(object message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Error(object message, Exception exception)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ErrorFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ErrorFormat(string format, object arg0)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Fatal(object message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Fatal(object message, Exception exception)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(string format, object arg0)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsDebugEnabled
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsInfoEnabled
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsWarnEnabled
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsErrorEnabled
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsFatalEnabled
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region ILoggerWrapper Members

        public log4net.ILog Log
        {
            get { return _logger; }
        }

        #endregion
    }
}
