using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Interfaces
{
    public interface ITransportAdapter
    {
        List<string> GetFileList(SyncConfig syncConfig);
        string ReadFileContent(string fileName, SyncConfig syncConfig);
        bool MoveFile(string sourceFileName, string destinationFileName, SyncConfig syncConfig);
        bool DownloadFile(string sourceFileName, string destinationFileName, SyncConfig syncConfig);
        bool UploadFile(string sourceFileName, SyncConfig syncConfig);
        bool RemoteFile(string fileName);
    }
}
