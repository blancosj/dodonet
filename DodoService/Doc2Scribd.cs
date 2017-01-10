using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scribd.Net;

namespace DodoService
{
    public sealed class Doc2Scribd : DodoConverter
    {
        public override string AcceptedExt { get { return ".pdf"; } }

        public override string ConvertedExt { get { return ".sbd"; } }

        public Doc2Scribd()
        {
            // Set up credentials
            Service.APIKey = DodoService.Settings1.Default.ScribdApiKey;
            Service.SecretKey = DodoService.Settings1.Default.ScribdSecretKey;
            Service.PublisherID = DodoService.Settings1.Default.ScribdPublisherId;
            Service.EnforceSigning = DodoService.Settings1.Default.ScribdEnforceSigning;

        }

        public override void Convert(System.IO.Stream source, System.IO.Stream target, out string log)
        {
            throw new NotImplementedException();
        }

        public override void Convert(string pathSource, ref string pathTarget, out string log)
        {
            var sb = new StringBuilder();

            try
            {
                var userName = DodoService.Settings1.Default.ScribdUserName;

                User.LoginFailed +=
                    (object s, UserEventArgs x) => sb.AppendLine("Login failed " + userName);

                User.LoginFailed +=
                    (object s, UserEventArgs x) => sb.AppendLine("Login failed " + userName);

                User.Login(
                    userName, DodoService.Settings1.Default.ScribdPassword);

                Document _document =
                    Document.Upload(pathSource,
                        AccessTypes.Private, false);

                sb.AppendLine("Document was uploaded");

                _document.DisableAboutDialog = true;
                _document.DisableInfoDialog = true;
                _document.DisablePrint = true;
                _document.DisableRelatedDocuments = true;
                _document.DisableSelectText = true;
                _document.DisableUploadLink = true;
                _document.License = CCLicenseTypes.C;
                _document.Save();

                pathTarget = string.Format("document_id={0}&access_key={1}", 
                    _document.DocumentId, _document.AccessKey);

                sb.AppendLine("Ok");
            }
            catch (Exception err)
            {
                sb.AppendLine(err.Message);
            }

            log = sb.ToString();
        }
    }
}
