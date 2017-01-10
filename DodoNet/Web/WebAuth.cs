using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;

using DodoNet.Extensions;
using DodoNet.Tools;

namespace DodoNet.Web
{
    public class WebAuth
    {
        public string Id { get; private set; }
        public string Handle { get; private set; }
        public string Salt { get; private set; }

        public static explicit operator string(WebAuth tmp)
        {
            return tmp.ToString();
        }

        public static explicit operator WebAuth(string tmp)
        {
            WebAuth ret = null;

            if (tmp != null)
                ret = new WebAuth(tmp);

            return ret;
        }

        public WebAuth()
        {
            Id = RandomDataGenerator.GetRandomString(WebConfig.SessionIdLength);

            byte[] saltBytes = GenerateSalt();

            // Convert plain text into a byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(Id);
            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes = plainTextBytes.AddBytes(saltBytes);

            /*
            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = ComputeHash(plainTextWithSaltBytes);
            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = hashBytes.AddBytes(saltBytes);
            */

            Handle = Convert.ToBase64String(plainTextWithSaltBytes);
            Salt = Convert.ToBase64String(saltBytes);
        }

        public WebAuth(string id)
        {           
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            bool mayBeSalted = id.Length > WebConfig.SessionIdLength;

            if (mayBeSalted)
            {
                Handle = id;

                Id = id.Substring(0, WebConfig.SessionIdLength);
                Salt = id.Substring(id.Length);
            }
            else
                Id = id;
        }

        byte[] GenerateSalt()
        {
            // Define min and max salt sizes.
            int minSaltSize = 4;
            int maxSaltSize = 8;

            // Generate a random number for the size of the salt.
            Random random = new Random();
            int saltSize = random.Next(minSaltSize, maxSaltSize);

            // Allocate a byte array, which will hold the salt.
            byte[] saltBytes = new byte[saltSize];

            var rng = new RNGCryptoServiceProvider();

            // Fill the salt with cryptographically strong byte values.
            rng.GetNonZeroBytes(saltBytes);

            return saltBytes;
        }

        byte[] ComputeHash(byte[] textBytes)
        {
            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            var hash = new SHA256Managed();

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(textBytes);

            hash.Clear();

            return hashBytes;
        }

        bool VerifyHash(byte[] textBytes, byte[] hashBytes)
        {
            return ComputeHash(textBytes) == hashBytes;
        }

        public override string ToString()
        {
            return Id;
        }

        public string ToStringHandle()
        {
            return Handle;
        }

        public override bool Equals(object obj)
        {
            var ret = false;

            if (!object.ReferenceEquals(obj, null))
            {
                ret = GetHashCode() == obj.GetHashCode();
            }

            return ret;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
