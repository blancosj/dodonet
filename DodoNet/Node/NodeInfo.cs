using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Security.Principal;

namespace DodoNet
{
    [Serializable]
    public class NodeInfo
    {
        // propiedades del nodo
        private string m_UserName;
        private string m_DomainName;
        private string m_OperatingSystem;
        private string m_MachineName;
        private string m_MachineId;
        private string m_AppConnecting;
        private string m_AppConnectingVer;
        private string m_VerDodoNet;
        private string m_SID;
        private int m_ProcessorCount;

        // propiedades
        public string UserName { get { return m_UserName; } }
        public string DomainName { get { return m_DomainName; } }
        public string OperatingSystem { get { return m_OperatingSystem; } }
        public string MachineName { get { return m_MachineName; } }
        public string MachineId { get { return m_MachineId; } }
        public string AppConnecting { get { return m_AppConnecting; } set { m_AppConnecting = value; } }
        public string AppConnectingVer { get { return m_AppConnectingVer; } set { m_AppConnectingVer = value; } }
        public string VerDodoNet { get { return m_VerDodoNet; } }
        public string SID { get { return m_SID; } }
        public int ProcessorCount { get { return m_ProcessorCount; } }

        public void InitLocal()
        {
            try
            {
                // rellenar datos del entorno donde se va a crear el nodo
                m_UserName = Environment.UserName;
                m_MachineName = Environment.MachineName;
                m_OperatingSystem = System.EnvironmentInfo.ComputerInfo.GetOperatingSystem();	// sistema operativo
                m_MachineId = System.EnvironmentInfo.ComputerInfo.GetMACAddress();  // MAC ADRESS
                m_DomainName = Environment.UserDomainName;
                m_VerDodoNet = System.EnvironmentInfo.LibInfo.GetVersionThisModule();	// version del modulo actual

                WindowsIdentity c = WindowsIdentity.GetCurrent();

                // preguntamos por el sid del administrador la máquina local en varios idiomas
                NTAccount name;
                try
                {
                    name = new NTAccount(Environment.MachineName.ToUpper(), "ADMINISTRADOR");
                    SecurityIdentifier sid = (SecurityIdentifier)name.Translate(typeof(SecurityIdentifier));
                    m_SID = sid.AccountDomainSid.ToString();
                }
                catch { }

                if (String.IsNullOrEmpty(m_SID))
                {
                    try
                    {
                        name = new NTAccount(Environment.MachineName.ToUpper(), "ADMINISTRATOR");
                        SecurityIdentifier sid = (SecurityIdentifier)name.Translate(typeof(SecurityIdentifier));
                        m_SID = sid.AccountDomainSid.ToString();
                    }
                    catch { }
                }

                try
                {
                    if (String.IsNullOrEmpty(m_SID))
                        m_SID = c.User.Value;
                }
                catch { }

                Node.LogAppendLine("SID: {0} {1}", m_SID, string.Format("{0}/{1}", m_MachineName, m_UserName));

                // quitamos el .exe del nombre de la aplicacion				
                string app = AppDomain.CurrentDomain.FriendlyName;

                if (app == "DefaultDomain")
                    app = "Dodo";

                int pos = app.IndexOf(".exe", 0);
                if (pos > 0)
                    app = app.Substring(0, pos);

                m_AppConnecting = app; // aplicación que se conecta

                try
                {
                    Version ver = AssemblyName.GetAssemblyName(AppDomain.CurrentDomain.FriendlyName).Version;
                    m_AppConnectingVer = string.Format("{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
                }
                catch { m_AppConnectingVer = "unknown"; }

                m_ProcessorCount = Environment.ProcessorCount;
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
                throw err;
            }
        }
    }
}
