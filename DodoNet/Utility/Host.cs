using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Text;

namespace DodoNet.Utility
{
    public class Host
    {
        public static IPAddress GetIP()
        {
            IPAddress ret = null;
            
            // create collection to hold network interfaces
            NetworkInterface[] Interfaces;

            // get list of all interfaces
            Interfaces = NetworkInterface.GetAllNetworkInterfaces();

            // loop through interfaces
            foreach (NetworkInterface Interface in Interfaces)
            {
                if (Interface.OperationalStatus == OperationalStatus.Up)
                {
                    // create collection to hold IP information for interfaces
                    UnicastIPAddressInformationCollection IPs;

                    // get list of all unicast IPs from current interface
                    IPs = Interface.GetIPProperties().UnicastAddresses;

                    // loop through IP address collection
                    foreach (UnicastIPAddressInformation IP in IPs)
                    {
                        // check IP address for IPv4
                        if (IP.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(IP.Address))
                        {
                            // write IP address to debug window
                            ret = IP.Address;
                            break;
                        }
                    }
                }
            }

            return ret;
        }
    }
}
