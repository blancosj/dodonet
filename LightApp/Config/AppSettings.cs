using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace LightApp.Config
{
	/// <summary>
	/// Descripción breve de AppSettings.
	/// </summary>
	public class AppSettings
	{
		public static string FileConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";

		public XmlDocument MyDoc;
		public XmlElement MyRoot;
		public XmlNode MySectionGroup;
		public XmlNode MyProfile;
		public string Profile = "Default";

		public AppSettings(string SectionGroup)
		{
			try
			{
				// cargamos el archivo de configuracion
				if (File.Exists(FileConfigPath))
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(FileConfigPath);

					MyRoot = doc.DocumentElement;

					MySectionGroup = MyRoot.SelectSingleNode("Profile[@Name='" + Profile + "']/" + 
						SectionGroup);

					SetProfile();
				}
				else
				{
					throw new Exception( string.Format( "El archivo de configuración no existe {0}", FileConfigPath ) );
				}
			}
			catch ( Exception err )
			{
				throw err;
			}
		}

		public AppSettings()
		{
			try
			{
				// cargamos el archivo de configuracion
				if (File.Exists(FileConfigPath))
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(FileConfigPath);

					MyRoot = doc.DocumentElement;

					SetProfile();

					MyProfile = MyRoot.SelectSingleNode("Profile[@Name='" + Profile + "']");
				}
				else
				{
					throw new Exception( string.Format( "El archivo de configuración no existe {0}", FileConfigPath ) );
				}
			}
			catch ( Exception err )
			{
				throw err;
			}
		}

		public void Save()
		{
			MyRoot.OwnerDocument.Save( FileConfigPath );
		}

		private void SetProfile()
		{
			XmlNode tmpNode;

			if (MyRoot != null)
			{
				tmpNode = MyRoot.SelectSingleNode("ProfileInUse");
 
				if (tmpNode != null)
				{
					Profile	= tmpNode.InnerText;
				}
			}
		}

		public ConfigValue GetValue(string Section)
		{
			try
			{
				XmlNode xn = MySectionGroup.SelectSingleNode("Item[@Name='" + Section + "']");

			
				ConfigValue cv = new ConfigValue(xn);
				return cv;
			}
			catch ( Exception err )
			{
				throw new Exception( string.Format( "{0}, {1}", Section, err.Message ), err );
			}
		}

		public ConfigValue GetValue(string SectionGroup, string Section)
		{
			try
			{
				MySectionGroup = MyRoot.SelectSingleNode("Profile[@Name='" + Profile + "']/" + 
					SectionGroup);

				XmlNode xn = MySectionGroup.SelectSingleNode("Item[@Name='" + Section + "']");

				ConfigValue cv = new ConfigValue(xn);

				return cv;
			}
			catch ( Exception err )
			{
				throw new Exception( string.Format( "{0}->{1}, {2}", SectionGroup, Section, err.Message ), err );
			}
		}

		public Hashtable GetValues(string SectionGroup, string Section)
		{
            Hashtable ret = null;
            try
            {   
                Hashtable tmp;

                // init vars
                ret = new Hashtable();

                MySectionGroup = MyRoot.SelectSingleNode("Profile[@Name='" + Profile + "']/" +
                    SectionGroup);

                XmlNode xn = MySectionGroup.SelectSingleNode(Section);

                foreach (XmlNode Item in xn.ChildNodes)
                {
                    tmp = new Hashtable();

                    foreach (XmlAttribute xa in Item.Attributes)
                    {
                        tmp.Add(xa.Name, xa.Value);
                    }

                    ret.Add(
                        Item.Attributes.GetNamedItem("Name").Value, tmp);

                }
            }
            catch (Exception err)
            {
                throw new Exception(string.Format("{0}->{1}, {2}", SectionGroup, Section, err.Message), err);
            }

			return ret;
		}

		public string GetValueStr(string SectionGroup, string Section)
		{
			try
			{
				string ret;

				// init vars
				ret = "";

				MySectionGroup = MyRoot.SelectSingleNode("Profile[@Name='" + Profile + "']/" + 
					SectionGroup);

				XmlNode xn = MySectionGroup.SelectSingleNode("Item[@Name='" + Section + "']");

				ConfigValue cv = new ConfigValue(xn);

				ret = cv.Value.ToString();

				return ret;
			}
			catch ( Exception err )
			{
				throw new Exception( string.Format( "{0}->{1}, {2}", SectionGroup, Section, err.Message ), err );
			}
		}


		public void SetValue(string _sectiongroup, 
			string _name, object _xvalue, bool _activated)
		{
			XmlNode MyItem, xntmp;

			MySectionGroup = MyRoot.SelectSingleNode("Profile[@Name='" + Profile + "']/" + 
				_sectiongroup);

			if ( MySectionGroup != null )
			{
				MyItem = MySectionGroup.SelectSingleNode("Item[@Name='" + _name + "']");;
				xntmp = CreateItem(_name, _xvalue, _activated);
				if (MyItem == null)
				{
					MySectionGroup.AppendChild(xntmp);
				}
				else
				{
					MySectionGroup.ReplaceChild(xntmp, MyItem);
				}
			}
			else
			{
				throw new Exception( string.Format( "no existe el grupo", _sectiongroup ) );
			}
		}

		public XmlNode GetGroupNode(string _sectiongroup)
		{
			MySectionGroup = MyRoot.SelectSingleNode("Profile[@Name='" + Profile + "']/" + 
				_sectiongroup);

			if (MySectionGroup == null)
			{
				MySectionGroup = MyDoc.CreateNode(XmlNodeType.Element, _sectiongroup, "");
			}

			return MySectionGroup;
		}

		public XmlNode GetSectionNode(string _section)
		{
			return MySectionGroup;
		}

		private XmlNode CreateItem(string _name, object _xvalue, 
			bool _activated)
		{
			XmlNode xntmp;
			XmlAttribute xa;

			xntmp = MyRoot.OwnerDocument.CreateElement("", "Item", "");

			xa = MyRoot.OwnerDocument.CreateAttribute("", "Name", "");
			xa.Value = _name;
			xntmp.Attributes.Append(xa);
			xa = MyRoot.OwnerDocument.CreateAttribute("", "Value", "");
			xa.Value = _xvalue.ToString();
			xntmp.Attributes.Append(xa);
			xa = MyRoot.OwnerDocument.CreateAttribute("", "Type", "");
			xa.Value = _xvalue.GetType().Name;
			xntmp.Attributes.Append(xa);
			xa = MyRoot.OwnerDocument.CreateAttribute("", "Activated", "");
			xa.Value = _activated.ToString();
			xntmp.Attributes.Append(xa);

			return xntmp;
		}

		private XmlNode CreateGroup(string _name)
		{
			XmlNode xntmp;

			xntmp = MyRoot.OwnerDocument.CreateElement("", _name, "");

			return xntmp;
		}
	}

	public class ConfigValue
	{
		private string Name;
		public string Type;
		private string StringValue;
		public bool Activated;

		public ConfigValue(string _name, object _xvalue, bool _activated)
		{
			Name = _name;
			Type = _xvalue.GetType().Name;
			StringValue = _xvalue.ToString();
			Activated = _activated;
		}

		public override string ToString()
		{
			return StringValue;
		}


		/*
		public XmlNode GetXmlNode()
		{
			XmlNode xn = 
			XmlAttribute xa_name = new XmlAttribute();
			xa_name.Name = "Name";
			xa_name.Value = Name;

			XmlAttribute xa_type = new XmlAttribute();
			xa_type.Name = "Type";
			xa_type.Value = Type.ToString();

			XmlAttribute xa_stringvalue = new XmlAttribute();
			xa_stringvalue.Name = "StringValue";
			xa_stringvalue.Value = StringValue.ToString();

			XmlAttribute xa_activated = new XmlAttribute();
			xa_activated.Name = "Activated";
			xa_activated.Value = Activated.ToString();

			xn.Attributes.Append(xa_name);
			xn.Attributes.Append(xa_type);
			xn.Attributes.Append(xa_stringvalue);
			xn.Attributes.Append(xa_activated);
			
			return xn;

		}
					*/

		public ConfigValue(XmlNode xn)
		{
			Name = xn.Attributes.GetNamedItem("Name").Value;
			Type = xn.Attributes.GetNamedItem("Type").Value;
			StringValue = xn.Attributes.GetNamedItem("Value").Value;
			Activated = bool.Parse(xn.Attributes.GetNamedItem("Activated").Value);
		}

		public object Value
		{
			get
			{
				try
				{
					object ret;
					ret = "";
					switch(Type)
					{
						case("Bool"):
						{
							ret = bool.Parse(StringValue);
							break;
						}
						case("String"):
						{
							ret = StringValue;
							break;
						}
						case("Int32"):
						{
							ret = int.Parse(StringValue);
							break;
						}
						case("TimeSpan"):
						{
							ret = TimeSpan.Parse(StringValue);
							break;
						}
					}
					return ret;
				}
				catch ( Exception err )
				{
					throw new Exception( string.Format( "{0}, {1}", Name, err.Message ), err );
				}
			}
		}
	}
}
