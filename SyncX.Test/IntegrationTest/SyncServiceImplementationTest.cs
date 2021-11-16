using SynX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Test.IntegrationTest
{
    public class SyncServiceImplementationTest : ISync
    {
        public void OnFileReceived(string syncId, string idNo, Dictionary<string, object> payload, string syncLogId)
        {
            if (string.IsNullOrEmpty(syncId)) throw new ArgumentException("syncId cannot be null or empty", new Exception(syncLogId));
            if (string.IsNullOrEmpty(idNo)) throw new ArgumentException("idNo cannot be null or empty", new Exception(syncLogId));
            if (payload == null || payload.Count <= 0) throw new ArgumentNullException("payload cannot be null or empty", new Exception(syncLogId));
            if (string.IsNullOrEmpty(syncLogId)) throw new ArgumentException("syncLogId cannot be null or empty", new Exception(syncLogId));
            if (idNo == "UT_FAILED") throw new Exception("Failed demo", new Exception(syncLogId));
        }

        public void OnFileResponseReceived(string syncId, string idNo, Dictionary<string, object> payload, string syncLogId)
        {
            throw new NotImplementedException();
        }
    }
}
