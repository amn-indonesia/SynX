using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Interfaces
{
    public interface IFileAdapter
    {
        dynamic ReadSyncFile(string fileName);
        string GenerateSyncFile(dynamic payload);
    }
}
