using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Common;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using DodoNet;
using DodoNet.Http;

using MySql.Data.MySqlClient;

namespace DodoService
{
    public class DodoDb : IDisposable
    {
        /// <summary>
        /// main table of pwq
        /// </summary>
        public static readonly string table_poweq1 = "poweq1";

        /// <summary>
        /// alternative format of same content
        /// </summary>
        public static readonly string table_poweq2 = "poweq2";

        /// <summary>
        /// rules content types
        /// </summary>
        public static readonly string table_rules_content_types = "rulescontenttypes";

        DodoApplication app;

        public object Sync = new object();

        public DodoDb(DodoApplication app)
        {
            this.app = app;
        }

        public DodoSession CheckUserAndPwd(string user, string pwd)
        {
            DodoSession ret = null;
            using (MySqlConnection conn = CreateConnection())
            {
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select userid, username, fullname from powequsers where username = ?user and password = ?pwd";
                    // cmd.Parameters.Add("user", user);
                    // cmd.Parameters.Add("user", pwd);
                    cmd.Parameters.AddWithValue("user", user);
                    cmd.Parameters.AddWithValue("pwd", pwd);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ret = new DodoSession(app, reader.GetInt32("userid"),
                                reader.GetString("username"),
                                reader.GetString("fullname"));
                        }
                    }
                }
            }
            return ret;
        }

        public DodoSession CheckEmail(string email)
        {
            DodoSession ret = null;
            using (MySqlConnection conn = CreateConnection())
            {
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select userid, username, fullname from powequsers where email = ?email";
                    cmd.Parameters.AddWithValue("email", email);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ret = new DodoSession(app, reader.GetInt32("userid"),
                                reader.GetString("username"),
                                reader.GetString("fullname"));
                        }
                    }
                }
            }
            return ret;
        }

        public string GetRecordType(string uniqueId)
        {
            string ret = string.Empty;

            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select type from poweq1 where uniqueId = @uniqueId";
                    cmd.Parameters.AddWithValue("uniqueId", uniqueId);
                    ret = cmd.ExecuteScalar() as string;
                }
            }

            return ret;
        }

        public long CountFolderChildren(string folderId)
        {
            long ret = 0;

            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select count(UniqueId) cc from poweq1 where folderId = @folderId";
                    cmd.Parameters.AddWithValue("folderId", folderId);
                    ret = Convert.ToInt64(cmd.ExecuteScalar());
                }
            }

            return ret;
        }

        public void RemoveRecord(string id)
        {
            bool go = false;
            var recType = GetRecordType(id);

            if (recType == "folder")
            {
                var c = CountFolderChildren(id);
                go = c == 0;
            }
            else
                go = true;

            if (go)
            {
                using (var conn = CreateConnection())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "delete from poweq1 where uniqueId = @uniqueId";
                        cmd.Parameters.AddWithValue("uniqueId", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                throw new Exception("FOLDER NOT EMPTY");
            }
        }

        public IEnumerable<DodoItem> GetLastItems(string userId, string typeItem)
        {
            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                var sql = "select uniqueId from {0}";
                sql += " where userId = @userId and type = @type";
                sql += " order by registered desc";
                sql += " limit 0, 10";

                cmd.CommandText = string.Format(sql, table_poweq1);

                cmd.Parameters.AddWithValue("userId", Convert.ToInt64(userId));
                cmd.Parameters.AddWithValue("type", typeItem);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return DodoItemBase.GetItem<DodoItem>(
                                app, reader.GetInt64(reader.GetOrdinal("uniqueId")));
                    }
                }
            }
        }

        public EnumStatusPoweq2 GetItem2Status(DodoItem item, string contenType)
        {
            var ret = EnumStatusPoweq2.NOT_EXISTS;

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                string sql = "select status, p1Modified from {0} "
                    + "where uniqueId = @uniqueId and fileContentType = @type";

                cmd.CommandText = string.Format(sql, table_poweq2);
                cmd.Parameters.AddWithValue("uniqueId", item.UniqueId);
                cmd.Parameters.AddWithValue("type", contenType);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var tmpStatus = reader.GetString(reader.GetOrdinal("status"));
                        ret = (EnumStatusPoweq2)Enum.Parse(typeof(EnumStatusPoweq2), tmpStatus);

                        if (ret == EnumStatusPoweq2.AVAILABLE && false)
                        {
                            var p1Mod = reader.GetDateTime(reader.GetOrdinal("p1Modified"));
                            if (item.modified != p1Mod)
                                ret = EnumStatusPoweq2.MODIFIED_ITEM;
                        }
                    }
                }
            }

            return ret;
        }

        public DodoItem2 GetItem2(DodoItem item, string contentType)
        {
            DodoItem2 ret = null;

            lock (Sync)
            {
                CheckInstanceOfItem2(item.UniqueId, contentType, app.InstanceId);

                using (var conn = CreateConnection())
                using (var cmd = conn.CreateCommand())
                {
                    var sql = "select boxBinary, registered, status, p1Modified, instanceId, pathPhysical from {0} "
                        + "where uniqueId = @uniqueId and "
                        + "fileContentType = @type";

                    cmd.CommandText = string.Format(sql, table_poweq2);
                    cmd.Parameters.AddWithValue("uniqueId", item.UniqueId);
                    cmd.Parameters.AddWithValue("type", contentType);

                    using (var reader = cmd.ExecuteReader())
                    {
                        bool gotCreateItem = false;

                        if (reader.Read())
                        {
                            var tmpStatus = reader.GetString(reader.GetOrdinal("status"));
                            var status = (EnumStatusPoweq2)Enum.Parse(typeof(EnumStatusPoweq2), tmpStatus);

                            switch (status)
                            {
                                case EnumStatusPoweq2.AVAILABLE:
                                    ret = new DodoItem2();
                                    ret.BoxBinary = SqlUtility.GetBytesFromBlob(reader, "boxBinary");

                                    var pathPhysical = reader.GetString(reader.GetOrdinal("pathPhysical"));
                                    //ret.FileName = Path.GetFileNameWithoutExtension(item.fileName) +
                                    //    Path.GetExtension(pathPhysical);
                                    ret.FileName = pathPhysical;
                                    ret.FileSize = ret.BoxBinary.Length;
                                    ret.FileContentType = contentType;
                                    ret.Registered = reader.GetDateTime(reader.GetOrdinal("registered"));
                                    break;
                                case EnumStatusPoweq2.PROCESSING:
                                    throw new ItemNotAvailableException();
                                case EnumStatusPoweq2.NOT_EXISTS:
                                case EnumStatusPoweq2.MODIFIED_ITEM:
                                    gotCreateItem = true;
                                    throw new ItemNotFoundException();
                            }
                        }
                        else
                            gotCreateItem = true;

                        if (gotCreateItem)
                        {
                            var task = new DodoConversion(item, app, 1000);
                            task.PrepareConversion();
                            app.LocalNode.SetTimer(task);

                            throw new ItemNotAvailableException();
                        }
                    }
                }
            }

            return ret;
        }

        public bool CheckInstanceOfItem2(long uniqueId, string contentType, string appInstance)
        {
            bool ret = false;

            using (var conn = CreateConnection())
            {
                var deleteItem2 = false;

                using (var cmd = conn.CreateCommand())
                {
                    var sql = "select count(*) cc from {0} "
                        + "where uniqueId = @uniqueId and "
                        + "fileContentType = @type and "
                        + "instanceId != @ins and "
                        + "status != @status";

                    cmd.CommandText = string.Format(sql, table_poweq2);
                    cmd.Parameters.AddWithValue("uniqueId", uniqueId);
                    cmd.Parameters.AddWithValue("type", contentType);
                    cmd.Parameters.AddWithValue("ins", app.InstanceId);
                    cmd.Parameters.AddWithValue("status", EnumStatusPoweq2.AVAILABLE.ToString());

                    var tmpCounter = cmd.ExecuteScalar();
                    var counter = Convert.ToInt64(tmpCounter);

                    deleteItem2 = counter > 0;
                }

                if (deleteItem2)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        var sql = "delete from {0} where uniqueId = @id and fileContentType = @type";

                        cmd.CommandText = string.Format(sql, table_poweq2);
                        cmd.Parameters.AddWithValue("id", uniqueId);
                        cmd.Parameters.AddWithValue("type", contentType);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return ret;
        }

        public DodoItem GetItem(long id)
        {
            DodoItem ret = DodoItemBase.GetItem<DodoItem>(app, id);
            return ret;
        }

        public DodoItem GetFolder(string id, DodoSession s)
        {
            DodoItem ret = null;
            if (id.CompareTo("-1") == 0)
            {
                // objeto nuevo
                ret = new DodoItem();
                ret.UniqueId = -1;
                ret.UserId = s.userId;
            }
            else
            {
                using (var conn = CreateConnection())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select uniqueId from poweq1 where uniqueId = @uniqueId";

                    cmd.Parameters.AddWithValue("uniqueId", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ret = DodoItemBase.GetItem<DodoItem>(
                                app, reader.GetInt64(reader.GetOrdinal("uniqueId")));
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// obtener el registro de un contenido
        /// </summary>
        /// <param name="id"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public DodoItem GetRecord(string id, DodoSession s)
        {
            DodoItem ret = null;

            if (id.CompareTo("-1") == 0)
            {
                // objeto nuevo
                ret = new DodoItem();
                ret.UniqueId = -1;
                ret.UserId = s.userId;
            }
            else
            {
                using (var conn = CreateConnection())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select " +
                            "uniqueId " +
                            "from poweq1 where uniqueId = @uniqueId";

                        // SqlUtility.AddParameter(cmd, "uniqueId", id);
                        cmd.Parameters.AddWithValue("uniqueId", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ret = DodoItemBase.GetItem<DodoItem>(
                                    app, reader.GetInt64(reader.GetOrdinal("uniqueId")));
                            }
                        }
                    }
                }
            }

            return ret;
        }

        public DodoItem GetRecord(long uniqueId)
        {
            DodoItem ret = null;
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select " +
                        "uniqueId, userId, keywords, modified, registered, type, fileName, " +
                        "fileContentType, fileSize, boxText, folderId, weight, visible " +
                        "from poweq1 " +
                        "where uniqueid = @uniqueId";

                    // SqlUtility.AddParameter(cmd, "uniqueId", uniqueId);
                    cmd.Parameters.AddWithValue("uniqueId", uniqueId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ret = DodoItemBase.GetItem<DodoItem>(
                                app, reader.GetInt64(reader.GetOrdinal("uniqueId")));
                        }
                    }
                }
            }
            return ret;
        }

        public DodoItemCollection SearchMyItems(string userId, string keySearch)
        {
            var ret = new DodoItemCollection();
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select uniqueId from poweq1 where " +
                        "visible = true and " +
                        "userid = ?userId and ";

                    if (keySearch.IndexOf("|") > 0)
                    {
                        keySearch = keySearch.Replace("|", @"\.*");
                        cmd.CommandText += string.Format("keywords regexp '{0}' ", keySearch);
                    }
                    else
                    {
                        cmd.CommandText += "keywords like concat('%', @keywords, '%') ";
                        cmd.Parameters.AddWithValue("keywords", keySearch);
                    }

                    cmd.CommandText += "order by keywords {0}";

                    cmd.CommandText = string.Format(cmd.CommandText,
                        DodoApplication.collateMySql);

                    cmd.Parameters.AddWithValue("userId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add(DodoItemBase.GetItem<DodoItem>(
                                app, reader.GetInt64(reader.GetOrdinal("uniqueId"))));
                        }
                    }
                }
            }
            return ret;
        }

        public DodoItemCollection GetItems(int skip, int take, string userId)
        {
            var ret = new DodoItemCollection();
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select uniqueId from {0} where " +
                        "visible = true and " +
                        "userid = ?userId ";

                    cmd.CommandText += "order by keywords {1} ";
                    cmd.CommandText += "limit {2}, {3} ";

                    cmd.CommandText = string.Format(cmd.CommandText,
                        DodoDb.table_poweq1,
                        DodoApplication.collateMySql,
                        skip, take);

                    cmd.Parameters.AddWithValue("userId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add(DodoItemBase.GetItem<DodoItem>(
                                app, reader.GetInt64(reader.GetOrdinal("uniqueId"))));
                        }
                    }
                }
            }
            return ret;
        }

        public List<DodoItem> GetFolders(DodoSession session, long folderId)
        {
            var ret = new List<DodoItem>();
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select uniqueId, keywords, userId " +
                        "from poweq1 " +
                        "where userId = @userId and type = 'folder' and folderId = @folderid " +
                        "order by keywords";

                    cmd.Parameters.AddWithValue("userId", session.userId);
                    cmd.Parameters.AddWithValue("folderid", folderId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add(DodoItemBase.GetItem<DodoItem>(
                                app, reader.GetInt64(reader.GetOrdinal("uniqueId"))));
                        }
                    }
                }
            }
            return ret;
        }

        public IEnumerable<DodoItem> GetRecordsFromFolder(long folderId)
        {
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select uniqueId from poweq1 a where " +
                        "a.folderId = ?folderId and visible = true ";
                    cmd.CommandText += "order by a.weight desc, a.keywords {0}";
                    cmd.CommandText = string.Format(cmd.CommandText, DodoApplication.collateMySql);

                    cmd.Parameters.AddWithValue("folderId", folderId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return DodoItemBase.GetItem<DodoItem>(
                                app, reader.GetInt64(reader.GetOrdinal("uniqueId")));
                        }
                    }
                }
            }
        }

        public DodoItemCollection SearchMyRecords(int userId, string keySearch, long folderId)
        {
            var ret = new DodoItemCollection();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select a.uniqueId " +
                    "from poweq1 a where " +
                    "a.userid = ?userId and " +
                    "a.keywords like concat('%', ?keywords, '%')";

                if (folderId > -1)
                    cmd.CommandText += " and a.folderId = ?folderId";

                cmd.CommandText += " order by a.weight desc, a.keywords {0}";

                cmd.CommandText = string.Format(cmd.CommandText, DodoApplication.collateMySql);

                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("keywords", keySearch);
                cmd.Parameters.AddWithValue("folderId", folderId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(DodoItemBase.GetItem<DodoItem>(
                            app, reader.GetInt64(reader.GetOrdinal("uniqueId"))));
                    }
                }
            }

            return ret;
        }

        public MySqlConnection CreateConnection()
        {
#if LOCAL
            string server = "127.0.0.1";
#else
            string server = "www.poweq.com";
#endif

            string c = "Server={0}; Uid={1}; Pwd={2}; Database={3}; Compress=true;";
            c = String.Format(c, server, "root", "a210977", "poweq");

            MySqlConnection ret = new MySqlConnection(c);
            ret.Open();
            return ret;
        }

        #region Instructions

        long GetCounter()
        {
            long ret = 0;

            using (var conn = CreateConnection())
            {
                lock (this.Sync)
                {
                    using (var cmdCC = conn.CreateCommand())
                    {
                        cmdCC.CommandText = string.Format("select max(UniqueId) from {0}",
                            "poweq1");

                        ret = Convert.ToInt64(cmdCC.ExecuteScalar());
                        ret++;
                    }
                }
            }

            return ret;
        }

        public bool SetVisibleRecord(long uniqueId, bool visible, DodoSession s)
        {
            bool ret = false;

            HttpContext context = DodoApplication.CurrentContext;

            int retErr = 0;
            int userId = ((DodoSession)context.Session).userId;

            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "update poweq1 set " +
                        "visible = @visible, " +
                        "modified = CURRENT_TIMESTAMP " +
                        "where uniqueId = @uniqueId";

                    cmd.Parameters.AddWithValue("visible", visible);
                    cmd.Parameters.AddWithValue("uniqueId", uniqueId);

                    retErr = cmd.ExecuteNonQuery();

                    ret = true;
                }
            }

            return ret;
        }

        public DodoItem ModifyRecordWithForm()
        {
            HttpContext context = DodoApplication.CurrentContext;

            long uniqueId = 0;
            int retErr = 0;
            int userId = ((DodoSession)context.Session).userId;

            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    if (context.Request.Form["h_uniqueId"] == "-1")
                    {
                        cmd.CommandText = "insert into poweq1 " +
                        "(UniqueId, userid, keywords, boxText, type, folderId, visible, registered, modified) values " +
                        "(@UniqueId, " +
                        "@userId, " +
                        "@keywords, " +
                        "@boxText, " +
                        "'text', " +
                        "@folderId, " +
                        "true, " +
                        "CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

                        // cmd.CommandText += "SELECT @@IDENTITY";

                        uniqueId = GetCounter();
                        cmd.Parameters.AddWithValue("UniqueId", uniqueId);
                        cmd.Parameters.AddWithValue("userId", userId);
                        cmd.Parameters.AddWithValue("keywords", context.Request.Form["keywords"]);
                        cmd.Parameters.AddWithValue("folderId", context.Request.Form["folderId"]);

                        string boxText = context.Request.Form["rte1"];
                        CleanText(ref boxText);
                        cmd.Parameters.AddWithValue("boxText", boxText);

                        // record = GetRecord((decimal)cmd.ExecuteScalar());
                        retErr = cmd.ExecuteNonQuery();
                        // para subir fichero
                        context.Request.Form["h_uniqueId"] = uniqueId.ToString();
                    }
                    else
                    {
                        cmd.CommandText = "update poweq1 set " +
                        "keywords = @keywords, " +
                        "boxText = @boxText, " +
                        "folderId = @folderId, " +
                        "modified = CURRENT_TIMESTAMP " +
                        "where uniqueId = @uniqueId";

                        uniqueId = Convert.ToInt64(context.Request.Form["h_uniqueId"]);

                        cmd.Parameters.AddWithValue("uniqueId", uniqueId);
                        cmd.Parameters.AddWithValue("keywords", context.Request.Form["keywords"]);
                        cmd.Parameters.AddWithValue("folderId", context.Request.Form["folderId"]);

                        string boxText = context.Request.Form["rte1"];
                        CleanText(ref boxText);
                        cmd.Parameters.AddWithValue("boxText", boxText);

                        retErr = cmd.ExecuteNonQuery();
                    }
                }
            }
            // grabar fichero
            ModifyRecordFile();
            // borramos el fichero si así se desea
            RemoveRecordFile();

            return GetRecord(uniqueId);
        }

        /// <summary>
        /// añadir y modificar registro
        /// </summary>
        /// <param name="record"></param>
        public void ModifyRecord(ref DodoItem record)
        {
            using (MySqlConnection conn = CreateConnection())
            {
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    //  && record.isNewItem
                    if (false)
                    {
                        cmd.CommandText = "insert into poweq1 " +
                        "(userId, keywords, boxText, type, registered, modified, visible) values " +
                        "(@userId, " +
                        "@keywords, " +
                        "@boxText, " +
                        "'text', " +
                        "@visible, " +
                        "CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

                        cmd.CommandText += "SELECT @@IDENTITY";

                        cmd.Parameters.AddWithValue("userId", record.UserId);
                        cmd.Parameters.AddWithValue("keywords", record.Keywords);
                        cmd.Parameters.AddWithValue("visible", record.Visible);

                        CleanText(ref record.boxText);
                        cmd.Parameters.AddWithValue("boxText", record.boxText);

                        // record = GetRecord((decimal)cmd.ExecuteScalar());
                    }
                    else
                    {
                        cmd.CommandText = "update poweq1 set " +
                        "keywords = @keywords, " +
                        "boxText = @boxText, " +
                        "modified = CURRENT_TIMESTAMP, " +
                        "visible = @visible " +
                        "where uniqueId = @uniqueId";

                        cmd.Parameters.AddWithValue("uniqueId", record.UniqueId);
                        cmd.Parameters.AddWithValue("keywords", record.Keywords);
                        cmd.Parameters.AddWithValue("visible", record.Visible);
                        CleanText(ref record.boxText);
                        cmd.Parameters.AddWithValue("boxText", record.boxText);

                        int retErr = cmd.ExecuteNonQuery();

                        record = GetRecord(record.UniqueId);
                    }
                }
            }
        }

        /// <summary>
        /// añadir y modificar registro
        /// </summary>
        /// <param name="record"></param>
        public void ModifyFolder(ref DodoItem folder, DodoSession s)
        {
            int retErr = 0;

            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    if (folder.UniqueId == -1)
                    {
                        cmd.CommandText = "insert into poweq1 " +
                        "(uniqueId, userId, folderId, keywords, type, visible, weight, registered, modified) values " +
                        "(@uniqueId, " +
                        "@userId, " +
                        "@folderId, " +
                        "@keywords, " +
                        "'folder', " +
                        "@visible, " +
                        "100, " +
                        "CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

                        // cmd.CommandText += "SELECT @@IDENTITY";

                        folder.UniqueId = GetCounter();

                        cmd.Parameters.AddWithValue("uniqueId", folder.UniqueId);
                        cmd.Parameters.AddWithValue("userId", folder.UserId);
                        cmd.Parameters.AddWithValue("keywords", folder.Keywords);
                        cmd.Parameters.AddWithValue("visible", folder.Visible);
                        cmd.Parameters.AddWithValue("folderId", folder.FolderId);

                        retErr = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = "update poweq1 set " +
                        "keywords = @keywords, " +
                        "folderId = @folderId, " +
                        "modified = CURRENT_TIMESTAMP, " +
                        "visible = @visible " +
                        "where uniqueId = @uniqueId";

                        cmd.Parameters.AddWithValue("uniqueId", folder.UniqueId);
                        cmd.Parameters.AddWithValue("keywords", folder.Keywords);
                        cmd.Parameters.AddWithValue("folderId", folder.FolderId);
                        cmd.Parameters.AddWithValue("visible", folder.Visible);

                        retErr = cmd.ExecuteNonQuery();
                    }

                    folder = GetFolder(folder.UniqueId.ToString(), s);
                }
            }
        }

        public void RemoveRecordFile()
        {
            var context = DodoApplication.CurrentContext;

            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    // subir ficheros
                    if (context.Request.Form["h_remove_file_checkbox"] == "S")
                    {
                        cmd.CommandText = "update poweq1 set " +
                        "boxBinary = null, " +
                        "imageMini = null, " +
                        "fileName = null, " +
                        "fileContentType = null, " +
                        "fileSize = null, " +
                        "type = @type, " +
                        "modified = CURRENT_TIMESTAMP " +
                        "where uniqueId = @uniqueId";

                        cmd.Parameters.AddWithValue("type", "text");
                        cmd.Parameters.AddWithValue("uniqueId", context.Request.Form["h_uniqueId"]);

                        int retErr = cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// añadir registro
        /// </summary>
        /// <param name="record"></param>
        public void ModifyRecordFile()
        {
            var context = DodoApplication.CurrentContext;

            var fa = context.Request.Files["upload_file_name"];

            // subir ficheros
            if (fa != null)
            {
                using (var fs = new FileStream(fa.FileTmp, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length > 2)
                    {
                        byte[] buffer = new byte[fs.Length];
                        int c = fs.Read(buffer, 0, buffer.Length);

                        using (var conn = CreateConnection())
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "update poweq1 set " +
                            "imageMini = @imageMini, " +
                            "fileName = @fileName, " +
                            "fileContentType = @fileContentType, " +
                            "fileSize = @fileSize, " +
                            "type = @type, " +
                            "modified = CURRENT_TIMESTAMP " +
                            "where uniqueId = @uniqueId";

                            cmd.Parameters.AddWithValue("uniqueId", context.Request.Form["h_uniqueId"]);
                            cmd.Parameters.AddWithValue("boxBinary", buffer);

                            string faName = fa.Name.Replace(" ", "_");

                            cmd.Parameters.AddWithValue("fileName", faName);
                            cmd.Parameters.AddWithValue("fileContentType", fa.ContentType);

                            if (fa.ContentType.StartsWith("image"))
                            {
                                using (var img = Image.FromStream(fs))
                                using (var imgMini = ImageResize.ConstrainProportions(img, 180, ImageResize.Dimensions.Width))
                                using (var tmpMini = new MemoryStream())
                                {
                                    imgMini.Save(tmpMini, ImageFormat.Jpeg);
                                    byte[] bufferMini = new byte[tmpMini.Length];
                                    tmpMini.Position = 0;
                                    c = tmpMini.Read(bufferMini, 0, bufferMini.Length);
                                    cmd.Parameters.AddWithValue("imageMini", bufferMini);
                                }
                                cmd.Parameters.AddWithValue("type", "image");
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("imageMini", new byte[0]);
                                cmd.Parameters.AddWithValue("type", "file");
                            }

                            cmd.Parameters.AddWithValue("fileSize", buffer.Length);
                            int retErr = cmd.ExecuteNonQuery();
                        }

                        UpdateLargeFile(buffer);
                    }
                }
            }
        }

        void UpdateLargeFile(byte[] fileContent)
        {
            var context = DodoApplication.CurrentContext;

            var sql = "update poweq1 set " +
                "boxBinary = @boxBinary, " +
                "modified = CURRENT_TIMESTAMP " +
                "where uniqueId = @uniqueId";

            var fieldName = "boxBinary";

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;

                cmd.CommandTimeout = 300;

                cmd.Parameters.AddWithValue("uniqueId", context.Request.Form["h_uniqueId"]);
                cmd.Parameters.AddWithValue(fieldName, fileContent);

                int retErr = cmd.ExecuteNonQuery();
            }
        }

        /// añadir registro
        /// </summary>
        /// <param name="record"></param>
        public Stream GetRecordFile(string id)
        {
            MemoryStream ret = null;

            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select " +
                    "boxBinary, " +
                    "where uniqueId = @uniqueId and type = 'file'";

                    cmd.Parameters.AddWithValue("uniqueId", id);

                    ret = new MemoryStream((byte[])cmd.ExecuteScalar());
                }
            }
            return ret;
        }

        /// <summary>
        /// limpiar registro
        /// </summary>
        /// <param name="text"></param>
        public void CleanText(ref string text)
        {
            text = text.Replace("\"", "'");
            text = text.Trim();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
