using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Core.Config
{
    public class TransportConfig
    {
        public string TransportType { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RemotePath { get; set; }
        public string DestinationPath { get; set; }
        public string DomainName { get; set; }
        public string IPAddress { get; set; }
        public string LocalPath { get; set; }
        public int EncryptionMode { get; set; } = -1;

        public string RemoteBackupPath { get; set; }

    }
}
