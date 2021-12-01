using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SynX.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Core
{
    public class SyncLogService
    {
        private readonly SyncLogDbContext _context;

        public SyncLogService(SyncLogDbContext context)
        {
            _context = context;
            //var builder = new DbContextOptionsBuilder<AppDbContext>();
            //builder.UseSqlServer(LoadConfig().GetConnectionString("synclog"));
            //_context = new AppDbContext(builder.Options);
        }

        public async Task<string> GenerateIdNo(string format)
        {
            var formatted = format.Replace("{date}", DateTime.Now.ToString("ddMMyyyy"))
                .Replace("{time}", DateTime.Now.ToString("hhmmss"));

            string prefix = formatted.Replace("{counter}", "");

            var idnodb = await _context.idNoCounters.Where(e => e.Prefix == prefix).FirstOrDefaultAsync();
            if (idnodb == null)
            {
                idnodb = new IdNoCounter()
                {
                    Id = Guid.NewGuid().ToString(),
                    Prefix = prefix,
                    Counter = 0
                };
                await _context.idNoCounters.AddAsync(idnodb);
                await _context.SaveChangesAsync();
            }

            idnodb = await _context.idNoCounters.Where(e => e.Prefix == prefix).FirstOrDefaultAsync();
            idnodb.Counter++;
            _context.idNoCounters.Update(idnodb);
            await _context.SaveChangesAsync();

            return formatted.Replace("{counter}", idnodb.Counter.ToString());
        }

        public async Task<string> LogError(string idNo, string message, string id = "")
        {
            var log = new SyncLog();
            if (!string.IsNullOrEmpty(id))
            {
                log = _context.SyncLogs.Where(e => e.Id == id).FirstOrDefault();
            }

            log.IdNo = idNo;
            log.ErrorMessage = message;
            log.SyncStatus = "EXCEPTION";

            if (string.IsNullOrEmpty(log.Id))
            {
                log.Id = Guid.NewGuid().ToString();
                await _context.SyncLogs.AddAsync(log);
            } else {
                _context.SyncLogs.Update(log);
            }

            await _context.SaveChangesAsync();
            return log.Id;
        }

        public async Task<string> LogSyncSet(string recordId, string syncType, string idNo, string fileName, bool isResponseFile, 
            string syncStatus, string errorMessage = "")
        {
            var log = new SyncLog()
            {
                Id = Guid.NewGuid().ToString(),
                RecordId = recordId,
                SyncType = syncType,
                IdNo = idNo,
                FileName = fileName,
                IsResponseFile = isResponseFile,
                SyncStatus = syncStatus,
                ErrorMessage = errorMessage,
                FileDate = DateTime.Now,
                IsSyncOut = true
            };

            await _context.SyncLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return log.Id;
        }

        public async Task<string> LogSyncGet(string idNo, string syncType, string fileName, bool isResponseFile, 
            string syncStatus, string errorMessage = "")
        {
            var log = new SyncLog()
            {
                Id = Guid.NewGuid().ToString(),
                RecordId = null,
                SyncType = syncType,
                IdNo = idNo,
                FileName = fileName,
                IsResponseFile = isResponseFile,
                SyncStatus = syncStatus,
                ErrorMessage = errorMessage,
                FileDate = DateTime.Now,
                IsSyncOut = false
            };

            await _context.SyncLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return log.Id;
        }

        public async Task<List<SyncLog>> GetById(string idNo)
        {
            var logs = await _context.SyncLogs.Where(e => e.IdNo == idNo).OrderByDescending(e=>e.FileDate).ToListAsync();
            return logs;
        }

        public async Task<bool> IsResponse(string idNo)
        {
            if (await _context.SyncLogs.Where(e => e.IdNo == idNo && e.IsSyncOut == true).AnyAsync())
                return true;

            return false;
        }

        public static AppSyncConfig LoadAppSyncConfig(string configFile = "")
        {
            var configuration = LoadConfig(configFile);
            var config = configuration.GetSection("Sync").Get<AppSyncConfig>();
            if (config == null) {
                string configFileName = "appsettings.json";
                if (!string.IsNullOrEmpty(configFile))
                {
                    configFileName = configFile;
                }

                throw new Exception($"Sync:AppSyncConfig not found on {configFileName}.");
            }
            return config;
        }

        public static IConfiguration LoadConfig(string configFile = "")
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(configFile))
                configFile = System.IO.Path.Combine(baseDir, "appsettings.json");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(configFile)
                .Build();

            return configuration;
        }
    }
}
