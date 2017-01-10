using System;
using System.Collections.Generic;
using System.Text;

using DodoNet.Web;

namespace DodoService
{
    public class DodoSession : WebSession
    {
        public string fullname;
        public string userName;
        public int userId;

        public DodoSession(DodoApplication app)
            : base(app) { }

        public DodoSession(DodoApplication app, int userId, string userName, string fullname)
            : base(app)
        {
            this.fullname = fullname;
            this.userId = userId;
            this.userName = userName;
        }
    }
}
