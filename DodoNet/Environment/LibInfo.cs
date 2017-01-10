using System;
using System.Reflection;

namespace System.EnvironmentInfo
{
	/// <summary>
	/// Descripción breve de LibInfo.
	/// </summary>
	public class LibInfo
	{
		/// <summary>
		/// devuelve la version de un modulo
		/// </summary>
		/// <param name="Module"></param>
		public static string GetVersionThisModule()
		{
            try
            {
                string ret = "";
                Version __v = AssemblyName.GetAssemblyName(AppDomain.CurrentDomain.BaseDirectory + "\\DodoNet.dll").Version;
                ret = string.Format("{0}.{1}", __v.Major, __v.Minor);
                return ret;
            }
            catch
            {
                return "";
            }
		}
	}
}
