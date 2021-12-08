using SynX.Core;
using SynX.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;

namespace SynX.FileAdapter.SimpleXml
{
    public class SimpleXmlFileAdapter : IFileAdapter
    {
        public string GenerateSyncFile(Dictionary<string, object> payload, SyncConfig config)
        {
            var writer = new XMLReadWrite.XMLWriter();
            var rootXml = config.SyncTypeTag;
            var tempFile = System.IO.Path.GetTempFileName();
            writer.GenerateXML(payload, rootXml, tempFile);
            var content = System.IO.File.ReadAllText(tempFile);
            return content;
        }

        public Dictionary<string, object> ReadSyncFile(string fileName, SyncConfig config)
        {
            if (!File.Exists(fileName))
            {
                throw new Exception($"Could not find file \"{fileName}\".");
            }

            var reader = new XMLReadWrite.XMLReader();
            var rootXml = config.SyncTypeTag;
            var dict = reader.ReadXML(fileName, rootXml);
            return dict;
        }
    }
}
