using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DodoNet.Tools
{
    public class Streams
    {
        public static void CopyToEnd(Stream source, Stream destiny, long total)
        {
            byte[] buf = new byte[8192];
            long read = 0;
            int readFromSource = 0;

            while (read < total)
            {
                readFromSource = buf.Length;

                long diff = total - read;
                if (total - read < readFromSource)
                    readFromSource = (int)diff;

                int tmp = source.Read(buf, 0, readFromSource);

                if (tmp > 0)
                {
                    destiny.Write(buf, 0, tmp);
                    read += tmp;
                }
                else
                    break; ;
            }
        }

        public static string ReadLine(Stream st)
        {
            byte[] dumb;
            return Streams.ReadLine(st, out dumb, Encoding.ASCII);
        }

        public static string ReadLine(Stream st, Encoding encoding)
        {
            byte[] dumb;
            return Streams.ReadLine(st, out dumb, encoding);
        }

        /// <summary>
        /// lee la siguiente linea en stream desplazando el cursor y devolvieldolo a 
        /// a su sitio si no se detectan retornos de carro
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static string ReadLine(Stream st, out byte[] raw, Encoding encoding)
        {
            string ret = null;
            StringBuilder sb = new StringBuilder();

            var oldPos = st.Position;

            List<byte> line = new List<byte>();

            raw = null;

            /*
            var tmp = Encoding.ASCII.GetString(((MemoryStream)st).ToArray());

            if (tmp.EndsWith("Message-Path"))
                Debugger.Break();

            Console.WriteLine("__Received: {0} -- \r\n{1}\r\n", st.Position,
                tmp.Substring((int)st.Position));
            */
            //FSM code for parsing the \r\n
            int state = -1;
            int x = 0;

            while (state < 2)
            {                
                int i = st.ReadByte();

                if (i > 0)
                    x++;

                switch (i)
                {
                    case 13:
                        state = 1;
                        break;
                    case 10:
                        state = state == 1 ? 2 : 0;
                        break;
                    case -1:
                        // devolvemos el cursor a su sitio pq no se ha encontrado \r\n
                        st.Position -= x;
                        state = 3;
                        break;
                    default:
                        state = 0;
                        // sb.Append(encoding.i);
                        line.Add((byte)i);
                        break;
                }

                switch (state)
                {
                    case 2:
                        raw = line.ToArray();
                        ret = encoding.GetString(raw);
                        break;
                    case 3:
                        raw = line.ToArray();
                        // Console.WriteLine(encoding.GetString(raw));
                        break;
                }
            }

            // Console.WriteLine(ret);

            return ret;
        }
    }
}
