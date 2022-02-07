using System;

namespace SynX.Core
{
    public class SyncLog
    {
        public string Id { get; set; }
        public string RecordId { get; set; }
        public string IdNo { get; set; }
        public string SyncType { get; set; }
        public string FileName { get; set; }
        public DateTime FileDate { get; set; } = DateTime.Now;
        public bool IsSyncOut { get; set; }
        public bool IsResponseFile { get; set; }
        public string SyncStatus { get; set; }
        public string ErrorMessage { get; set; }

    }
}
