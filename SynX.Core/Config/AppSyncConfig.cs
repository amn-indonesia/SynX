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
            if (Configs == null)
            {
                throw new Exception("AppSyncConfig.Configs can't be null.");
            }

            var config = Configs.Where(e=>e.Id == id).FirstOrDefault();
            if (config == default) return default;
            config.TransportConfig = Transports?.Where(e => e.Id == config.TransportId).FirstOrDefault();
            if (config.TransportConfig == null)
            {
                throw new Exception($"Sync:Transport with id = {config.TransportId} not found.");
            }

            config.BackupInPath = BackupInFolder;
            config.BackupOutPath = BackupOutFolder;
            config.TempPath = TempSyncFolder;
            return config;
        }
    }
}
