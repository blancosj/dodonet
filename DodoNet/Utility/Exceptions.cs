using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace DodoNet
{
    public class Thrower
    {
        /// <summary>
        /// conserva el la pila de llamadas del error
        /// </summary>
        /// <param name="err"></param>
        public static void Exception(Exception err)
        {
            Exception tmp = null;

            if (err is InnerException)
                tmp =((InnerException)err).internalException;
            else
                tmp = err;

            throw new InnerException(err.Message, tmp);
        }

        /// <summary>
        /// obtener el error
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static Exception Get(Exception err)
        {
            Exception ret = null;

            if (err is InnerException)
                ret = ((InnerException)err).internalException;
            else
                ret = err;

            return ret;
        }

        /// <summary>
        /// excepción que conserva la pila de llamadas
        /// </summary>
        [Serializable]
        public class InnerException : Exception
        {
            internal Exception internalException;

            internal string stackTrace;
            public override string StackTrace { get { return stackTrace; } }

            MethodBase targetSite;
            public new MethodBase TargetSite { get { return targetSite; } }

            public InnerException(string message, Exception internalException)
                : base(message, internalException)
            {
                this.internalException = internalException;
                this.stackTrace = internalException.StackTrace;
                this.targetSite = internalException.TargetSite;
            }
        }
    }
}
