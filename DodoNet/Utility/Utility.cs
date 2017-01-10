using System;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;

namespace DodoNet.Base
{
	/// <summary>
	/// Helper class to contain functions of use when working with mulitcast interfaces.
	/// </summary>
    [ComVisibleAttribute(true)]
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class Utility
	{
		//Pri1: Review for Exception Handling & Event Logging
		#region Private Constants
		private const short AF_INET = 2;
		private const short AF_INET6 = 23;
		private const int SIO_ROUTING_INTERFACE_QUERY = unchecked ((int) (0x40000000 | 0x80000000 | 0x08000000 | 20));
		private const string MulticastIPAddress = "233.45.17.171";  // Use the Pipecleaner address
		private const short port = 5004;
		#endregion
		#region members
		private static IPAddress externalInterface = null;
		public static IPAddress multicastIP;
		#endregion
		#region Private Structures
		/// <summary>
		/// DotNet definition of sockaddr_in, the Winsock2 structure that roughly corresponds to an EndPoint structure from System.Net.  Used to interop with Winsock2,
		/// in this case to call IOControl for SIO_ROUTING_INTERFACE_QUERY.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
			private class sockaddr_in
		{
			public short sin_family = AF_INET;
			public short sin_port = 0;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]       
			public byte[] sin_addr = new Byte[4];
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]       
			public byte[] sin_zero = new Byte[8];
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
			private class sockaddr_in6
		{
			public short sin_family = AF_INET6;
			public short sin_port = 0;
			public uint sin6_flowinfo = 0;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=16)]       
			public byte[] sin_addr = new Byte[16];
			public uint sin6_scope_id = 0;

		}
		#endregion
		#region Public Static Methods

		/// <summary>
		/// Find the interface we should be binding to to receive a multicast stream by using Socket.IOControl to call
		/// SIO_ROUTING_INTEFACE_QUERY (see WSAIoctl in Winsock2 documentation) passing in a known multicast address.
		/// </summary>
		/// <returns>IPAddress containing the local multicast interface</returns>
		/// <example>
		/// IPAddress ifAddress = MulticastInterface.GetLocalMulticastInterface();
		/// </example>
		public static IPAddress GetLocalMulticastInterface()
		{
			if (externalInterface != null)
			{
				return externalInterface;
			}
			else if (multicastIP != null)
			{
				return GetLocalRoutingInterface(multicastIP);
			}
			else
			{
				return null;
			}
		}

		//Removed, replace this functionality by using bind(IPAddress.Any, ... or IPAddress.IPv6Any);
		public static IPAddress GetLocalRoutingInterface(IPAddress ipAddress)
		{
			Socket sock = null;
			IntPtr ptrInAddr = IntPtr.Zero;
			IntPtr ptrOutAddr = IntPtr.Zero;

			if ( ipAddress.AddressFamily == AddressFamily.InterNetwork )
			{
				sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				sockaddr_in inAddr = new sockaddr_in();
				sockaddr_in outAddr = new sockaddr_in();

				try 
				{
					ipAddress.GetAddressBytes().CopyTo(inAddr.sin_addr, 0);

					// create a sockaddr_in function for our destination IP address
					inAddr.sin_port = IPAddress.HostToNetworkOrder(port);

					// create an block of unmanaged memory for use by Marshal.StructureToPtr.  We seem to need to do this, even though
					// StructureToPtr will go ahead and release/reallocate the memory
					ptrInAddr = Marshal.AllocCoTaskMem(Marshal.SizeOf(inAddr));

					// Copy inAddr from managed to unmanaged, reallocating the unmanaged memory
					Marshal.StructureToPtr(inAddr, ptrInAddr, true);

					// Create a managed byte array to hold the structure, but in byte array form
					byte[] byteInAddr = new byte[Marshal.SizeOf(inAddr)];

					// Copy the structure from unmanaged ptr into managed byte array
					Marshal.Copy(ptrInAddr, byteInAddr, 0, byteInAddr.Length);

					// Create a second managed byte array to hold the output sockaddr_in structure
					byte[] byteOutAddr = new byte[Marshal.SizeOf(inAddr)];

					// Make the call to IOControl, asking for the Interface we should use if we want to route a packet to inAddr
					sock.IOControl(
						SIO_ROUTING_INTERFACE_QUERY,
						byteInAddr,
						byteOutAddr
						);

					// create the memory placeholder for our local interface
          
					// Copy the results from the byteOutAddr into an unmanaged pointer
					ptrOutAddr = Marshal.AllocCoTaskMem(Marshal.SizeOf(outAddr));
					Marshal.Copy(byteOutAddr, 0, ptrOutAddr, byteOutAddr.Length);
          
					// Copy the data from the unmanaged pointer to the ourAddr structure
					Marshal.PtrToStructure(ptrOutAddr, outAddr);
				}
				catch(SocketException se)
				{
					// Perhaps there were no interfaces present, AKA No Network Adapters enabled/installed and connected to media (wired or wireless)
					EventLog.WriteEntry("RtpListener", se.ToString(), EventLogEntryType.Warning, 99);
				}
				finally
				{
					// Release the socket
					sock = null;

					Marshal.FreeCoTaskMem(ptrInAddr);
					Marshal.FreeCoTaskMem(ptrOutAddr);
				}

				// Return an IPAddress structure that is initialized with the value of the IP address contained in the outAddr structure
				if (outAddr != null)
				{
					int len = outAddr.sin_addr.Length;

					// Have to convert the byte[] to a uint.  It turns out that the IPAddress ctor won't create an IPv4 address from four bytes, it only uses the byte[] ctor for 16 byte IPv6 construction?!?
					uint ipAsInt = 0;

					for (int i = len; i > 0; i--)
					{
						ipAsInt = ipAsInt * 256 + outAddr.sin_addr[i - 1];
					}
					externalInterface = new IPAddress(ipAsInt);
					return(new IPAddress(ipAsInt));
				}
				else
				{
					return null;
				}

			}

			if ( ipAddress.AddressFamily == AddressFamily.InterNetworkV6 )
			{
				sock = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
				sockaddr_in6 inAddr = new sockaddr_in6();
				sockaddr_in6 outAddr = new sockaddr_in6();

				try 
				{
					ipAddress.GetAddressBytes().CopyTo(inAddr.sin_addr, 0);

					// create a sockaddr_in function for our destination IP address
					inAddr.sin_port = IPAddress.HostToNetworkOrder(port);

					// create an block of unmanaged memory for use by Marshal.StructureToPtr.  We seem to need to do this, even though
					// StructureToPtr will go ahead and release/reallocate the memory
					ptrInAddr = Marshal.AllocCoTaskMem(Marshal.SizeOf(inAddr));

					// Copy inAddr from managed to unmanaged, reallocating the unmanaged memory
					Marshal.StructureToPtr(inAddr, ptrInAddr, true);

					// Create a managed byte array to hold the structure, but in byte array form
					byte[] byteInAddr = new byte[Marshal.SizeOf(inAddr)];

					// Copy the structure from unmanaged ptr into managed byte array
					Marshal.Copy(ptrInAddr, byteInAddr, 0, byteInAddr.Length);

					// Create a second managed byte array to hold the output sockaddr_in structure
					byte[] byteOutAddr = new byte[Marshal.SizeOf(inAddr)];

					// Make the call to IOControl, asking for the Interface we should use if we want to route a packet to inAddr

					sock.IOControl(
						SIO_ROUTING_INTERFACE_QUERY,
						byteInAddr,
						byteOutAddr
						);

					// create the memory placeholder for our local interface
					// Copy the results from the byteOutAddr into an unmanaged pointer
					ptrOutAddr = Marshal.AllocCoTaskMem(Marshal.SizeOf(outAddr));
					Marshal.Copy(byteOutAddr, 0, ptrOutAddr, byteOutAddr.Length);
          
					// Copy the data from the unmanaged pointer to the ourAddr structure
					Marshal.PtrToStructure(ptrOutAddr, outAddr);
				}
				catch (SocketException se)
				{
					// Perhaps there were no interfaces present, AKA No Network Adapters enabled/installed and connected to media (wired or wireless)
					EventLog.WriteEntry("RtpListener", se.ToString(), EventLogEntryType.Warning, 99);
				}
				finally
				{
					// Release the socket
					sock = null;

					Marshal.FreeCoTaskMem(ptrInAddr);
					Marshal.FreeCoTaskMem(ptrOutAddr);
				}

				// Return an IPAddress structure that is initialized with the value of the IP address contained in the outAddr structure
				if (outAddr != null)
				{
					externalInterface = new IPAddress(outAddr.sin_addr);
					return(new IPAddress(outAddr.sin_addr));
				}
				else
				{
					return null;
				}

			}
			return null;

		}
        
		public static bool IsMulticast(IPAddress ipAddress)
		{
			// Only IPv4 multicast addresses are supported for now.
			//Pri3: Review IPv6 multicast group support
			if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
			{
				return false;
			}

			byte[] addressBytes = ipAddress.GetAddressBytes();
			byte[] multicastLowerBoundBytes = IPAddress.Parse("224.0.0.0").GetAddressBytes();
			byte[] multicastUpperBoundBytes = IPAddress.Parse("240.0.0.0").GetAddressBytes();

			if ( !BytesGreaterThanOrEqual(addressBytes, multicastLowerBoundBytes ) )
			{
				return false;
			}

			if ( !BytesGreaterThanOrEqual( multicastUpperBoundBytes, addressBytes) )
			{
				return false;
			}

			return true;
		}

		private static bool BytesGreaterThanOrEqual(byte[] b1, byte[] b2)
		{
			for (int i = 0; i < b1.Length; i++)
			{
				if (b1[i] < b2[i])
				{ 
					return false;
				}
				if (b1[i] > b2[i])
				{
					return true;
				}
			}
			return true;
		}

		public static bool IsMulticast(IPEndPoint ipEndPoint)
		{
			return IsMulticast(ipEndPoint.Address);
		}

		#endregion
		#region Calculate EndPoint
		public static IPEndPoint GetIPEndPoint(string __ip, int __port)	
		{
			// init vars
			IPEndPoint ret = null;
			IPHostEntry ipEntry;
			// The IP Address Array. Holds an array of resolved Host Names.
			IPAddress ipAddr;			
			if ( __ip.ToLower().CompareTo( "localhost" ) == 0 ||
				__ip.ToLower().CompareTo( "0.0.0.0" ) == 0 )
			{
                __ip = IPHost();
			}

			// Value of alpha characters
			char[] alpha = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ-".ToCharArray();
			// If alpha characters exist we know we are doing a forward lookup
			if (__ip.IndexOfAny(alpha) == -1) 
			{
                ipEntry = Dns.GetHostByName(__ip);
                if (ipEntry.AddressList.Length > 0)
                {
                    ipAddr = ipEntry.AddressList[0];
                    if (__port < 0)
                        __port = 0;
                    ret = new IPEndPoint(ipAddr, __port);
                }
			} 
			else 
			{
                if (__ip.ToLower() == "any")
                {
                    ret = new IPEndPoint(IPAddress.Any, __port);
                }
                else
                {
                    IAsyncResult __iar = Dns.BeginResolve(__ip, null, null);
                    __iar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10), false);
                    if (__iar.IsCompleted)
                    {
                        ipEntry = Dns.EndResolve(__iar);
                        if (ipEntry != null)
                        {
                            ipAddr = ipEntry.AddressList[0];
                            if (__port < 0)
                                __port = 0;
                            ret = new IPEndPoint(ipAddr, __port);
                        }
                    }
                }
			}

			return ret;
		}

		public static string IPHost()
		{
			string ret = "";

			try
			{
                // Otra forma de enumerar...
				ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
				ManagementObjectCollection objMOC = objMC.GetInstances();

				bool found = false;
				foreach(ManagementObject objMO in objMOC)
				{
					if( Convert.ToBoolean( objMO["ipEnabled"] ) )
					{
						string[] direcciones = (string[]) objMO["IPAddress"];
						for (int i = 0; i < direcciones.Length; i++)
						{	
							Console.WriteLine
								("IP #" + direcciones[i] + ": " 
								+ "ID: " + objMO["Index"]
								+ "; DNSHostName: " + objMO["DNSHostName"]
								+ "; DNSDomain: " + objMO["DNSDomain"]
								+ "; Tipo: " + objMO["ServiceName"]
								+ "; NetworkAddresses: " + direcciones[i]
								+ "; Descripcion: " + objMO["Description"]);

							if ( ! found && ! direcciones[i].StartsWith( "0.0.0.0" ) && direcciones[i].Length > 0 )
							{
								Console.WriteLine( "Cogemos esta dirección: {0}", direcciones[i] );
								ret = direcciones[i];
								found = true;
								break;
							}
						}				
					}
				}
			}
			catch
			{   
			}

			return ret;
		}

		#endregion

		#region options w32
		/*
					Win32_NetworkAdapterConfiguration.ArpAlwaysSourceRoute
					Win32_NetworkAdapterConfiguration.ArpUseEtherSNAP
					Win32_NetworkAdapterConfiguration.Caption
					Win32_NetworkAdapterConfiguration.DatabasePath
					Win32_NetworkAdapterConfiguration.DeadGWDetectEnabled
					Win32_NetworkAdapterConfiguration.DefaultIPGateway
					Win32_NetworkAdapterConfiguration.DefaultTOS
					Win32_NetworkAdapterConfiguration.DefaultTTL
					Win32_NetworkAdapterConfiguration.Description
					Win32_NetworkAdapterConfiguration.DHCPEnabled
					Win32_NetworkAdapterConfiguration.DHCPLeaseExpires
					Win32_NetworkAdapterConfiguration.DHCPLeaseObtained
					Win32_NetworkAdapterConfiguration.DHCPServer
					Win32_NetworkAdapterConfiguration.DNSDomain
					Win32_NetworkAdapterConfiguration.DNSDomainSuffixSearchOrder
					Win32_NetworkAdapterConfiguration.DNSEnabledForWINSResolution
					Win32_NetworkAdapterConfiguration.DNSHostName
					Win32_NetworkAdapterConfiguration.DNSServerSearchOrder
					Win32_NetworkAdapterConfiguration.DomainDNSRegistrationEnabled
					Win32_NetworkAdapterConfiguration.ForwardBufferMemory
					Win32_NetworkAdapterConfiguration.FullDNSRegistrationEnabled
					Win32_NetworkAdapterConfiguration.GatewayCostMetric
					Win32_NetworkAdapterConfiguration.IGMPLevel
					Win32_NetworkAdapterConfiguration.Index
					Win32_NetworkAdapterConfiguration.IPAddress
					Win32_NetworkAdapterConfiguration.IPConnectionMetric
					Win32_NetworkAdapterConfiguration.IPEnabled
					Win32_NetworkAdapterConfiguration.IPFilterSecurityEnabled
					Win32_NetworkAdapterConfiguration.IPPortSecurityEnabled
					Win32_NetworkAdapterConfiguration.IPSecPermitIPProtocols
					Win32_NetworkAdapterConfiguration.IPSecPermitTCPPorts
					Win32_NetworkAdapterConfiguration.IPSecPermitUDPPorts
					Win32_NetworkAdapterConfiguration.IPSubnet
					Win32_NetworkAdapterConfiguration.IPUseZeroBroadcast
					Win32_NetworkAdapterConfiguration.IPXAddress
					Win32_NetworkAdapterConfiguration.IPXEnabled
					Win32_NetworkAdapterConfiguration.IPXFrameType
					Win32_NetworkAdapterConfiguration.IPXMediaType
					Win32_NetworkAdapterConfiguration.IPXNetworkNumber
					Win32_NetworkAdapterConfiguration.IPXVirtualNetNumber
					Win32_NetworkAdapterConfiguration.KeepAliveInterval
					Win32_NetworkAdapterConfiguration.KeepAliveTime
					Win32_NetworkAdapterConfiguration.MACAddress
					Win32_NetworkAdapterConfiguration.MTU
					Win32_NetworkAdapterConfiguration.NumForwardPackets
					Win32_NetworkAdapterConfiguration.PMTUBHDetectEnabled
					Win32_NetworkAdapterConfiguration.PMTUDiscoveryEnabled
					Win32_NetworkAdapterConfiguration.ServiceName
					Win32_NetworkAdapterConfiguration.SettingID
					Win32_NetworkAdapterConfiguration.TcpipNetbiosOptions
					Win32_NetworkAdapterConfiguration.TcpMaxConnectRetransmissions
					Win32_NetworkAdapterConfiguration.TcpMaxDataRetransmissions
					Win32_NetworkAdapterConfiguration.TcpNumConnections
					Win32_NetworkAdapterConfiguration.TcpUseRFC1122UrgentPointer
					Win32_NetworkAdapterConfiguration.TcpWindowSize
					Win32_NetworkAdapterConfiguration.WINSEnableLMHostsLookup
					Win32_NetworkAdapterConfiguration.WINSHostLookupFile
					Win32_NetworkAdapterConfiguration.WINSPrimaryServer
					Win32_NetworkAdapterConfiguration.WINSScopeID
					Win32_NetworkAdapterConfiguration.WINSSecondaryServer
					*/	
		#endregion
	}
}