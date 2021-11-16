using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Core.Config
{
    public class SyncConfig
    {
        /// <summary>
        /// Id sync config, sebagai penanda pada applications.config
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Tipe tag yang digunakan sebagai identifikasi tipe file sync
        /// </summary>
        public string SyncTypeTag { get; set; }         // Root Tag GR, MT_CWS_GR, dll

        /// <summary>
        /// Full class name, assembly name untuk binary yang akan menghandle proses transport
        /// </summary>
        public string TransportAdapter { get; set; }    // SyncX.Ftp.FtpTransportAdapter

        /// <summary>
        /// Full class name, assembly name untuk binary yang akan menghandle proses file
        /// </summary>
        public string FileAdapter { get; set; }         // SyncX.AdmTamSap.AdmTamSapFileAdapter

        /// <summary>
        /// Nama tag untuk ID_No, default adalah IDNo
        /// </summary>
        public string IdNoTag { get; set; } = "IDNo";

        /// <summary>
        /// Format file untuk syncout. Default "SYNC_{date}_{time}.xml"
        /// </summary>
        public string SyncOutFileName { get; set; } = "SYNC_{date}_{time}.xml";

        /// <summary>
        /// Format IdNo, tag yang mungkin: {date} {time} {counter}
        /// </summary>
        public string IdNoFormat { get; set; }

        /// <summary>
        /// Flag jenis sync out atau in, default false (IN)
        /// </summary>
        public bool IsSyncOut { get; set; } = false;    // true = sync OUT, false = sync IN (default)

        /// <summary>
        /// Full class name, assembly name untuk binary yang akan menghandle jika ada sync get
        /// </summary>
        public string AssemblyHandler { get; set; }     // ApplicationCore.Infrastructure.Service.PurchaseOrderSyncService

        /// <summary>
        /// Path backup untuk sync IN
        /// </summary>
        public string BackupInPath { get; set; }        // ~ = AppDomain.AppPath

        /// <summary>
        /// Path backup untuk sync OUT
        /// </summary>
        public string BackupOutPath { get; set; }       // ~ = AppDomain.AppPath

        /// <summary>
        /// Path untuk temporary processing file
        /// </summary>
        public string TempPath { get; set; }

        /// <summary>
        /// Lokasi remote path untuk meletakkan / mengambil file sync. Jika kosong maka akan menggunakan dari TransportConfig
        /// </summary>
        public string RemotePath { get; set; }

        /// <summary>
        /// Id penanda konfigurasi transport yang digunakan
        /// </summary>
        public string TransportId { get; set; }

        /// <summary>
        /// Objek konfigurasi transport
        /// </summary>
        public TransportConfig TransportConfig { get; set; }    // Transport config information

    }
}
