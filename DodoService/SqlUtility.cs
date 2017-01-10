using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;

using MySql.Data.MySqlClient;

namespace DodoService
{
    public class SqlUtility
    {
        public static byte[] GetBytesFromBlob(MySqlDataReader reader, string fieldName)
        {
            byte[] ret = null;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                long pos = 0, y = 0;
                bool exit = false;
                int iCol = reader.GetOrdinal(fieldName);

                for (; ; )
                {
                    try
                    {
                        y = reader.GetBytes(iCol, pos, buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, (int)y);
                        pos += y;

                        exit = y < buffer.Length;

                        if (exit) break;
                    }
                    catch { break; }
                }
                ret = ms.ToArray();
            }
            return ret;
        }

        public static void AddParameter(SqlCommand cmd, string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
                cmd.Parameters.Add(new SqlParameter(key, value));
            else
                cmd.Parameters.Add(new SqlParameter(key, ""));
        }

        public static void AddParameter(SqlCommand cmd, string key, bool value)
        {
            cmd.Parameters.Add(new SqlParameter(key, value));
        }

        public static void AddParameter(SqlCommand cmd, string key, DateTime value)
        {
            cmd.Parameters.Add(new SqlParameter(key, value));
        }

        public static void AddParameter(SqlCommand cmd, string key, int value)
        {
            cmd.Parameters.Add(new SqlParameter(key, value));
        }

        public static void AddParameter(SqlCommand cmd, string key, decimal value)
        {
            cmd.Parameters.Add(new SqlParameter(key, value));
        }

        public static void AddParameter(SqlCommand cmd, string key, byte[] value)
        {
            cmd.Parameters.Add(new SqlParameter(key, value));
        }

        public static void AddParameterBool(SqlCommand cmd, string key, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                cmd.Parameters.Add(new SqlParameter(key, false));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter(key, true));
            }
        }

        public static void AddParameterInt(SqlCommand cmd, string key, string value)
        {
            int tmp = 0;
            try
            {
                tmp = int.Parse(value);
                cmd.Parameters.Add(new SqlParameter(key, tmp));
            }
            catch
            {
                cmd.Parameters.Add(new SqlParameter(key, tmp));
            }
        }
    }
}
