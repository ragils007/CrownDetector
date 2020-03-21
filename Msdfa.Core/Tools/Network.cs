using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Msdfa.Tools
{
	public class Network
	{
        public static long PingHost(string nameOrAddress)
        {
            var ret = -1L;

            using (var pinger = new Ping())
            {
                try
                {
                    var reply = pinger.Send(nameOrAddress);
                    if (reply.Status == IPStatus.Success) ret = reply.RoundtripTime;
                }
                catch (PingException) { /* Discard PingExceptions */ }
            }

            return ret;
        }

		public static string GetIPv4()
		{
			var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
			var ip = (
						  from addr in hostEntry.AddressList
						  where addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
						  select addr.ToString()
				 ).FirstOrDefault();
			return ip;
		}

        /// <summary>
        /// Metoda próbuje ustalić pojedynczy adres IP komputera.
        /// Brane są pod uwagę tylko interfejsy typu Ethernet nie mające w nazwie "Virtual" (VirtualBox) 
        /// </summary>
        /// <returns></returns>
	    public static string GetIpv4Ethernet()
	    {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            var adapters = interfaces
                .Where(x => x.OperationalStatus == OperationalStatus.Up)
                .Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Where(x => !x.Name.Contains("Virtual"))
                .ToList();

            if (adapters.Count == 0) throw new Exception("Nie odnalazłem interfejsów sieciowych typu Ethernet.");
            if (adapters.Count > 1) throw new Exception("Znalazłem wiele interfejsów sieciowych typu Ethernet.");

            var adapter = adapters.First();
            var ipProps = adapter.GetIPProperties();

            var addresses = ipProps.UnicastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork)
                .ToList();
            if (addresses.Count > 1) throw new Exception($"Znalazłem wiele adresów sieciowych dla interfejsu {adapter.Name}");

            return addresses.First().Address.ToString();
        }
    }
}
