using Newtonsoft.Json;
using SynX.Core;
using System;
using System.Dynamic;
using System.Xml.Linq;

namespace SynX.FileAdapter.AdmTamPpos
{
    public class AdmTamFileAdapter : IFileAdapter
    {
        public string GenerateSyncFile(dynamic payload)
        {
            throw new NotImplementedException();
        }

        public dynamic ReadSyncFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName)) return null;
            XDocument doc = XDocument.Parse(fileName);
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic expected = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);
            return expected;
        }
    }
}
