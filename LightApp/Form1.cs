using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using DodoNet.Tools;

using LightApp.ServiceProcess;

namespace LightApp
{
	/// <summary>
	/// Descripción breve de Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
    {
        private IContainer components;

		public Form1()
		{
			//
			// Necesario para admitir el Diseñador de Windows Forms
			//
			InitializeComponent();
		}

		/// <summary>
		/// Limpiar los recursos que se estén utilizando.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.lblConcurrency = new System.Windows.Forms.Label();
            this.lblThreadsInPool = new System.Windows.Forms.Label();
            this.lblWorksInPool = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblIdentifier = new System.Windows.Forms.Label();
            this.lblEndPoint = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSessions = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblRoutes = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblRecMsg = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblSentMsg = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblErrors = new System.Windows.Forms.Label();
            this.txtServiceName = new System.Windows.Forms.TextBox();
            this.cmdInstallService = new System.Windows.Forms.Button();
            this.cmdUninstallService = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.lblSentBytes = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblReceivedBytes = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblConcurrency
            // 
            this.lblConcurrency.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblConcurrency.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblConcurrency.Location = new System.Drawing.Point(92, 161);
            this.lblConcurrency.Name = "lblConcurrency";
            this.lblConcurrency.Size = new System.Drawing.Size(32, 23);
            this.lblConcurrency.TabIndex = 0;
            this.lblConcurrency.Text = "0";
            this.lblConcurrency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblThreadsInPool
            // 
            this.lblThreadsInPool.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblThreadsInPool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblThreadsInPool.Location = new System.Drawing.Point(92, 187);
            this.lblThreadsInPool.Name = "lblThreadsInPool";
            this.lblThreadsInPool.Size = new System.Drawing.Size(32, 23);
            this.lblThreadsInPool.TabIndex = 1;
            this.lblThreadsInPool.Text = "0";
            this.lblThreadsInPool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblWorksInPool
            // 
            this.lblWorksInPool.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblWorksInPool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWorksInPool.Location = new System.Drawing.Point(92, 214);
            this.lblWorksInPool.Name = "lblWorksInPool";
            this.lblWorksInPool.Size = new System.Drawing.Size(32, 23);
            this.lblWorksInPool.TabIndex = 2;
            this.lblWorksInPool.Text = "0";
            this.lblWorksInPool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(12, 214);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 23);
            this.label4.TabIndex = 5;
            this.label4.Text = "WorksInPool";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(12, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 23);
            this.label5.TabIndex = 4;
            this.label5.Text = "Threads";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Control;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(12, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 23);
            this.label6.TabIndex = 3;
            this.label6.Text = "Concurrency";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(11, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 50);
            this.label7.TabIndex = 6;
            this.label7.Text = "Engine";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(11, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Identifier:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblIdentifier
            // 
            this.lblIdentifier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIdentifier.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblIdentifier.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblIdentifier.Location = new System.Drawing.Point(111, 10);
            this.lblIdentifier.Name = "lblIdentifier";
            this.lblIdentifier.Size = new System.Drawing.Size(197, 16);
            this.lblIdentifier.TabIndex = 8;
            this.lblIdentifier.Text = "...";
            this.lblIdentifier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEndPoint
            // 
            this.lblEndPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEndPoint.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblEndPoint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEndPoint.Location = new System.Drawing.Point(111, 30);
            this.lblEndPoint.Name = "lblEndPoint";
            this.lblEndPoint.Size = new System.Drawing.Size(197, 16);
            this.lblEndPoint.TabIndex = 10;
            this.lblEndPoint.Text = "...";
            this.lblEndPoint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(11, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 16);
            this.label8.TabIndex = 9;
            this.label8.Text = "EndPoint:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(11, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 23);
            this.label2.TabIndex = 12;
            this.label2.Text = "Sessions:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSessions
            // 
            this.lblSessions.BackColor = System.Drawing.Color.LightCoral;
            this.lblSessions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSessions.Location = new System.Drawing.Point(91, 51);
            this.lblSessions.Name = "lblSessions";
            this.lblSessions.Size = new System.Drawing.Size(32, 23);
            this.lblSessions.TabIndex = 11;
            this.lblSessions.Text = "0";
            this.lblSessions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(11, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 23);
            this.label3.TabIndex = 14;
            this.label3.Text = "Routes:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRoutes
            // 
            this.lblRoutes.BackColor = System.Drawing.Color.LightCoral;
            this.lblRoutes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRoutes.Location = new System.Drawing.Point(91, 77);
            this.lblRoutes.Name = "lblRoutes";
            this.lblRoutes.Size = new System.Drawing.Size(32, 23);
            this.lblRoutes.TabIndex = 13;
            this.lblRoutes.Text = "0";
            this.lblRoutes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(136, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 23);
            this.label9.TabIndex = 18;
            this.label9.Text = "Rec. Msgs:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRecMsg
            // 
            this.lblRecMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecMsg.BackColor = System.Drawing.Color.PaleGreen;
            this.lblRecMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRecMsg.Location = new System.Drawing.Point(216, 78);
            this.lblRecMsg.Name = "lblRecMsg";
            this.lblRecMsg.Size = new System.Drawing.Size(92, 23);
            this.lblRecMsg.TabIndex = 17;
            this.lblRecMsg.Text = "0";
            this.lblRecMsg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(136, 51);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 23);
            this.label11.TabIndex = 16;
            this.label11.Text = "Sent Msgs:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSentMsg
            // 
            this.lblSentMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSentMsg.BackColor = System.Drawing.Color.PaleGreen;
            this.lblSentMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSentMsg.Location = new System.Drawing.Point(216, 51);
            this.lblSentMsg.Name = "lblSentMsg";
            this.lblSentMsg.Size = new System.Drawing.Size(92, 23);
            this.lblSentMsg.TabIndex = 15;
            this.lblSentMsg.Text = "0";
            this.lblSentMsg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(136, 106);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 23);
            this.label10.TabIndex = 20;
            this.label10.Text = "Errors:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblErrors
            // 
            this.lblErrors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblErrors.BackColor = System.Drawing.Color.PaleGreen;
            this.lblErrors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblErrors.Location = new System.Drawing.Point(216, 106);
            this.lblErrors.Name = "lblErrors";
            this.lblErrors.Size = new System.Drawing.Size(92, 23);
            this.lblErrors.TabIndex = 19;
            this.lblErrors.Text = "0";
            this.lblErrors.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtServiceName
            // 
            this.txtServiceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServiceName.Location = new System.Drawing.Point(11, 249);
            this.txtServiceName.MaxLength = 255;
            this.txtServiceName.Name = "txtServiceName";
            this.txtServiceName.Size = new System.Drawing.Size(297, 20);
            this.txtServiceName.TabIndex = 23;
            this.txtServiceName.Text = "LightApp :: Server";
            this.toolTip1.SetToolTip(this.txtServiceName, "Nombre del servicio de LightApp");
            // 
            // cmdInstallService
            // 
            this.cmdInstallService.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdInstallService.Location = new System.Drawing.Point(136, 188);
            this.cmdInstallService.Name = "cmdInstallService";
            this.cmdInstallService.Size = new System.Drawing.Size(172, 22);
            this.cmdInstallService.TabIndex = 24;
            this.cmdInstallService.Text = "Instalar servicio";
            this.cmdInstallService.UseVisualStyleBackColor = true;
            this.cmdInstallService.Click += new System.EventHandler(this.cmdInstallService_Click);
            // 
            // cmdUninstallService
            // 
            this.cmdUninstallService.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdUninstallService.Location = new System.Drawing.Point(136, 215);
            this.cmdUninstallService.Name = "cmdUninstallService";
            this.cmdUninstallService.Size = new System.Drawing.Size(172, 22);
            this.cmdUninstallService.TabIndex = 25;
            this.cmdUninstallService.Text = "Desinstalar servicio";
            this.cmdUninstallService.UseVisualStyleBackColor = true;
            this.cmdUninstallService.Click += new System.EventHandler(this.cmdUninstallService_Click);
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.SystemColors.Control;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Location = new System.Drawing.Point(136, 133);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 23);
            this.label12.TabIndex = 27;
            this.label12.Text = "Bytes env:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSentBytes
            // 
            this.lblSentBytes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSentBytes.BackColor = System.Drawing.Color.Khaki;
            this.lblSentBytes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSentBytes.ForeColor = System.Drawing.Color.Crimson;
            this.lblSentBytes.Location = new System.Drawing.Point(216, 133);
            this.lblSentBytes.Name = "lblSentBytes";
            this.lblSentBytes.Size = new System.Drawing.Size(92, 23);
            this.lblSentBytes.TabIndex = 26;
            this.lblSentBytes.Text = "0";
            this.lblSentBytes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.SystemColors.Control;
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Location = new System.Drawing.Point(136, 160);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(72, 23);
            this.label13.TabIndex = 29;
            this.label13.Text = "Bytes rec:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblReceivedBytes
            // 
            this.lblReceivedBytes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReceivedBytes.BackColor = System.Drawing.Color.Khaki;
            this.lblReceivedBytes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReceivedBytes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReceivedBytes.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblReceivedBytes.Location = new System.Drawing.Point(216, 160);
            this.lblReceivedBytes.Name = "lblReceivedBytes";
            this.lblReceivedBytes.Size = new System.Drawing.Size(92, 23);
            this.lblReceivedBytes.TabIndex = 28;
            this.lblReceivedBytes.Text = "0";
            this.lblReceivedBytes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(320, 281);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblReceivedBytes);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lblSentBytes);
            this.Controls.Add(this.cmdUninstallService);
            this.Controls.Add(this.cmdInstallService);
            this.Controls.Add(this.txtServiceName);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblErrors);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblRecMsg);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lblSentMsg);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblRoutes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblSessions);
            this.Controls.Add(this.lblEndPoint);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblIdentifier);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblWorksInPool);
            this.Controls.Add(this.lblThreadsInPool);
            this.Controls.Add(this.lblConcurrency);
            this.Name = "Form1";
            this.Text = "LightApp";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label lblConcurrency;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lblIdentifier;
		private System.Windows.Forms.Label lblEndPoint;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblThreadsInPool;
		private System.Windows.Forms.Label lblWorksInPool;
		private System.Windows.Forms.Label lblSessions;

		#region MyObjects

		Server server;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblRoutes;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label lblRecMsg;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label lblSentMsg;
		private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblErrors;
        private TextBox txtServiceName;
        private Button cmdInstallService;
        private Button cmdUninstallService;
        private ToolTip toolTip1;
        private Label label12;
        private Label lblSentBytes;
        private Label label13;
        private Label lblReceivedBytes;
		System.Threading.Timer timer;

		#endregion

		private void Form1_Load(object sender, System.EventArgs e)
		{
			try
			{
				server = new Server(false, null);

				// actualizamos la pantalla
				lblIdentifier.Text = server.Node.localBind.NodeId.ToString();
                lblEndPoint.Text = server.Node.localBind.NodeAddress.ToString();

				timer = new System.Threading.Timer( new TimerCallback( TimerFunction ),  
					this, 0, 1000 );
			}
			catch
			{                
				this.Dispose();
			}
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				if ( server != null )
				{
					server.Dispose();
				}
			}
			catch
			{
			}
		}

        private void TimerFunction(object state)
		{
			try
			{
                BeginInvoke(new SetStatisticLabelsDelegate(SetStatisticLabels), new object[] { });
			}
			catch ( Exception ex)
			{
                Console.WriteLine(string.Format("{0}", ex.Message));
			}
		}

        private delegate void SetStatisticLabelsDelegate();
        private void SetStatisticLabels()
        {
            lblConcurrency.Text = server.Node.localEvents.CurInUseThreads.ToString();
            lblThreadsInPool.Text = server.Node.localEvents.CurActiveThreads.ToString();
            lblWorksInPool.Text = server.Node.localEvents.CurQueuedWorks.ToString();
            // lblSessions.Text = server.Node.Sessions.Count.ToString();
            lblRoutes.Text = server.Node.RouteTable.Count.ToString();
            // lblSentMsg.Text = server.Node.numSentReply.ToString();
            // lblRecMsg.Text = server.Node.numReceivedRequest.ToString(); 
            // lblErrors.Text = server.Node.numErrors.ToString();
            lblSentBytes.Text = ReadableDataLength.Calculate(server.Node.mailboxOut.TotalBytesSent);
            lblReceivedBytes.Text = ReadableDataLength.Calculate(server.Node.mailboxIn.TotalBytesReceived);
        }

        private void cmdInstallService_Click(object sender, EventArgs e)
        {
            try
            {
                string nameService = txtServiceName.Text;

                if (WinService.Install(
                    AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + " -service",
                    nameService, nameService, "DodoServer",
                    ServiceProcess.Attributes.ServiceType.Default,
                    ServiceProcess.Attributes.ServiceAccessType.Start,
                    ServiceProcess.Attributes.ServiceStartType.AutoStart,
                    ServiceProcess.Attributes.ServiceErrorControl.Normal,
                    "LanmanWorkstation\0LanmanServer\0Netman"))

                    MessageBox.Show("Instalación correcta. Reinicia el servidor para que se inicie el servicio.");
                else
                    MessageBox.Show("Error en la instalación");
            }
            catch
            {
            }
        }

        private void cmdUninstallService_Click(object sender, EventArgs e)
        {
            try
            {
                string nameService = txtServiceName.Text;
                if (WinService.Uninstall(nameService))
                    MessageBox.Show("Desinstalación correcta");
                else
                    MessageBox.Show("Error en la desinstalación");

            }
            catch
            {
            }
        }
	}
}
