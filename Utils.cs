using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ServManager.Utils
{
    public class ParseArguments
    {
        public ParseArguments(string[] args){
        foreach (string item in args)
            {
                Console.WriteLine(item);
            }
        }
    }

    public class SendPing
    {

        private static PingReply? Reply { get; set; }
        public static bool PingAddress(IPAddress address)
        {
            Ping pingSender = new();
            PingReply reply = pingSender.Send(address);

            if (reply.Status == IPStatus.Success)
            {
                Reply = reply;
                return true;
            }

            return false;
        }

        public static bool PingAddress(string domainName)
        {
            Ping pingSender = new();
            PingReply reply = pingSender.Send(domainName);

            if (reply.Status == IPStatus.Success)
            {
                Reply = reply;
                return true;
            }

            return false;
        }

        public static long GetRoundtripTime()
        {
            if (Reply is not null)
            {
                return Reply.RoundtripTime;
            }
            return -1;
        }

        public static IPStatus GetStatus()
        {
            if (Reply is not null)
            {
                return Reply.Status;
            }
            return IPStatus.Unknown;
        }

        public static bool PingHost(IPAddress Address, int portNumber)
        {
            try
            {
                using var client = new TcpClient(Address.ToString(), portNumber);
                return true;
            }
            catch (SocketException)
            {
                Console.WriteLine("Error pinging host: " + Address + ":" + portNumber.ToString());
                return false;
            }            
        }
        public static bool PingHost(string domainName, int portNumber)
        {
            try
            {
                using var client = new TcpClient(domainName, portNumber);
                return true;
            }
            catch (SocketException)
            {
                Console.WriteLine("Error pinging host: " + domainName + ":" + portNumber.ToString());
                return false;
            }            
        }

        public static IPAddress GetIPAddress(string domainName)
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(domainName);
            for(int index=0; index < hostInfo.AddressList.Length; index++)
            {
                Console.WriteLine(hostInfo.AddressList[index]);
            }
            return hostInfo.AddressList[0];
        }

        public static string GetHostName(IPAddress address)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(address);
            return hostEntry.HostName;
        }

    }
}