using System.Net;
using ServManager.Data;
using ServManager.Models;
using ServManager.Utils;

namespace ServManager.Worker
{
    public class MyBackgroundTasks
    {
        public Task PingServer()
        {
            IPAddress Address = IPAddress.Parse("192.168.0.29");

            bool status = SendPing.PingAddress(Address);
            string hostName = SendPing.GetHostName(Address);
            // Log.Warning($"{Address} - {hostName} -> {status} - port: 1111 - {SendPing.PingHost(Address, 1111)}");
            Console.WriteLine($"{Address} - {hostName} -> {status} - port: 1111 - {SendPing.PingHost(Address, 1111)}");
            Console.WriteLine();

            return Task.CompletedTask;
        }
    }
}