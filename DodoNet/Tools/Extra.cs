using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DodoNet.Tools
{
    public class Conversion
    {
        protected static SHA1 sha = SHA1CryptoServiceProvider.Create();
        protected static string donChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWYXZabcdefghijklmnopqrstuvwxyz";

        public static string CalculateHashCode(Stream stream)
        {
            string ret = "";
            byte[] hashcode = sha.ComputeHash(stream);
            ret = Conversion.Bytes2Don(hashcode);
            stream.Position = 0;
            return ret;
        }

        public static string Bytes2Don(byte[] bytes)
        {
            string ret = "";
            foreach (byte b in bytes)
            {
                ret += Num2Don((long)b);
            }
            return ret;
        }

        public static long Don2Num(string don)
        {
            long ret = 0;
            long a = 0;

            for (int x = 0; x < don.Length; x++)
            {
                string b = don.Substring(don.Length - x - 1, 1);
                a = (long)donChars.IndexOf(b) * (long)Math.Pow((long)donChars.Length, x);
                ret += a;
            }

            return ret;
        }

        public static string Num2Don(long num)
        {
            string ret = "";
            long a = num; long b = 0; long c = 0;
            while (a > 0)
            {
                Math.DivRem(a, donChars.Length, out b);
                a = Math.DivRem(a - b, (long)donChars.Length, out c);
                ret = donChars.Substring((int)b, 1) + ret;
            }
            return ret;
        }

        /// <summary>
        /// El n�mero que se genera es mutante.
        /// Una forma com�n podr�a ser la siguiente: 29KAT374512M
        ///
        /// A�o      -> 1999 (1999-1970 = 29)
        /// Mes      -> 11   (Letra K)
        /// D�a      -> 10   (Letra A) (Del 1 al 9 son n�meros, del 10 en adelante son letras)
        /// Hora     -> 20   (Letra U) (las 0 horas tambi�n cuentan)
        /// Minuto   -> 37
        /// Segundo  -> 45
        /// Cent�sima-> 12
        /// Azar     -> M    N�mero 13 (Del 0 al 25 son en total 26 n�meros)
        ///
        /// que se basa en el a�o, mes, d�a, hora, minuto, Segundo, cent�sima, azar.
        /// 
        /// Se usa base hexadecimal para convertir los n�meros.
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueCode12s()
        {
            string ret = "";

            DateTime dtCur = DateTime.Now;

            string sdate = string.Format("{0:yy}{1}{2}", 
                dtCur, dtCur.Month, dtCur.Day);
            string stime = string.Format("{0}{1}{2}{3}", 
                dtCur.Hour, dtCur.Minute, dtCur.Second, dtCur.Millisecond);

            Random rdm = new Random(13);
            int random = rdm.Next(65, 90);
            // string random = char.rdm.Next(1, 254);
            ret = Conversion.Num2Don(long.Parse(string.Format("{0}{1}{2}", sdate, stime, random)));
            return ret;
        }

        /// <summary>
        /// Valida una cuenta bancaria 
        /// </summary>
        /// <param name="accountBank"></param>
        /// <returns></returns>
        public static bool ValidateAccountBank(string accountBank)
        {
            int[] weights = new int[] { 1, 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            int dc1 = 0;
            int dc2 = 0;
            bool ret = false;
            if (accountBank.Length == 20)
            {
                // calculo primer d�gito de control
                for (int i = 7; i >= 0; i--) dc1 += weights[i+2] * int.Parse(accountBank.Substring(i, 1));
                dc1 = 11 - (dc1 % 11);
                if (dc1 == 11) dc1 = 0;
                if (dc1 == 10) dc1 = 1;

                // calculo segundo d�gito de control
                for (int i = 9; i >= 0; i--) dc2 += weights[i] * int.Parse(accountBank.Substring(i + 10, 1));
                dc2 = 11 - (dc2 % 11);
                if (dc2 == 11) dc2 = 0;
                if (dc2 == 10) dc2 = 1;

                string chequeo = string.Format("{0:00}", 10 * dc1 + dc2);

                if (chequeo == accountBank.Substring(8, 2))
                    ret = true;
            }
            return ret;
        }
    }
}
