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

        protected SyncEngine(SyncLogService syncLogService)
        {
            _syncLogService = syncLogService;
            if (_syncLogService == null) throw new Exception("syncLogService cannot be loaded");
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
            if (appConfig == null)
                throw new Exception($"Could not load configuration from default file appsettings.json");

            var configs = appConfig.Configs;
            if (configs == null)
                throw new Exception($"Could not load configuration with id {syncId}. Check applications.config file.");

            if (!string.IsNullOrEmpty(syncId))
                configs = appConfig.Configs.Where(e=>e.Id == syncId).ToList();

            foreach(var cfg in configs)
            {
                try
                {
                    var config = appConfig.GetConfig(cfg.Id);
                    // prepare transport adapter, file adapter
                    var transportAdapter = GetTransportAdapter(config.TransportAdapter);
                    if (transportAdapter == null)
                        throw new Exception($"Could not load transport adapter with assembly [{config.TransportAdapter}]");

                    var fileAdapter = GetFileAdapter(config.FileAdapter);
                    if (fileAdapter == null)
                        throw new Exception($"Could not load file adapter with assembly [{config.FileAdapter}].");

                    var syncHandler = CreateInstance<ISync>(config.AssemblyHandler);
                    if (syncHandler == null)
                        throw new Exception($"Could not create instance for assembly {config.AssemblyHandler}");

                    // load files
                    var files = transportAdapter.GetFileList(config);
                    if (files == null || files.Count == 0) continue;

                    // process every files
                    foreach(var file in files)
                    {
                        var logid = string.Empty;
                        var idNo = string.Empty;

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
                            if (payload.ContainsKey(config.IdNoTag))
                                idNo = (string)payload[config.IdNoTag];

                            // check if this is response file by querying idno in synclog table
                            string fileName = Path.GetFileName(file);
                            if (await _syncLogService.IsResponse(idNo))
                            {
                                try
                                {
                                    logid = await _syncLogService.LogSyncGet(idNo, config.SyncTypeTag, fileName, true, "RECEIVED");
                                    syncHandler.OnFileResponseReceived(config.Id, idNo, payload, logid);
                                }
                                catch { }
                            } else
                            {
                                try
                                {
                                    logid = await _syncLogService.LogSyncGet(idNo, config.SyncTypeTag, fileName, false, "RECEIVED");
                                    syncHandler.OnFileReceived(config.Id, idNo, payload, logid);
                                }
                                catch { }
                            }

                            // move success files to backup folder
                            if (transportAdapter.MoveToBackup(file, config) == false)
                                continue;
                        } catch (Exception fileEx)
                        {
                            await _syncLogService.LogError(idNo, fileEx.Message, logid);
                        }
                    }
                } catch { }
            }
        }

        public void ResendFile(string syncId, string fileName)
        {
            var appConfig = SyncLogService.LoadAppSyncConfig();
            if (appConfig == null)
                throw new Exception($"Could not load configuration from default file appsettings.json");

            var config = appConfig.GetConfig(syncId);
            if (config == null)
                throw new Exception($"Could not load configuration with id {syncId}. Check applications.config file.");

            string backupPath = config.BackupOutPath;
            string synxFileName = Path.GetFileName(fileName);
            string syncFullFileName = Path.Combine(backupPath, synxFileName);

            if (!File.Exists(fileName))
            {
                fileName = syncFullFileName;
            }

            if (!File.Exists(fileName))
            {
                throw new Exception($"File not exists {fileName}");
            }

            var transport = GetTransportAdapter(config.TransportAdapter);
            if (transport == null)
                throw new Exception($"Could not load transport adapter with assembly {config.TransportAdapter}");

            if (transport.UploadFile(fileName, config) == false)
                throw new Exception($"Failed to upload sync file {fileName}");
        }

        public async Task Reprocess(string syncId, string fileName)
        {
            var appConfig = SyncLogService.LoadAppSyncConfig();
            if (appConfig == null)
                throw new Exception($"Could not load configuration from default file appsettings.json");

            var config = appConfig.GetConfig(syncId);
            if (config == null)
                throw new Exception($"Could not load configuration with id {syncId}. Check applications.config file.");

            string backupPath = config.BackupOutPath;
            string synxFileName = Path.GetFileName(fileName);
            string syncFullFileName = Path.Combine(backupPath, synxFileName);

            if (!File.Exists(fileName))
            {
                fileName = syncFullFileName;
            }

            if (!File.Exists(fileName))
            {
                throw new Exception($"File not exists {fileName}");
            }

            var fileAdapter = GetFileAdapter(config.FileAdapter);
            if (fileAdapter == null)
                throw new Exception($"Could not load file adapter with assembly [{config.FileAdapter}].");

            var payload = fileAdapter.ReadSyncFile(fileName, config);
            if (payload == null)
                throw new Exception($"Failed to read file {fileName}");

            // check if idno exists
            var idNo = "";
            if (payload.ContainsKey(config.IdNoTag))
                idNo = (string)payload[config.IdNoTag];

            var logid = "";
            var syncHandler = CreateInstance<ISync>(config.AssemblyHandler);
            if (syncHandler == null)
                throw new Exception($"Could not create instance for assembly {config.AssemblyHandler}");

            if (await _syncLogService.IsResponse(idNo))
            {
                try
                {
                    logid = await _syncLogService.LogSyncGet(idNo, config.SyncTypeTag, fileName, true, "REPROCESS");
                    syncHandler.OnFileResponseReceived(config.Id, idNo, payload, logid);
                }
                catch { }
            }
            else
            {
                try
                {
                    logid = await _syncLogService.LogSyncGet(idNo, config.SyncTypeTag, fileName, false, "REREPROCESSEIVED");
                    syncHandler.OnFileReceived(config.Id, idNo, payload, logid);
                }
                catch { }
            }
            
            //syncHandler.OnFileResponseReceived(config.Id, idNo, payload, logid);
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
        public async Task<string> SendSyncSetResponse(string syncId, string idNo, Dictionary<string, object> payload)
        {
            return await SendSyncSetResponse(syncId, idNo, payload, true);
        }

        public string GeneratedSyncFileName { get; private set; }

        private async Task<string> SendSyncSetResponse(string syncId, string recordId, Dictionary<string, object> payload, bool isResponse)
        {
            var appConfig = SyncLogService.LoadAppSyncConfig();
            if (appConfig == null)
                throw new Exception($"Could not load configuration from default file appsettings.json");

            var config = appConfig.GetConfig(syncId);
            if (config == null)
            {
                throw new KeyNotFoundException($"Sync id {syncId} not found.");
            }

            // prepare transport adapter, file adapter
            if (string.IsNullOrEmpty(config.TransportAdapter))
            {
                throw new Exception($"TransportAdapter is required for config Id = {config.Id}.");
            }

            if (string.IsNullOrEmpty(config.FileAdapter))
            {
                throw new Exception($"FileAdapter is required for config Id = {config.Id}.");
            }

            var transportAdapter = GetTransportAdapter(config.TransportAdapter);
            var fileAdapter = GetFileAdapter(config.FileAdapter);

            if (transportAdapter == null)
            {
                throw new Exception($"Failed create instance transport adapter {config.TransportAdapter}.");
            }

            if (fileAdapter == null)
            {
                throw new Exception($"Failed create instance file adapter {config.FileAdapter}");
            }

            // generate ID_No
            var idNo = "";
            if (isResponse)
                idNo = recordId;
            else
                idNo = await _syncLogService.GenerateIdNo(config.IdNoFormat);

            if (payload.ContainsKey(config.IdNoTag))
                payload[config.IdNoTag] = idNo;
            else
                payload.Add(config.IdNoTag, idNo);

            var content = fileAdapter.GenerateSyncFile(payload, config);

            // write file to upload
            if (string.IsNullOrEmpty(config.SyncOutFileName)) 
                config.SyncOutFileName = "SYNC_{date}{time}.xml";

            var tempFileName = config.SyncOutFileName
                .Replace("{date}", DateTime.Now.ToString("ddMMyyyy"))
                .Replace("{time}", DateTime.Now.ToString("hhmmss"));

            var tempFile = Path.Combine(Path.GetTempPath(), tempFileName);

            GeneratedSyncFileName = tempFileName;

            await File.WriteAllTextAsync(tempFile, content);
            if (!File.Exists(tempFile))
                throw new Exception($"Failed to generate sync file {tempFile}");

            if (transportAdapter.UploadFile(tempFile, config) == false)
                throw new Exception($"Failed to upload sync file {tempFile}");

            await _syncLogService.LogSyncSet(recordId, config.SyncTypeTag, idNo, tempFileName, isResponse, "SENT TO SAP");

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

        private static SyncEngine syncLogEngine = null;
        public static SyncEngine CreateInstance(string connectionStringName)
        {
            if (syncLogEngine != null) return syncLogEngine;
            var config = SyncLogService.LoadConfig();
            var options = new DbContextOptionsBuilder<SyncLogDbContext>()
                .UseSqlServer(config.GetConnectionString(connectionStringName))
                .Options;

            var context = new SyncLogDbContext(options);
            var syncLogService = new SyncLogService(context);
            syncLogEngine = new SyncEngine(syncLogService);

            return syncLogEngine;
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
