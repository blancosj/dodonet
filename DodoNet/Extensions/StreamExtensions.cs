using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DodoNet.Extensions
{
    public static class StreamExtensions
    {
        public static Stream CopyTo(this Stream stream, Stream target)
        {
            return stream.CopyTo(target, 1024 * 32);
        }

        /// <summary>
        /// Copies one stream into another one.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="targetStream">The target stream.</param>
        /// <param name="bufferSize">The buffer size used to read / write.</param>
        /// <returns>The source stream.</returns>
        public static Stream CopyTo(this Stream stream, Stream target, int bufferSize)
        {
            if (stream.CanRead == false) throw new InvalidOperationException("Source stream does not support reading.");
            if (target.CanWrite == false) throw new InvalidOperationException("Target stream does not support writing.");

            var s1 = new BufferedStream(stream, bufferSize);
            var s2 = new BufferedStream(target, bufferSize);

            var buffer = new byte[bufferSize];
            var bytesRead = 0;

            stream.Position = 0;
            while ((bytesRead = s1.Read(buffer, 0, bufferSize)) > 0)
            {
                s2.Write(buffer, 0, bytesRead);
            }

            return stream;
        }

        public static string ReadToEnd(this Stream stream, Encoding encoding, int bufferSize)
        {
            string ret = string.Empty;

            var reader = new StreamReader(
                stream, encoding,
                false, bufferSize);

            stream.Position = 0;
            ret = reader.ReadToEnd();

            return ret;
        }
    }
}
