using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DodoNet.Extensions;

namespace DodoService
{
    public class Pdf2Flash : DodoConverter
    {
        public override string AcceptedExt { get { return ".pdf"; } }

        public override string ConvertedExt { get { return ".swf"; } }

        public string PathApp { get; set; }

        public override void Convert(Stream source, Stream target, out string log)
        {
            log = string.Empty;
            source.CopyTo(target);
        }

        public override void Convert(string pathSource, ref string pathTarget, out string log)
        {
            var source = new FileInfo(pathSource);
            var target = new FileInfo(pathTarget);

            DodoNet.Node.LogAppendLine("Print2Flash - Source {0}", pathSource);
            DodoNet.Node.LogAppendLine("Print2Flash - Target {0}", pathTarget);

            if (!source.Exists)
                throw new FileNotFoundException(pathSource);

            if (target.Exists)
                throw new FileLoadException("File target exists", pathTarget);

            var tmpPathApp = @"\Print2Flash3\p2fServer.exe";

            if (File.Exists("%ProgramFiles(x86)%" + tmpPathApp))
                PathApp = Environment.ExpandEnvironmentVariables(
                    "%ProgramFiles(x86)%" + tmpPathApp);
            else
                PathApp = Environment.ExpandEnvironmentVariables(
                    "%ProgramFiles%" + tmpPathApp);

            PathApp = string.Format("\"{0}\"", PathApp);

            string logFile = Path.Combine(
                target.DirectoryName,
                Path.ChangeExtension(target.Name, ".txt"));

            string args = string.Empty;
            args = string.Format("\"{0}\" \"{1}\" /PressPrintButton:on /logfilename:\"{2}\"",
                pathSource, pathTarget,
                logFile);

            DodoNet.Node.LogAppendLine("Print2Flash - App {0} {1}", PathApp, args);

            var info = new ProcessStartInfo(PathApp, args);
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;

            var proc = Process.Start(info);

            var retVal = proc.WaitForExit(1000 * 60 * 10);

            if (!retVal)
            {
                log = "Time out expire";
                proc.Kill();
                throw new TimeoutException();
            }

            log = string.Empty;
            if (File.Exists(logFile))
            {
                using (var file = File.OpenRead(logFile))
                using (var reader = new StreamReader(file, Encoding.Unicode))
                {
                    log = reader.ReadToEnd();
                }

                //File.Delete(logFile);
            }
        }
    }
}
