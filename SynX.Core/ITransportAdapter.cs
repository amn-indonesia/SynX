using SynX.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Core
{
    public interface ITransportAdapter
    {
        public Exception ExceptionData { get; set; }
        List<string> GetFileList(SyncConfig syncConfig);
        string ReadFileContent(string remoteFileName, SyncConfig syncConfig);
        bool MoveFile(string remoteSourceFileName, string remoteDestinationFileName, SyncConfig syncConfig);
        bool DownloadFile(string remoteFileName, string localFileName, SyncConfig syncConfig);
        bool UploadFile(string localFileName, SyncConfig syncConfig);
        bool RemoveFile(string remoteFileName, SyncConfig syncConfig);
    }
}
