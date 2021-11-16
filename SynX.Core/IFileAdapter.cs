using SynX.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Core
{
    public interface IFileAdapter
    {
        Dictionary<string, object> ReadSyncFile(string fileName, SyncConfig config);
        string GenerateSyncFile(Dictionary<string, object> payload, SyncConfig config);
    }
}
