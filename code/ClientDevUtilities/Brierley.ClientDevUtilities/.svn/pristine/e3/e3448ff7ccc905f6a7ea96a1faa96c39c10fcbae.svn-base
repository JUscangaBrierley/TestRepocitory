using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Brierley.FrameWork.LWIntegration.IPFiltering
{
    public class INetUtil
    {
        /// <summary>
        /// This method converts an Ipv4 address to a string representation
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public static string GetStringIpAddress(IPAddress addr)
        {
            //string ipaddress = Encoding.BigEndianUnicode.GetString(addr.GetAddressBytes());
            var ipbytes = addr.GetAddressBytes();
            string ipaddress = string.Empty;
            for (int i = 0; i < ipbytes.Length; i++)
            {
                if (!string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress += ".";
                }
                ipaddress += ipbytes[i];
            }
            return ipaddress;
        }

        public static uint ConvertIpv4ToUint(IPAddress ipAddress)
        {
            var ipBytes = ipAddress.GetAddressBytes();
            var ip = (uint)ipBytes[3] << 24;
            ip += (uint)ipBytes[2] << 16;
            ip += (uint)ipBytes[1] << 8;
            ip += (uint)ipBytes[0];

            return ip;
        }

        public static IPAddress GetIPv4Address(IPAddress address)
        {
            if (IPAddress.IPv6Loopback.Equals(address))
            {
                return new IPAddress(0x0100007F);
            }
            IPAddress[] addresses = Dns.GetHostAddresses(address.ToString());
            IPAddress v4 = addresses.FirstOrDefault(i => i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            if (v4 == null)
            {
                IPHostEntry entry = Dns.GetHostEntry(address);
                addresses = Dns.GetHostAddresses(entry.HostName);
                v4 = addresses.FirstOrDefault(i => i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }
            return v4;
        }
    }
}
