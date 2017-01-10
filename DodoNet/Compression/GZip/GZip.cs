using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections.Generic;

namespace DodoNet.Compression.GZip
{
    public class GZip
    {
        /// <summary>
        /// Comprimir un Stream en formato GZIP
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static void Compress(Stream inStream, Stream outStream)
        {
            if (inStream == null)
            {
                throw new ArgumentNullException("inStream");
            }

            if (outStream == null)
            {
                throw new ArgumentNullException("outStream");
            }

            byte[] bytes = null;
            int n = 0;
            inStream.Position = 0;
            using (GZipStream stGzip = new GZipStream(outStream, CompressionMode.Compress, true))
            {
                bytes = new byte[8192];
                while ((n = inStream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    stGzip.Write(bytes, 0, n);
                }
                stGzip.Flush();
            }
        }

        /// <summary>
        /// Descomprimir un Stream en formato GZIP
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static void Decompress(Stream inStream, Stream outStream)
        {
            if (inStream == null)
            {
                throw new ArgumentNullException("inStream");
            }

            if (outStream == null)
            {
                throw new ArgumentNullException("outStream");
            }

            byte[] bytes = null;
            int n = 0;
            inStream.Position = 0;
            using (GZipStream stGzip = new GZipStream(inStream, CompressionMode.Decompress, true))
            {
                bytes = new byte[8192];
                while ((n = stGzip.Read(bytes, 0, bytes.Length)) > 0)
                {
                    outStream.Write(bytes, 0, n);
                }
            }
        }
    }
}
