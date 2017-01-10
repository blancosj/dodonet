using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Diagnostics;

using DodoNet;
using DodoNet.Overlay;
using DodoNet.Extensions;

using System.Runtime.Serialization.Formatters.Soap;
using Newtonsoft.Json;

namespace TestDodo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Principio");
                Console.ReadKey();

                //var str = Rest.HttpGet("http://127.0.0.1:8080/DodoItem/1200/?userId=1");
                //Console.WriteLine(str);

                var str = Rest.HttpGet("http://127.0.0.1:8080/DodoItem/?userId=1&skip=1&take=10");

                //var p = new Dictionary<string, string>() { { "pp", "pp" } };

                //var ser = new JavaScriptSerializer(new SimpleTypeResolver());

                // var ser = new JavaScriptSerializer(new ManualResolver());

                var ser = Serialization.Json();

                //var q = new Class1();

                //var str2 = ser.Serialize(q);

                var ret = ser.DeserializeObject(str);
                // var ret = ser.DeserializeObject(str);

                Console.WriteLine(str);

                //var msg = JavaScriptConvert.DeserializeObject(str);


                Console.ReadKey();

                //var source = "C:\\borrable\\xperia.pdf";

                //DodoService.DodoDb db = new DodoService.DodoDb(null);
                //DodoService.DodoContext context = new DodoService.DodoContext();
                //context.CurrentDb = db;

                //DodoService.DodoConversion pp = new DodoService.DodoConversion(context, 100);

                //var item = db.GetItem(19);

                //pp.Converter = new DodoService.Pdf2Flash();
                //pp.Item = item;

                //pp.Convert();


                Console.WriteLine("Fin");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// as __type is missing ,we need to add this
        /// </summary>
        public class ManualResolver : SimpleTypeResolver
        {
            public ManualResolver() { }
            public override Type ResolveType(string id)
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }
}
