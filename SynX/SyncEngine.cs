using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SynX.Core;
using SynX.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SynX
{
    public class SyncEngine
    {
        private readonly SyncLogService _syncLogService;

        public SyncEngine(SyncLogService syncLogService)
        {
            _syncLogService = syncLogService;
        }

        /// <summary>
        /// Check all sync based on all sync configuration registered in applications.config
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public async Task CheckSyncGet()
        {
            await CheckSyncGet(string.Empty);
        }

        /// <summary>
        /// Check sync based on given sync configuration id registered in application.config
        /// </summary>
        /// <param name="syncId"></param>
        /// <exception cref="NotImplementedException"></exception>
        public async Task CheckSyncGet(string syncId)
        {
            var appConfig = SyncLogService.LoadAppSyncConfig();
            var configs = appConfig.Configs;
            if(!string.IsNullOrEmpty(syncId))
                configs = appConfig.Configs.Where(e=>e.Id == syncId).ToList();

            foreach(var config in configs)
            {
                try
                {
                    // prepare transport adapter, file adapter
                    var transportAdapter = GetTransportAdapter(config.TransportAdapter);
                    var fileAdapter = GetFileAdapter(config.FileAdapter);
                    var syncHandler = CreateInstance<ISync>(config.AssemblyHandler);

                    // load files
                    var files = transportAdapter.GetFileList(config);
                    if (files == null || files.Count == 0) continue;

                    // process every files
                    foreach(var file in files)
                    {
                        try
                        {
                            // download sync file to temporaray file
                            var tempFile = Path.GetTempFileName();
                            if(transportAdapter.DownloadFile(file, tempFile, config) == false) 
                                continue;

                            // read and convert sync file to payload
                            var payload = fileAdapter.ReadSyncFile(tempFile, config);
                            if (payload == null) 
                                continue;

                            // check if idno exists
                            var idNo = string.Empty;
                            if (payload.ContainsKey(config.IdNoTag))
                                idNo = (string)payload[config.IdNoTag];

                            // check if this is response file by querying idno in synclog table
                            if (await _syncLogService.IsResponse(idNo))
                            {
                                var logid = await _syncLogService.LogSyncGet(idNo, config.SyncTypeTag, file, true, "RECEIVED");
                                syncHandler.OnFileResponseReceived(config.Id, idNo, payload, logid);
                            } else
                            {
                                var logid = await _syncLogService.LogSyncGet(idNo, config.SyncTypeTag, file, false, "RECEIVED");
                                syncHandler.OnFileReceived(config.Id, idNo, payload, logid);
                            }

                            // move success files to backup folder
                            if (transportAdapter.MoveToBackup(file, config) == false)
                                continue;
                        } catch { }
                    }
                } catch { }
            }
        }

        /// <summary>
        /// Create sync file based on payload given
        /// </summary>
        /// <param name="syncId"></param>
        /// <param name="recordId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task<string> SendSyncSet(string syncId, string recordId, Dictionary<string, object> payload)
        {
            return await SendSyncSetResponse(syncId, recordId, payload, false);
        }

        /// <summary>
        /// Create response sync file based on payload given
        /// </summary>
        /// <param name="syncId"></param>
        /// <param name="recordId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task<string> SendSyncSetResponse(string syncId, string recordId, Dictionary<string, object> payload)
        {
            return await SendSyncSetResponse(syncId, recordId, payload, true);
        }

        private async Task<string> SendSyncSetResponse(string syncId, string recordId, Dictionary<string, object> payload, bool isResponse)
        {
            var appConfig = SyncLogService.LoadAppSyncConfig();
            var config = appConfig.Configs.Where(e => e.Id == syncId).FirstOrDefault();
            if (config == null) 
                throw new KeyNotFoundException($"Sync id {syncId} not found.");

            // prepare transport adapter, file adapter
            var transportAdapter = GetTransportAdapter(config.TransportAdapter);
            var fileAdapter = GetFileAdapter(config.FileAdapter);

            // TODO generate ID_No
            var idNo = "";

            if (payload.ContainsKey(config.IdNoTag))
                payload[config.IdNoTag] = idNo;
            else
                payload.Add(config.IdNoTag, idNo);

            var content = fileAdapter.GenerateSyncFile(payload, config);

            // write file to upload
            var tempFile = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFile, content);
            if (!File.Exists(tempFile))
                throw new Exception($"Failed to generate sync file {tempFile}");

            if (transportAdapter.UploadFile(tempFile, config) == false)
                throw new Exception($"Failed to upload sync file {tempFile}");

            await _syncLogService.LogSyncSet(recordId, config.SyncTypeTag, idNo, tempFile, isResponse, "SENT TO SAP");

            return idNo;
        }

        #region STATIC METHODS

        /// <summary>
        /// Get transport adapter instance
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static ITransportAdapter GetTransportAdapter(string assemblyName)
        {
            var obj = CreateInstance<ITransportAdapter>(assemblyName);
            return obj;
        }

        /// <summary>
        /// Get file adapter instance
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static IFileAdapter GetFileAdapter(string assemblyName)
        {
            var obj = CreateInstance<IFileAdapter>(assemblyName);
            return obj;
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

        //private static SyncLogService syncLog;
        //private static SyncLogService GetSyncLogServiceInstance()
        //{
        //    if (syncLog != null) return syncLog;

        //    var builder = new DbContextOptionsBuilder<AppDbContext>();
        //    builder.UseSqlServer(SyncLogService.LoadConfig().GetConnectionString("synclog"));
        //    var context = new AppDbContext(builder.Options);
        //    syncLog = new SyncLogService(context);
        //    return syncLog;
        //}

        #endregion
    }
}
