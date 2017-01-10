using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.Script.Serialization;
using DodoNet.Http;
using DodoNet.Tools;

using MySql.Data.MySqlClient;

namespace DodoService
{
    [Serializable]
    public class DodoItemBase
    {
        /// <summary>
        /// identificador unico
        /// </summary>
        public long UniqueId { get; set; }
        /// <summary>
        /// propietario del item
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// última fecha de modificacion
        /// </summary>
        public DateTime modified;
        /// <summary>
        /// fecha de alta
        /// </summary>
        public DateTime registered;
        /// <summary>
        /// palabras clave
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// tipo de contenido
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// nombre del fichero del contenido
        /// </summary>
        public string fileName;
        /// <summary>
        /// tipo de fichero del contenido
        /// </summary>
        public string fileContentType;
        /// <summary>
        /// tamaño del fichero de contenido
        /// </summary>
        public long fileSize;
        /// <summary>
        /// texto del contenido
        /// </summary>
        // [NonSerialized]
        public string boxText = "";
        /// <summary>
        /// imagen thumbnail del contenido
        /// </summary>
        [ScriptIgnore]
        [NonSerialized]
        public byte[] imageMini = new byte[0];
        /// <summary>
        /// fichero del contenido
        /// </summary>
        [ScriptIgnore]
        [NonSerialized]
        public byte[] boxBinary = new byte[0];
        /// <summary>
        /// tipo de imagen thumbnail del contenido
        /// </summary>
        public string imageMiniContentType = "image/jpeg";
        /// <summary>
        /// codigo de incrustado
        /// </summary>
        public string EmbeddedCode { get; set; }
        /// <summary>
        /// dirección url para acceder al contenido
        /// </summary>
        public string UrlCode { get; set; }
        /// <summary>
        /// etiqueta html para incrustar la imagen del contenido
        /// </summary>
        public string miniImageElementHml = "";
        /// <summary>
        /// etiqueta html para incrustar el fichero del contenido
        /// </summary>
        public string fileElementHtml = "";
        /// <summary>
        /// visible
        /// </summary>
        public bool Visible = true;

        public long FolderId { get; set; }

        public bool BelongsToFolder;

        public bool HasFile;

        [NonSerialized]
        [ScriptIgnore]
        public bool isFileLoaded = false;

        [ScriptIgnore]
        public DodoApplication App { get; set; }

        public static T GetItem<T>(DodoApplication app, long uniqueId)
            where T : DodoItemBase, new()
        {
            T ret = null;
            ret = new T();
            ret.UniqueId = uniqueId;
            ret.App = app;
            ret.ReadValues();
            return ret;
        }

        public DodoItemBase()
        {
            Type = "text";
            modified = DateTime.Now;
            registered = DateTime.Now;
        }

        public void ReadValues()
        {
            using (var conn = App.CurrentDb.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                var sql = "select userId, keywords, modified, registered,"
                    + " type, fileName, fileContentType, fileSize, boxText, folderId,"
                    + " weight, visible"
                    + " from {0} where uniqueId = @uniqueId";
                cmd.CommandText = string.Format(sql, DodoDb.table_poweq1);

                cmd.Parameters.AddWithValue("uniqueId", UniqueId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var table = reader.GetSchemaTable();
                        table.PrimaryKey = new DataColumn[] { table.Columns["columnName"] };
                        var rows = table.Rows;

                        if (rows.Contains("userId") && !reader.IsDBNull(reader.GetOrdinal("userId")))
                            UserId = reader.GetInt32(reader.GetOrdinal("userId"));

                        if (rows.Contains("keywords") && !reader.IsDBNull(reader.GetOrdinal("keywords")))
                            Keywords = reader.GetString(reader.GetOrdinal("keywords"));

                        if (rows.Contains("modified") && !reader.IsDBNull(reader.GetOrdinal("modified")))
                            modified = reader.GetDateTime(reader.GetOrdinal("modified"));

                        if (rows.Contains("registered") && !reader.IsDBNull(reader.GetOrdinal("registered")))
                            registered = reader.GetDateTime(reader.GetOrdinal("registered"));

                        if (rows.Contains("folderId") && !reader.IsDBNull(reader.GetOrdinal("folderId")))
                            FolderId = reader.GetInt32(reader.GetOrdinal("folderId"));

                        if (rows.Contains("visible") && !reader.IsDBNull(reader.GetOrdinal("visible")))
                            Visible = reader.GetBoolean(reader.GetOrdinal("visible"));

                        if (rows.Contains("type") && !reader.IsDBNull(reader.GetOrdinal("type")))
                        {
                            Type = reader.GetString(reader.GetOrdinal("type")).Trim();
                            switch (Type)
                            {
                                case "folder":
                                case "text":
                                case "image":
                                case "file":
                                    if (rows.Contains("fileName") && !reader.IsDBNull(reader.GetOrdinal("fileName")))
                                    {
                                        fileName = reader.GetString(reader.GetOrdinal("fileName"));
                                        fileElementHtml = GetMiniImageElementHtml();
                                    }

                                    if (rows.Contains("fileContentType") && !reader.IsDBNull(reader.GetOrdinal("fileContentType")))
                                        fileContentType = reader.GetString(reader.GetOrdinal("fileContentType"));

                                    if (rows.Contains("boxText") && !reader.IsDBNull(reader.GetOrdinal("boxText")))
                                        boxText = reader.GetString(reader.GetOrdinal("boxText"));

                                    if (rows.Contains("fileSize") && !reader.IsDBNull(reader.GetOrdinal("fileSize")))
                                        fileSize = reader.GetInt32(reader.GetOrdinal("fileSize"));

                                    if (rows.Contains("imageMini") && !reader.IsDBNull(reader.GetOrdinal("imageMini")))
                                        miniImageElementHml = GetMiniImageElementHtml();

                                    // only into app
                                    if (DodoApplication.CurrentContext != null)
                                    {
                                        EmbeddedCode = GetEmbeddedCode();
                                        UrlCode = GetUrlCode();
                                    }

                                    HasFile = !string.IsNullOrEmpty(fileName);
                                    BelongsToFolder = FolderId > 0;

                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void ReadBlobValues()
        {
            if (!isFileLoaded)
            {
                isFileLoaded = true;

                using (var conn = App.CurrentDb.CreateConnection())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select boxBinary, imageMini"
                        + " from poweq1 where uniqueId = @uniqueId";
                    cmd.Parameters.AddWithValue("uniqueId", UniqueId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var table = reader.GetSchemaTable();
                            table.PrimaryKey = new DataColumn[] { table.Columns["columnName"] };
                            var rows = table.Rows;

                            switch (Type)
                            {
                                case "folder":
                                case "text":
                                case "image":
                                case "file":
                                    if (rows.Contains("boxbinary") && !reader.IsDBNull(reader.GetOrdinal("boxbinary")))
                                        boxBinary = SqlUtility.GetBytesFromBlob(reader, "boxbinary");

                                    if (rows.Contains("imageMini") && !reader.IsDBNull(reader.GetOrdinal("imageMini")))
                                        imageMini = SqlUtility.GetBytesFromBlob(reader, "imageMini");

                                    break;
                            }
                        }
                    }
                }
            }
        }

        public string GetFileElementHtml()
        {
            StringBuilder ret = new StringBuilder();
            if (Type == "file")
            {
                ret.AppendFormat("<a href='{0}filedownload.dodo?code={1}'>Download {2}</img></a>",
                    GetUrlBase(),
                    UniqueId,
                    fileName);
            }
            return ret.ToString();
        }

        public string GetFileUrl()
        {
            StringBuilder ret = new StringBuilder();
            if (Type == "file")
            {
                ret.AppendFormat("{0}filedownload.dodo?code={1}",
                    GetUrlBase(),
                    UniqueId,
                    fileName);
            }
            return ret.ToString();
        }

        public string GetMiniImageElementHtml()
        {
            StringBuilder ret = new StringBuilder();
            if (Type == "image")
            {
                ret.AppendFormat("<a href='{0}fileget.dodo?code={1}'>" +
                    "<img src='{0}fileget.dodo?code={1}&mini=yes' title='{2}' style='border-width: 0px;'></img>" +
                    "</a>",
                    GetUrlBase(),
                    UniqueId,
                    Keywords);
            }
            return ret.ToString();
        }

        public string GetUniqueIdLong()
        {
            return Conversion.Num2Don(UniqueId * DodoApplication.MultiNumForId);
        }

        public string GetEmbeddedCode()
        {
            StringBuilder ret = new StringBuilder();

            ret.AppendFormat("<script src='{0}{1}?code=~{2}'></script>",
                GetUrlBase(),
                "getmethod",
                GetUniqueIdLong());

            return System.Web.HttpUtility.HtmlEncode(ret.ToString());
        }

        public string GetUrlCode()
        {
            StringBuilder ret = new StringBuilder();

            ret.AppendFormat("{0}{1}?code=~{2}",
                GetUrlBase(),
                "fileget.dodo",
                GetUniqueIdLong());

            return System.Web.HttpUtility.HtmlEncode(ret.ToString());
        }

        private string GetUrlBase()
        {
            var context = DodoApplication.CurrentContext;
            var ret = string.Empty;

            string path = "";
            int pos = 0;
            if ((pos = context.Request.Url.LastIndexOf("/")) > 0)
            {
                path = context.Request.Url.Substring(0, pos);
            }

            string shorterHost = ((DodoApplication)context.App).ShorterServerUrl;
            string host = shorterHost ?? context.Request.Host;

            //host = "127.0.0.1";
            //int port = 8080;
            int port = 0;

            if (string.IsNullOrEmpty(shorterHost))
            {
                if ((pos = context.Request.Host.LastIndexOf(":")) > 0)
                {
                    host = context.Request.Host.Substring(0, pos);
                    port = int.Parse(context.Request.Host.Substring(pos + 1));
                }
            }

            var uri = new UriBuilder(Uri.UriSchemeHttp, host);
            if (port > 0)
                uri.Port = port;
            uri.Path = path;
            ret = uri.Uri.ToString();
            return ret;
        }
    }
}
