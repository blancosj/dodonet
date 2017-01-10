using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace DodoNet.Http
{
    public class HttpTime
    {
        public static string Date2TimeHtml(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", new CultureInfo("en-US"));
        }

        public static DateTime TimeHtml2Date(string date)
        {
            return DateTime.Parse(date, new CultureInfo("en-US"));
        }

        /// <summary>
        /// redondear una fecha para que sea compatible con las fechas que llegan desde mensajes
        /// HTTP que no tienen milisegundos
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime RoundDate(DateTime dateTime)
        {
            return HttpTime.TimeHtml2Date(HttpTime.Date2TimeHtml(dateTime)); ;
        }

        public static DateTime RoundedNow()
        {
            return HttpTime.RoundDate(DateTime.Now);
        }

        public static string HtmlNow()
        { 
            return HttpTime.Date2TimeHtml(DateTime.Now);
        }
    }
}
