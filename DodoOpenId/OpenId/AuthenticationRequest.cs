using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet;
using DodoNet.Http;

namespace DodoOpenId.OpenId
{
    public class AuthenticationRequest
    {
        #region Properties

        public HttpRequest Request { get; private set; }

        public XrdsDocument XrdsDocument { get; private set; }

        /// <summary>
        /// (required) Interaction mode. Specifies whether Google may interact with the user to determine the outcome of the request. Valid values are:
        /// "checkid_immediate" (No interaction allowed)
        /// "checkid_setup" (Interaction allowed)
        /// Google supports an enhancement to checkid_immediate requests via the UI extension. If the request includes "openid.ui.mode=x-has-session", it will be echoed in the response only if Google detects an authenticated session.
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// (required) Protocol version. Value identifying the OpenID protocol version being used. This value should be "http://specs.openid.net/auth/2.0".
        /// </summary>
        public string Ns { get; set; }

        /// <summary>
        /// (required) Return URL. Value indicating the URL where the user should be returned to after signing in. Google supports HTTP and HTTPS address types; addresses that differ only in HTTP or HTTPS are considered the same address.
        /// </summary>
        public string ReturnTo { get; set; }

        /// <summary>
        /// (optional) Association handle. Set if an association was established between the relying party (web application) and the identity provider (Google). See OpenID specification Section 8.
        /// </summary>
        public string AssocHandle { get; set; }

        /// <summary>
        /// (optional) Claimed identifier. This value must be set to "http://specs.openid.net/auth/2.0/identifier_select".
        /// </summary>
        public string ClaimedId { get; set; }

        /// <summary>
        /// (optional) Alternate identifier. This value must be set to "http://specs.openid.net/auth/2.0/identifier_select".
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// (optional) Authenticated realm. Identifies the domain that the end user is being asked to trust. (Example: "http://*.myexamplesite.com") This value must be consistent with the domain defined in openid.return_to. If this parameter is not defined, Google will use the URL referenced in openid.return_to.
        /// The value of realm is used on the Google Federated Login page to identify the requesting site to the user. It is also used to determine the value of the persistent user ID returned by Google.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// (required) Identifies the extension protocol being used. This value should be "http://specs.openid.net/extensions/pape/1.0".
        /// </summary>
        public string PapeNs { get; set; }

        /// <summary>
        /// (optional) Sets the maximum acceptable time (in seconds) since the user last authenticated. If the session is older, the user will be prompted to log in again. Setting the value to zero will force a password reprompt regardless of session age.
        /// </summary>
        public string PapeMaxAuthAge { get; set; }

        /// <summary>
        /// (required) Indicates that the OpenID provider's authentication pages will be displayed in an alternative user interface This parameter must be set to "http://specs.openid.net/extensions/ui/1.0".
        /// </summary>
        public string UiNs { get; set; }

        /// <summary>
        /// (optional) Specifies the alternative user interface. The following values are valid:
        /// "popup"
        /// "x-has-session" (used to indicate the presence of an authenticated session)
        /// Additional modes may be supported in the future.
        /// </summary>
        public string UiMode { get; set; }

        /// <summary>
        /// (optional) Displays the favicon of the referring domain in the OpenID approval page if set to "true".
        /// </summary>
        public bool UiIcon { get; set; }

        /// <summary>
        /// (required) Indicates request for user attribute information. This value must be set to "http://openid.net/srv/ax/1.0". 
        /// </summary>
        public string AxNs { get; set; }

        /// <summary>
        /// (required) This value must be set to "fetch_request".
        /// </summary>
        public string AxMode { get; set; }

        /// <summary>
        /// (required) Specifies the attribute being requested. Valid values include:
        /// "country"
        /// "email"
        /// "firstname"
        /// "language"
        /// "lastname"
        /// To request multiple attributes, set this parameter to a comma-delimited list of attributes.
        /// </summary>
        public string AxRequired { get; set; }

        /// <summary>
        /// (optional) Requests the user's home country. This value must be set to "http://axschema.org/contact/country/home".
        /// </summary>
        public string AxTypeCountry { get; set; }

        /// <summary>
        /// (optional) Requests the user's gmail address. This value must be set to either "http://axschema.org/contact/email" or "http://schema.openid.net/contact/email".
        /// </summary>
        public string AxTypeEmail { get; set; }

        /// <summary>
        /// (optional) Requests the user's first name. This value must be set to "http://axschema.org/namePerson/first".
        /// </summary>
        public string AxTypeFirstName { get; set; }

        /// <summary>
        /// (optional) Requests the user's preferred language. This value must be set to "http://axschema.org/pref/language".
        /// </summary>
        public string AxTypeLanguage { get; set; }

        /// <summary>
        /// (optional) Requests the user's last name. This value must be set to "http://axschema.org/namePerson/last".
        /// </summary>
        public string AxTypeLastName { get; set; }

        /// <summary>
        /// (required) Indicates request for an OAuth token. This value must be set to "http://specs.openid.net/extensions/oauth/1.0"
        /// </summary>
        public string Ext2Ns { get; set; }

        /// <summary>
        /// (required) Consumer key provided by Google after registering the site. This is typically a DNS domain name. Must be consistent with the value for realm (for example, realm = "http://www.example.com" and ext2.consumer = "www.example.com", or realm = "http://*.somedomain.com" and ext2.consumer = "www.somedomain.com").
        /// </summary>
        public string Ext2Consumer { get; set; }

        /// <summary>
        /// (required) List of URLs identifying the Google service(s) to be accessed. See documentation for the services of interest to get the appropriate scope values. Lists of two or more scopes must be space-delimited and properly escaped. This parameter is not defined in the OAuth standards; it is a Google-specific parameter.
        /// </summary>
        public string Ext2Scope { get; set; }

        #endregion

        public AuthenticationRequest(DiscoverRequest discover, 
            string returnTo, string sessionId, string realm, Node localNode)
        {
            XrdsDocument = discover.XrdsDocument;

            Request = new HttpRequest(localNode);
            Request.Url = discover.ProviderEndpoint.ToString();
            //-- Request.Url = "https://www.google.com/accounts/o8/id";

            Mode = "checkid_setup";
            Ns = "http://specs.openid.net/auth/2.0";
            AssocHandle = sessionId;
            ClaimedId = "http://specs.openid.net/auth/2.0/identifier_select";
            Identity = "http://specs.openid.net/auth/2.0/identifier_select";
            ReturnTo = returnTo;
            Realm = realm;
            UiNs = "http://specs.openid.net/extensions/ui/1.0";
            UiMode = "x-has-session";
            UiIcon = true;
            AxNs = "http://openid.net/srv/ax/1.0";
            AxMode = "fetch_request";
            AxTypeEmail = "http://axschema.org/contact/email";
            AxTypeLanguage = "http://axschema.org/pref/language";
            AxRequired = "email,language";

            /*
            PapeNs = "http://specs.openid.net/extensions/pape/1.0";
            PapeMaxAuthAge = "50";
            UiNs = "http://specs.openid.net/extensions/ui/1.0";
            UiMode = "x-has-session";
            UiIcon = true;
            AxNs = "http://openid.net/srv/ax/1.0";
            AxMode = "fetch_request";
            AxRequired = "email";
            AxTypeCountry = "http://axschema.org/contact/country/home";
            AxTypeEmail = "http://axschema.org/contact/email";
            AxTypeFirstName = "http://axschema.org/namePerson/first";
            AxTypeLanguage = "http://axschema.org/pref/language";
            AxTypeLastName = "http://axschema.org/namePerson/last";
            */
        }

        public string UrlRedirectTo()
        {
            var sb = new StringBuilder();

            //-- AssocHandle = "{HMAC-SHA1}{4abdf2f1}{olw8ag==}";

            sb.AppendFormat("{0}?", Request.Url);
            sb.AppendFormat("openid.mode={0}&", ConvertUrlValue(Mode));
            sb.AppendFormat("openid.ns={0}&", ConvertUrlValue(Ns));
            sb.AppendFormat("openid.return_to={0}&", ConvertUrlValue(ReturnTo));
            sb.AppendFormat("openid.assoc_handle={0}&", ConvertUrlValue(AssocHandle));
            sb.AppendFormat("openid.claimed_id={0}&", ConvertUrlValue(ClaimedId));
            sb.AppendFormat("openid.identity={0}&", ConvertUrlValue(Identity));
            sb.AppendFormat("openid.realm={0}&", ConvertUrlValue(Realm));
            sb.AppendFormat("openid.ns.ui={0}&", ConvertUrlValue(UiNs));
            sb.AppendFormat("openid.ui.mode={0}&", ConvertUrlValue(UiMode));
            sb.AppendFormat("openid.ui.icon={0}&", ConvertUrlValue(UiIcon));
            sb.AppendFormat("openid.ns.ax={0}&", ConvertUrlValue(AxNs));
            sb.AppendFormat("openid.ax.mode={0}&", ConvertUrlValue(AxMode));
            sb.AppendFormat("openid.ax.required={0}&", ConvertUrlValue(AxRequired));
            sb.AppendFormat("openid.ax.type.email={0}&", ConvertUrlValue(AxTypeEmail));
            sb.AppendFormat("openid.ax.type.language={0}&", ConvertUrlValue(AxTypeLanguage));

            //-- sb.AppendFormat("openid.sign={0}&", "Xl94j3IJtfSEQ4oKfova68I8edc");
            //-- sb.AppendFormat("openid.signed={0}&", "identity");

            return sb.ToString();
        }

        public string ConvertUrlValue(object value)
        {
            return global::System.Web.HttpUtility.UrlEncode(value.ToString());
        }

        public void PrepareSend()
        {
            Request.Form.Add("openid.mode", Mode);
            Request.Form.Add("openid.ns", Ns);
            Request.Form.Add("openid.return_to", ReturnTo);
            Request.Form.Add("openid.assoc_handle", AssocHandle);
            Request.Form.Add("openid.claimed_id", ClaimedId);
            Request.Form.Add("openid.identity", Identity);
            Request.Form.Add("openid.realm", Realm);

            /*
            Request.Form.Add("openid.ns.pape", PapeNs);
            Request.Form.Add("opendid.pape.max_auth_age", PapeMaxAuthAge);
            Request.Form.Add("openid.ns.ui", UiNs);
            Request.Form.Add("openid.ui.mode", UiMode);
            Request.Form.Add("openid.ns.ax", AxNs);
            Request.Form.Add("openid.ax.required", AxRequired);
            Request.Form.Add("openid.ax.type.email", AxTypeEmail);
            Request.Form.Add("openid.ax.type.firstname", AxTypeFirstName);
            Request.Form.Add("openid.ax.type.language", AxTypeLanguage);
            Request.Form.Add("openid.ax.type.lastname", AxTypeLastName);
            Request.Form.Add("openid.ns.ext2", Ext2Ns);
            Request.Form.Add("openid.ext2.consumer", Ext2Consumer);
            Request.Form.Add("openid.ext2.scope", Ext2Scope);
            */
        }

        public void CheckSessionOpenId(HttpReply reply)
        {

        }
    }
}
