using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.IO;

using DodoNet;
using DodoNet.Utility;

namespace DodoService
{
    [Serializable]
    public class DodoStateKeys
    {
        #region Manage Keys

        public string GenerateId()
        {
            lock (sync)
            {
                string ret = "";
                ret = Extra.Num2Don(app.StateKeys.mainCount++);
                return ret;
            }
        }

        public void SaveInfo()
        {
            lock (sync)
            {
                app.StateKeys.totalRecords++;
                app.SaveStateKeys();
            }
        }

        static DodoStateKeys stateKeys;
        public static DodoStateKeys StateKeys { get { return stateKeys; } set { stateKeys = value; } }

        static string fileStateKeys = "keys.ser";

        static object sync = new object();

        public static void LoadStateKeys()
        {
            if (Monitor.TryEnter(sync))
            {
                try
                {
                    if (File.Exists(fileStateKeys))
                    {
                        using (FileStream fs = File.OpenRead(fileStateKeys))
                        {
                            stateKeys = Serialization.Deserialize(fs) as DodoStateKeys;
                            stateKeys.mainCount = stateKeys.totalRecords;
                        }
                    }
                    else
                    {
                        stateKeys = new DodoStateKeys();
                        stateKeys.totalRecords = CurrentDb.GetTotalRecords();
                        stateKeys.mainCount = stateKeys.totalRecords;
                        SaveStateKeys();
                    }
                }
                finally
                {
                    Monitor.Exit(sync);
                }
            }
        }

        public static void SaveStateKeys()
        {
            if (Monitor.TryEnter(sync))
            {
                try
                {
                    using (FileStream fs = File.Open(fileStateKeys, FileMode.OpenOrCreate))
                    {
                        Stream st = Serialization.Serialize(stateKeys);
                        st.Position = 0;

                        byte[] buffer = new byte[4096];
                        int c = 0;
                        while ((c = st.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, c);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(sync);
                }
            }
        }

        public static void UpdateStateKeys()
        {
            stateKeys.RefreshInfo(this);
            SaveStateKeys();
        }

        #endregion

        public long mainCount = 0;
        public long sizeBigFileInBytes = 0;
        public long totalRecords = 0;
        public int totalActivatedServers = 1;

        public void RefreshInfo(DodoApplication app)
        {
            sizeBigFileInBytes = app.CurrentDb.GetSizeBigFileInBytes();
            totalActivatedServers = 1;
            totalRecords = app.CurrentDb.GetTotalRecords();
        }
    }

    public class UpdateStateKeys : AcpTimer
    {
        public UpdateStateKeys(long initialDelayMs, long periodMs)
            : base(initialDelayMs, periodMs)
        { 
        }

        public override void Execute(AcpNode localNode)
        {
            try
            {
                DodoApplication app = (DodoApplication)localNode.HtmlBase;
                app.UpdateStateKeys();
            }
            catch { }
        }
    }
}
