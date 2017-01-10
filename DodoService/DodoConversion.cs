using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DodoService
{
    public class DodoConversion : DodoTask
    {
        public static object Sync = new object();

        public static Semaphore lights = new Semaphore(2, 2);

        public DodoItem Item { get; set; }

        public DodoConverter Converter { get; set; }

        string _sourceFileName = string.Empty;
        string _targetFileName = string.Empty;

        public DodoConversion(DodoItem item, DodoApplication app, long delayMs)
            : base(app, delayMs)
        {
            this.Item = item;

            Converter = new Doc2Scribd();
        }

        public void PrepareConversion()
        {
            _sourceFileName = GetOriginalFile();
            _targetFileName = RegisterPending();
        }

        public override void Execute(DodoNet.Node localNode)
        {
            if (Item.fileContentType == Converter.ConvertedContentType)
                throw new Exception("It is not needed converter. Content has same type than converter");

            if (String.IsNullOrEmpty(_sourceFileName))
                throw new ArgumentNullException("_sourceFileName");

            if (String.IsNullOrEmpty(_targetFileName))
                throw new ArgumentNullException("_targetFileName");

            try
            {
                lights.WaitOne();

                string log;
                Converter.Convert(_sourceFileName, ref _targetFileName, out log);
                UpdateTargetPathFile(_targetFileName);
                //UpdateLargeFile(_targetFileName);
                RegisterAvailable(log);

                // paramos el timer
                Periodic = false;
            }
            catch
            {
                TreatError(Item.UniqueId);
                throw;
            }
            finally
            {
                lights.Release();

                if (File.Exists(_sourceFileName))
                    File.Delete(_sourceFileName);                
            }
        }

        void UpdateTargetPathFile(string filePath)
        {
            using (var db = CurrentApp.CurrentDb.CreateConnection())
            using (var cmd = db.CreateCommand())
            {
                var sql = "update {0} set " +
                    "pathPhysical = @target " +
                    "where uniqueId = @id;";
                cmd.CommandText = string.Format(sql, DodoDb.table_poweq2);
                cmd.Parameters.AddWithValue("id", Item.UniqueId);
                cmd.Parameters.AddWithValue("target", filePath);

                int retErr = cmd.ExecuteNonQuery();
            }
        }

        void UpdateLargeFile(string filePath)
        {
            using (var db = CurrentApp.CurrentDb.CreateConnection())
            using (var cmd = db.CreateCommand())
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] buffer = new byte[fs.Length];
                int c = fs.Read(buffer, 0, buffer.Length);

                var sql = "update {0} set " +
                    "boxBinary = @box, fileSize = @size, pathPhysical = @target " +
                    "where uniqueId = @id;";
                cmd.CommandText = string.Format(sql, DodoDb.table_poweq2);
                cmd.Parameters.AddWithValue("id", Item.UniqueId);
                cmd.Parameters.AddWithValue("box", buffer);
                cmd.Parameters.AddWithValue("size", buffer.Length);
                cmd.Parameters.AddWithValue("target", filePath);

                cmd.CommandTimeout = 300;

                int retErr = cmd.ExecuteNonQuery();
            }
        }

        void CalculateFilePath(out string completePath, out string relativePath)
        {
            var fileDir = string.Format("{0:yyyyMM}", DateTime.Now);
            var fileName = DodoNet.Tools.Conversion.Num2Don(DateTime.Now.Ticks)
                + Converter.ConvertedExt;
            relativePath = Path.Combine(fileDir, fileName);

            completePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                relativePath);

            var dirPath = Path.GetDirectoryName(completePath);
            var dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
                dir.Create();
        }

        string RegisterPending()
        {
            string ret = null;

            using (var db = CurrentApp.CurrentDb.CreateConnection())
            using (var cmd = db.CreateCommand())
            {
                var sql = "insert into {0} (uniqueId, fileContentType, pathPhysical, status, " +
                    "registered, p1Modified, instanceId) " +
                    "values (@a, @b, @c, @d, @today, @p1m, @ins);";

                cmd.CommandText = string.Format(sql, DodoDb.table_poweq2);

                var relativePath = string.Empty;
                CalculateFilePath(out ret, out relativePath);

                cmd.Parameters.AddWithValue("a", Item.UniqueId);
                cmd.Parameters.AddWithValue("b", Converter.ConvertedContentType);
                cmd.Parameters.AddWithValue("c", relativePath);
                cmd.Parameters.AddWithValue("d", EnumStatusPoweq2.PROCESSING.ToString());
                cmd.Parameters.AddWithValue("today", DateTime.Now);
                cmd.Parameters.AddWithValue("p1m", Item.modified);
                cmd.Parameters.AddWithValue("ins", CurrentApp.InstanceId);

                cmd.ExecuteNonQuery();
            }

            return ret;
        }

        void RegisterAvailable(string log)
        {
            using (var db = CurrentApp.CurrentDb.CreateConnection())
            using (var cmd = db.CreateCommand())
            {
                var sql = "update {0} set " +
                    "status = @status, log = @log " +
                    "where uniqueId = @id;";
                cmd.CommandText = string.Format(sql, DodoDb.table_poweq2);
                cmd.Parameters.AddWithValue("id", Item.UniqueId);
                cmd.Parameters.AddWithValue("status", EnumStatusPoweq2.AVAILABLE.ToString());

                int maxLen = 2000;
                if (log.Length > maxLen)
                    log = log.Substring(0, maxLen);

                cmd.Parameters.AddWithValue("log", log);
                cmd.ExecuteNonQuery();
            }
        }

        void TreatError(long uniqueId)
        {
            using (var db = CurrentApp.CurrentDb.CreateConnection())
            using (var cmd = db.CreateCommand())
            {
                var sql = "delete from {0} where uniqueId = @id;";
                cmd.CommandText = string.Format(sql, DodoDb.table_poweq2);
                cmd.Parameters.AddWithValue("id", uniqueId);
                cmd.ExecuteNonQuery();
            }
        }

        string GetOriginalFile()
        {
            string ret = null;

            ret = Path.GetTempFileName();
            ret = Path.ChangeExtension(ret, ".pdf");

            using (var fs = File.OpenWrite(ret))
            using (var db = CurrentApp.CurrentDb.CreateConnection())
            using (var cmd = db.CreateCommand())
            {
                var sql = "select boxBinary from {0} where uniqueId = @id";
                cmd.CommandText = string.Format(sql, DodoDb.table_poweq1);
                cmd.Parameters.AddWithValue("id", Item.UniqueId);
                var tmp = cmd.ExecuteScalar();

                if (tmp != null)
                {
                    byte[] fileTmp = (byte[])tmp;
                    fs.Write(fileTmp, 0, fileTmp.Length);
                }
            }

            return ret;
        }
    }
}
