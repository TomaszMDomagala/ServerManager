using System.Net;
using ServManager.Docker;


namespace ServManager.Models
{
    public class Server
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public required IPAddress Address { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool? Available { get; set; }
        public List<ServerApp>? Apps { get; set; }
    }

    public class ServerApp
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public required uint Port { get; set; }
        public bool? Available { get; set; }
        public Container? Container { get; set;}
        public string? HowTo { get; set; }
    }
}