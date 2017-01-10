using System;
using System.Collections.Generic;
using System.Text;

namespace LightApp.ServiceProcess
{
    public class WinService
    {
        public static bool Install(string ServicePath, string Name, string DisplayName, string Description, Attributes.ServiceType ServType, Attributes.ServiceAccessType ServAccessType, Attributes.ServiceStartType ServStartType, Attributes.ServiceErrorControl ServErrorControl, string dependencies)
        {
            if (Name.Length > 256) throw new Exception("The maximum length for a service name is 256 characters.");
            if (Name.IndexOf(@"\") >= 0 || Name.IndexOf(@"/") >= 0) throw new Exception(@"Service names cannot contain \ or / characters.");
            if (DisplayName.Length > 256) throw new Exception("The maximum length for a display name is 256 characters.");
            //The spec says that if a service's path has a space in it, then we must quote it...
            //if (ServicePath.IndexOf(" ") >= 0)
            //	ServicePath = "\""  ServicePath + "\"";
            //ServicePath = ServicePath.Replace(@"\", @"\\");
            try
            {
                IntPtr sc_handle = ServicesAPI.OpenSCManagerA(null, null, ServicesAPI.ServiceControlManagerType.SC_MANAGER_CREATE_SERVICE);
                if (sc_handle == IntPtr.Zero) return false;

                IntPtr sv_handle = ServicesAPI.CreateService(sc_handle, Name, DisplayName, ServAccessType, ServType, ServStartType, ServErrorControl, ServicePath, null, 0, dependencies, null, null);
                //IntPtr sv_handle = ServicesAPI.CreateService(sc_handle, Name, DisplayName, 0xF0000 | 0x0001 | 0x0002 | 0x0004 | 0x0008 | 0x0010 | 0x0020 | 0x0040 | 0x0080 | 0x0100, 0x00000010, 0x00000002, 0x00000001, ServicePath, null, 0, null, null, null);
                if (sv_handle == IntPtr.Zero)
                {
                    ServicesAPI.CloseServiceHandle(sc_handle);
                    return false;
                }
                ServicesAPI.CloseServiceHandle(sv_handle);
                ServicesAPI.CloseServiceHandle(sc_handle);

                //Sets a service's description by adding a registry entry for it.
                if (Description != null && Description != "")
                {
                    try
                    {
                        using (Microsoft.Win32.RegistryKey serviceKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\" + Name, true))
                        {
                            serviceKey.SetValue("Description", Description);
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            }
			catch
            {
				return false;
			}
        }

        public static bool Uninstall(string Name)
        {
            try
            {
                IntPtr sc_hndl = ServicesAPI.OpenSCManagerA(null, null, ServicesAPI.GENERIC_WRITE);

                if (sc_hndl != IntPtr.Zero)
                {
                    IntPtr svc_hndl = ServicesAPI.OpenService(sc_hndl, Name, ServicesAPI.DELETE);
                    if (svc_hndl != IntPtr.Zero)
                    {
                        int i = ServicesAPI.DeleteService(svc_hndl);
                        if (i != 0)
                        {
                            ServicesAPI.CloseServiceHandle(sc_hndl);
                            return true;
                        }
                        else
                        {
                            ServicesAPI.CloseServiceHandle(sc_hndl);
                            return false;
                        }
                    }
                    else return false;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
