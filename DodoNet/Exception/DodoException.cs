using System;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;

using DodoNet.Overlay;

namespace DodoNet
{
    #region enums

    public enum ERROR_CODES
    {
        E_UNKNOWN = 1,
        E_TIME_OUT,
        E_EXCEPTION,
        E_SOCKET,
        E_USER = 9999
    }

    public enum ErrorLocations
    {
        local, remote
    }

    #endregion

    public class DodoException : Exception
    {
        public OverlayErrorMsg exceptionOverlay;                    // mensaje de error que llego del nodo remoto

        // Serialized Members
        public ErrorLocations errorLocation;                        // localizacion donde se produce el error (en nodo propio local o en el remoto)
        public string error_description;    					    // descripción genérica del error
        public string error_details;    						    // detalles concretos del error (traza, códigos, ...)
        public int error_code = (int)ERROR_CODES.E_UNKNOWN;	        // código del error producido. 0->No hay error; 1 - 9999 -> Reservados DodoNet; >9999 -> de usuario
        public int error_level; 						            // nivel de criticidad del error. A interpretar por las aplicaciones. A mayor valor más crítico (0->No critico)
        public NodeBind targetNodeBind;						    // nodeBind donde se produce el error

        public string stackTrace;								    // pila de llamadas
        public override string StackTrace { get { return stackTrace; } }

        public string typeException;						        // tipo de la exception
        public string typeRequest;					                // petición que provocó el error

        public string targetSiteName;                                   // método donde se produjo el error
        public string TargetSiteName { get { return targetSiteName; } }

        #region Statics Methods

        public static string GetStackCalls()
        {
            string ret = "";
            try
            {
                StringBuilder str = new StringBuilder();

                StackTrace st = new StackTrace(true);
                int y = 0;
                for (int x = 3; x < st.FrameCount; x++)
                {
                    y++;
                    StackFrame sf = st.GetFrame(x);
                    str.Append(string.Format("({0:000})\t {1} Line: {2}\n", y, sf.GetMethod().DeclaringType.Name, sf.GetMethod().Name, sf.GetFileLineNumber()));
                }
                ret = str.ToString();
            }
            catch { }
            return ret;
        }

        #endregion

        #region Constructors

        public DodoException(Exception err, string message, params object[] args)
            : base(string.Format(message, args))
        {
            error_description = string.Format(message, args);

            SetExceptionProperties(err);
        }

        public DodoException(Exception err)
            : base(err.Message)
        {
            SetExceptionProperties(err);
        }

        public DodoException(int code, string message, params object[] args)
            : base(string.Format(message, args))
        {
            error_code = code;
            error_description = string.Format("{0};{1};{2}", code, message, args);
        }

        public DodoException(string message, params object[] args)
            : base(string.Format(message, args))
        {
            error_description = string.Format(message, args);
        }

        public DodoException(OverlayErrorMsg oem)
            : base(oem.error_description)
        {
            error_description = oem.error_description;
            error_details = oem.error_details;
            error_code = (int)oem.error_code;
            error_level = oem.error_level;
            errorLocation = oem.errorLocation;
            typeException = oem.typeException;
            typeRequest = oem.typeRequest;
            stackTrace = oem.stackTrace;
            targetSiteName = oem.targetSiteName;

            exceptionOverlay = oem;
        }

        public DodoException()
            : base()
        {
        }

        #endregion

        public void SetExceptionProperties(Exception err)
        {
            Exception errTmp = err.InnerException == null ? err : err.InnerException;

            errorLocation = ErrorLocations.remote;
            error_code = (int)ERROR_CODES.E_EXCEPTION;
            error_description = errTmp.Message;

            errorLocation = ErrorLocations.remote;
            stackTrace = errTmp.StackTrace;
            if (errTmp != null && errTmp.TargetSite != null)
            {
                error_details = errTmp.TargetSite.Name;
                targetSiteName = errTmp.TargetSite.ToString();
            }

            typeException = err.GetType().AssemblyQualifiedName;
        }
    }

    [Serializable]
    public class OverlayErrorMsg : OverlayReply
    {
        // Serialized Members
        public ErrorLocations errorLocation;                        // localizacion donde se produce el error (en nodo propio local o en el remoto)
        public string error_description;    						// Descripción genérica del error
        public string error_details;    						    // Detalles concretos del error (traza, códigos, ...)
        public int error_code = (int)ERROR_CODES.E_UNKNOWN;	        // Código del error producido. 0->No hay error; 1 - 9999 -> Reservados DodoNet; >9999 -> de usuario
        public int error_level; 						            // Nivel de criticidad del error. A interpretar por las aplicaciones. A mayor valor más crítico (0->No critico)
        public string stackTrace;									// pila de llamadas
        public string typeException;    						    // tipo de la exception
        public string typeRequest;  						        // petición que provocó el error
        public string targetSiteName;                                   // método que dió el error

        public OverlayErrorMsg(OverlayRequest request, DodoException exception)
            : base(request)
        {
            error_description = exception.error_description;
            error_details = exception.error_details;
            error_code = exception.error_code;
            error_level = exception.error_level;
            errorLocation = exception.errorLocation;
            stackTrace = exception.stackTrace;
            typeException = exception.typeException;
            typeRequest = exception.typeRequest;
            targetSiteName = exception.targetSiteName;
        }

        public Exception GetException()
        {
            Exception ret = null;

            if (typeException != null)
            {
                string tmp = String.Format("Happened in {0}. Details: {1} {2} {3}", errorLocation, error_details, error_description, stackTrace);
                ret = (Exception)Activator.CreateInstance(Type.GetType(typeException, false),
                    new object[] { tmp, new DodoException(this) });
            }
            else
                ret = new DodoException(this);

            return ret;
        }
    }

    #region Exception user messages

    /// <summary>
    /// Descripción breve de ErrorLog.
    /// </summary>
    public class MessageException
    {
        public static DialogResult ShowErrorMsg(string text, IWin32Window owner, Exception exception)
        {
            try
            {
                DialogResult ret = DialogResult.None;
                string txtError = "";

                txtError = string.Format("{0}\n\n", text);
                txtError += string.Format("Descripción:\t{1}\nFuente del error:\t{0}", exception.Source, exception.Message);

                if (exception is DodoException)
                {
                    DodoException acpError = (DodoException)exception;
                    txtError += "\n\nExcepción DodoNet:";
                    txtError += "\n________________________________________";
                    if (acpError.TargetSite != null)
                        txtError += string.Format("\nMétodo:\t{0}", acpError.TargetSite);
                    txtError += string.Format("\nLugar del error:\t{0}", acpError.errorLocation);
                    txtError += string.Format("\nTraza del error:\t{0}", acpError.stackTrace);
                }

                ret = MessageBox.Show(owner,
                    txtError,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return ret;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static DialogResult ShowErrorMsg(IWin32Window owner, Exception exception)
        {
            try
            {
                DialogResult ret = DialogResult.None;
                string txtError = "";

                txtError = string.Format("Fuente del error:\t{0}\nDescripción:\t{1}", exception.Source, exception.Message);

                DodoException acpError = exception as DodoException;

                if (acpError == null && exception.InnerException != null)
                    acpError = exception.InnerException as DodoException;

                if (acpError != null)
                {
                    txtError += "\n\nExcepción DodoNet:";
                    txtError += "\n________________________________________";
                    txtError += string.Format("\nLugar del error:\t{0}", acpError.errorLocation);
                    txtError += string.Format("\nTraza del error:\t{0}", acpError.stackTrace);
                }

                ret = MessageBox.Show(owner,
                    txtError,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return ret;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }

    #endregion
}
