using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Core.Config
{
    public class SyncConfig
    {
        public string Id { get; set; }
        public string SyncTypeTag { get; set; }         // Root Tag GR, MT_CWS_GR, dll
        public string TransportAdapter { get; set; }    // SyncX.Ftp.FtpTransportAdapter
        public string FileAdapter { get; set; }         // SyncX.AdmTamSap.AdmTamSapFileAdapter
        public string IdNoFormat { get; set; }          // {ddMMyyyy} - date format {counter} - running number
        public bool IsSyncOut { get; set; } = false;    // true = sync OUT, false = sync IN (default)
        public string AssemblyHandler { get; set; }     // ApplicationCore.Infrastructure.Service.PurchaseOrderSyncService
        public string BackupInPath { get; set; }        // ~ = AppDomain.AppPath
        public string BackupOutPath { get; set; }       // ~ = AppDomain.AppPath
        public string TempPath { get; set; }
        public string TransportId { get; set; }
        public TransportConfig TransportConfig { get; set; }    // Transport config information

    }
}
