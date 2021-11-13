using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Core
{
    public interface ISync
    {
        void OnFileReceived(string syncId, string idNo, Dictionary<string, object> payload, string syncLogId);
        void OnFileResponseReceived(string syncId, string idNo, Dictionary<string, object> payload, string syncLogId);
    }
}
