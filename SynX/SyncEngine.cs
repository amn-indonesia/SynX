using SynX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SynX
{
    public class SyncEngine
    {
        public static void CheckSyncGet()
        {
            // TODO write implementation here
            throw new NotImplementedException();
        }

        public static string SendSyncSet(string syncId, string recordId, dynamic payload)
        {
            return SendSyncSetResponse(syncId, recordId, payload, false);
        }

        public static string SendSyncSetResponse(string syncId, string recordId, dynamic payload)
        {
            return SendSyncSetResponse(syncId, recordId, payload, true);
        }

        public static ITransportAdapter GetTransportAdapter(string assemblyName)
        {
            var obj = CreateInstance<ITransportAdapter>(assemblyName);
            return obj;
        }

        private static string SendSyncSetResponse(string syncId, string recordId, dynamic payload, bool isResponse)
        {
            // TODO write implmentation here
            throw new NotImplementedException();
        }

        private static T CreateInstance<T>(string fullyQualifiedName)
        {
            if (!fullyQualifiedName.Contains(",")) throw new ArgumentException("\"Full class name, Assembly name\" is requied as method parameter");

            var explodedNames = fullyQualifiedName.Split(",");
            var assemblyName = explodedNames[1];
            var className = explodedNames[0];

            var assem = Assembly.Load(assemblyName);
            var obj = (T) assem.CreateInstance(className);

            return obj;
        }
    }
}
