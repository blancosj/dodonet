using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DodoNet.Runtime
{
	/// <summary>
	/// Descripción breve de Serialization.
	/// </summary>
	public class Serialization
	{
		public static BinaryFormatter formatter = new BinaryFormatter();

		/// <summary>
		/// serializar un objeto
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static Stream Serialize(object o)
		{
			try
			{
				MemoryStream ret = new MemoryStream();			
				formatter.Serialize(ret, o);
				return ret;
			}
			catch ( Exception err )
			{
				throw err;
			}
		}

		/// <summary>
		/// deserializar el objeto
		/// </summary>
		/// <param name="__s"></param>
		/// <returns></returns>
		public static object Deserialize(Stream __s)
		{	
			try
			{
				__s.Position = 0;
				return formatter.Deserialize(__s);
			}
			catch ( Exception err )
			{
				throw err;
			}
		}
	}
}
