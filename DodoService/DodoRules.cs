using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoService
{
    public sealed class DodoRules
    {
        DodoApplication app;

        public DodoRules(DodoApplication app)
        {
            this.app = app;
        }

        /// <summary>
        /// any content type mandatory for specific content type of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public bool CheckDeniedRules(DodoEnumRules rules, DodoItem item)
        {
            bool ret = false;

            using (var conn = app.CurrentDb.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                string sql = "select denyOptions from {0} "
                    + "where userId = @userId and "
                    + "fileContentTypeIn = @type";

                cmd.CommandText = string.Format(sql, DodoDb.table_rules_content_types);

                cmd.Parameters.AddWithValue("userId", item.UserId);
                cmd.Parameters.AddWithValue("type", item.fileContentType);

                var tmpRet = cmd.ExecuteScalar();
                var tmpRules = Convert.ToString(tmpRet);

                ret = tmpRules.IndexOf(rules.ToString()) > -1;
            }

            return ret;
        }

        /// <summary>
        /// any content type mandatory for specific content type of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public bool AnyContentTypeMandatory(DodoItem item, out string newContentType)
        {
            bool ret = false;

            using (var conn = app.CurrentDb.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                string sql = "select fileContentTypeOut from {0} "
                    + "where userId = @userId and "
                    + "fileContentTypeIn = @type";

                cmd.CommandText = string.Format(sql, DodoDb.table_rules_content_types);

                cmd.Parameters.AddWithValue("userId", item.UserId);
                cmd.Parameters.AddWithValue("type", item.fileContentType);

                var tmpRet = cmd.ExecuteScalar();
                newContentType = Convert.ToString(tmpRet);

                ret = !string.IsNullOrEmpty(newContentType);
            }

            return ret;
        }
    }
}
