using SynX.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Test.UnitTest
{
    public class LocalTransportAdapter : Core.ITransportAdapter
    {
        public Exception ExceptionData { get; set; }

        public bool DownloadFile(string remoteFileName, string localFileName, SyncConfig syncConfig)
        {
            if (!System.IO.File.Exists(remoteFileName)) return false;
            System.IO.File.Copy(remoteFileName, localFileName, true);
            return true;
        }

        public List<string> GetFileList(SyncConfig syncConfig)
        {
            var list = System.IO.Directory.GetFiles(syncConfig.RemotePath, "*.xml");
            return list.ToList();
        }

        public bool MoveFile(string remoteSourceFileName, string remoteDestinationFileName, SyncConfig syncConfig)
        {
            if (!System.IO.File.Exists(remoteSourceFileName)) return false;
            if (!System.IO.File.Exists(remoteDestinationFileName)) return false;
            System.IO.File.Copy(remoteSourceFileName, remoteDestinationFileName);
            return true;
        }

        public bool MoveToBackup(string remoteFileName, SyncConfig syncConfig)
        {
            var destinationFolder = syncConfig.BackupOutPath;
            if (!System.IO.File.Exists(remoteFileName)) return false;
            System.IO.File.Copy(remoteFileName, destinationFolder);
            return true;
        }

        public string ReadFileContent(string remoteFileName, SyncConfig syncConfig)
        {
            if (!System.IO.File.Exists(remoteFileName)) return String.Empty;
            var content = System.IO.File.ReadAllText(remoteFileName);
            return content;
        }

        public bool RemoveFile(string remoteFileName, SyncConfig syncConfig)
        {
            if (!System.IO.File.Exists(remoteFileName)) return false;
            System.IO.File.Delete(remoteFileName);
            return true;
        }

        public bool UploadFile(string localFileName, SyncConfig syncConfig, bool isBackup = false, bool isBackupError = false, string originalFileName = null)
        {
            var destinationFolder = syncConfig.TransportConfig.LocalPath;
            if (!System.IO.File.Exists(localFileName)) return false;
            // tambahan reza
            if(isBackup == true)
            {
                destinationFolder = syncConfig.TransportConfig.RemoteBackupPath;
                if(isBackupError == true)
                {
                    if (isBackupError == true)
                    {
                        localFileName = "ERR_" + localFileName;
                    }
                }
            }
            //
            System.IO.File.Copy(localFileName, destinationFolder);
            return true;
        }
    }
}
