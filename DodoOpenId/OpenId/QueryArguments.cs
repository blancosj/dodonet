using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoOpenId.OpenId
{
    public sealed class QueryArguments
    {
        public ErrorCodes ErrorCode = new ErrorCodes();
        public SessionTypes SessionType = new SessionTypes();
        public SignatureAlgorithms SignatureAlgorithm = new SignatureAlgorithms();
        public Modes Mode = new Modes();
        public IsValidValues IsValid = new IsValidValues();

        public sealed class ErrorCodes
        {
            public string UnsupportedType = "unsupported-type";
        }

        public sealed class SessionTypes
        {
            /// <summary>
            /// A preference order list of all supported session types.
            /// </summary>
            public string[] All { get { return new[] { DH_SHA512, DH_SHA384, DH_SHA256, DH_SHA1, NoEncryption }; } }
            public string[] AllDiffieHellman { get { return new[] { DH_SHA512, DH_SHA384, DH_SHA256, DH_SHA1 }; } }
            public string DH_SHA1 = "DH-SHA1";
            public string DH_SHA256;
            public string DH_SHA384;
            public string DH_SHA512;
            public string NoEncryption = string.Empty;
            public string Best
            {
                get
                {
                    foreach (string algorithmName in All)
                    {
                        if (algorithmName != null)
                        {
                            return algorithmName;
                        }
                    }
                    throw new ProtocolException(); // really bad... we have no signing algorithms at all
                }
            }
        }

        public sealed class SignatureAlgorithms
        {
            /// <summary>
            /// A preference order list of signature algorithms we support.
            /// </summary>
            public string[] All { get { return new[] { HMAC_SHA512, HMAC_SHA384, HMAC_SHA256, HMAC_SHA1 }; } }
            public string HMAC_SHA1 = "HMAC-SHA1";
            public string HMAC_SHA256;
            public string HMAC_SHA384;
            public string HMAC_SHA512;
            public string Best
            {
                get
                {
                    foreach (string algorithmName in All)
                    {
                        if (algorithmName != null)
                        {
                            return algorithmName;
                        }
                    }
                    throw new ProtocolException(); // really bad... we have no signing algorithms at all
                }
            }
        }

        public sealed class Modes
        {
            public string cancel = "cancel";
            public string error = "error";
            public string id_res = "id_res";
            public string checkid_immediate = "checkid_immediate";
            public string checkid_setup = "checkid_setup";
            public string check_authentication = "check_authentication";
            public string associate = "associate";
            public string setup_needed = "id_res"; // V2 overrides this
        }

        public sealed class IsValidValues
        {
            public string True = "true";
            public string False = "false";
        }
    }
}
