using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Extensions
{
    public static class BytesExtensions
    {
        public static byte[] AddBytes(this byte[] source, byte[] extension)
        {
            int oldSourceSize = source.Length;
            int newSize = oldSourceSize + extension.Length;

            Array.Resize(ref source, newSize);
            Array.Copy(extension, 0, source, oldSourceSize, extension.Length);

            return source;
        }

        public static byte[] InsertBytes(this byte[] source, byte[] extension)
        {
            int oldSourceSize = source.Length;
            int newSize = oldSourceSize + extension.Length;

            Array.Resize(ref source, newSize);
            Array.Copy(source, 0, source, extension.Length, oldSourceSize);
            Array.Copy(extension, 0, source, 0, extension.Length);

            return source;
        }
    }
}
