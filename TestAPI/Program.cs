using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestAPI
{
    public class Program
    {
        static string myIP;
        public static void Main(string[] args)
        {
            //myIP = GetLocalIPv4();
            //Console.WriteLine($"HOSTNAME: {myIP}");            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5000",
                        $"http://{GetLocalIPv4()}:5000"); // my IP Address
                });

        internal static string GetLocalIPv4(NetworkInterfaceType _type = NetworkInterfaceType.Ethernet)
        {  // Checks your IP adress from the local network connected to a gateway. This to avoid issues with double network cards
            string output = "";  // default output
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) // Iterate over each network interface
            {  // Find the network interface which has been provided in the arguments, break the loop if found
                //if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                //{   // Fetch the properties of this adapter
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();
                    // Check if the gateway adress exist, if not its most likley a virtual network or smth
                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {   // Iterate over each available unicast adresses
                        foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                        {   // If the IP is a local IPv4 adress
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {   // we got a match!
                                output = ip.Address.ToString();
                                break;  // break the loop!!
                            }
                        }
                    }
                //}
                // Check if we got a result if so break this method
                if (output != "") { break; }
            }
            // Return results
            return output;
        }
    }
}
