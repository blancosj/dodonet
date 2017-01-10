using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;

using DodoNet.Logs;

namespace LightApp
{
	public class Service : System.ServiceProcess.ServiceBase
	{
		/// <summary> 
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region MyObjects

		Server server;

		#endregion

		/// <summary>
		/// para excribir logs
		/// </summary>
		private static FileLog log;
		public static FileLog Log { get { return log; } }

        public Service()
		{
			// Llamada necesaria para el Diseñador de componentes Windows.Forms.
			InitializeComponent();
		}

		// Punto de entrada principal del proceso
		[MTAThread]
		static void Main( string[] args )
		{
			try
            {
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

				log = new FileLog( AppDomain.CurrentDomain.BaseDirectory + "logs\\" + AppDomain.CurrentDomain.FriendlyName );
				log.AppendLine ( "Iniciando" );
				if ( args.Length == 0 )
				{
					// iniciar la aplicacion
				    Application.Run(new Form1());
				}
				else if ( args != null && args.Length > 0 && args[0] == "-service" )
				{
					log.AppendLine ( "Modo servicio." );
					System.ServiceProcess.ServiceBase[] ServicesToRun;	
					ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service() };
					System.ServiceProcess.ServiceBase.Run( ServicesToRun );
				}
			}
			catch ( Exception e )
			{
				log.AppendLine ( "Error ocurrido {0} {1}", e.TargetSite, e.Message );
			}
			finally
			{
                try
                {
                    if (log != null)
                    {
                        log.AppendLine("Finalizado");
                    }
                }
                catch { }
			}
		}

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            log.AppendLine(e.Exception);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.AppendLine("UnhandledException: {0} {1}", sender, e.ExceptionObject);
        }

		/// <summary> 
		/// Método necesario para admitir el Diseñador. No se puede modificar 
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.ServiceName = "Server";
		}

		/// <summary>
		/// Poner en movimiento los elementos para que el servicio pueda funcionar.
		/// </summary>
		protected override void OnStart( string[] args )
		{
			try
			{
				server = new Server(true, this);
			}
			catch ( Exception err )
			{
				throw err;
			}
		}
 
		/// <summary>
		/// Detener el servicio.
		/// </summary>
		protected override void OnStop()
		{
			try
			{
				if ( server != null )
				{
					server.Dispose();
					server = null;
				}
			}
			catch ( Exception err )
			{
				throw err;
			}
		}

		/// <summary>
		/// Limpiar los recursos que se estén utilizando.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}

				if ( server != null )
				{
					server.Dispose();
					server = null;
				}
			}
			base.Dispose( disposing );
		}

	}
}
