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
        private readonly AppDbContext _context;

        public SyncLogService(AppDbContext context)
        {
            _context = context;
            //var builder = new DbContextOptionsBuilder<AppDbContext>();
            //builder.UseSqlServer(LoadConfig().GetConnectionString("synclog"));
            //_context = new AppDbContext(builder.Options);
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
