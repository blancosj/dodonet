using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Tools
{
    public class RandomDataGenerator
    {
        /// <summary>
        /// The uppercase alphabet.
        /// </summary>
        internal const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// The lowercase alphabet.
        /// </summary>
        internal const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// The set of base 10 digits.
        /// </summary>
        internal const string Digits = "0123456789";

        /// <summary>
        /// The set of digits, and alphabetic letters (upper and lowercase) that are clearly
        /// visually distinguishable.
        /// </summary>
        internal const string AlphaNumericNoLookAlikes = "23456789abcdefghjkmnpqrstwxyzABCDEFGHJKMNPQRSTWXYZ";

        public static string GetRandomString(int length)
        {
            string ret = string.Empty;

            char[] randomString = new char[length];

            Random rand = new Random();

            for (int i = 0; i < length; i++)
            {
                randomString[i] = AlphaNumericNoLookAlikes[rand.Next(AlphaNumericNoLookAlikes.Length)];
            }

            ret = new string(randomString);

            return ret;
        }
    }
}
