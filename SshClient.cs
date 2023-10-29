using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Messages;
using System.Net;

namespace ServManager.SSHConnection
{
    public class SSH
    {
        private IPAddress Address { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private SshClient? ClientInstance { get; set; }

        public SSH(IPAddress Address, string Username, string Password)
        {
            this.Address = Address;
            this.Username = Username;
            this.Password = Password;
        }

        public void Connect()
        {
            ClientInstance = new SshClient(Address.ToString(), Username, Password);
            ClientInstance.Connect();
        }

        public bool IsConnected()
        {
            if (ClientInstance is not null)
            {
                return ClientInstance.IsConnected;
            }
            return false;
        }

        public string ExecureCommand(string command)
        {
            if (ClientInstance is not null)
            {
                var output = ClientInstance.RunCommand(command);
                return output.Result;
            }

            return "none";
        }

        public void Disconnect()
        {
            try
            {
                ClientInstance?.Disconnect();
            }
            catch(ObjectDisposedException e)
            {
                Console.WriteLine("Caught: {0}", e.Message);
            }
        }
    }
}

