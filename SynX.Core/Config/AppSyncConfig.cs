using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Core.Config
{
    public class AppSyncConfig
    {
        public string TempSyncFolder { get; set; }
        public string BackupInFolder { get; set; }
        public string BackupOutFolder { get; set; }
        public List<SyncConfig> Configs { get; set; } = new List<SyncConfig>();
        public List<TransportConfig> Transports { get; set; } = new List<TransportConfig>();

        public SyncConfig GetConfig(string id)
        {
            var config = Configs.Where(e=>e.Id == id).FirstOrDefault();
            if (config == default) return default;
            config.TransportConfig = Transports.Where(e => e.Id == config.TransportId).FirstOrDefault();
            config.BackupInPath = BackupInFolder;
            config.BackupOutPath = BackupOutFolder;
            config.TempPath = TempSyncFolder;
            return config;
        }
    }
}
