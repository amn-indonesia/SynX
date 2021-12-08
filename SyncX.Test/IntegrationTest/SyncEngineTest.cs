using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SynX.Core;
using SynX.Test.UnitTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SynX.Test.IntegrationTest
{
    public class SyncEngineTest : SyncLogSqliteTest
    {
        public SyncEngineTest() : base(
            new DbContextOptionsBuilder<SyncLogDbContext>()
            .UseSqlite("Filename=test.db")
            .Options)
        {

        }

        [Fact]
        public async Task SyncGet_SuccessResult_DataShouldUpdated()
        {
            using (var context = new SyncLogDbContext(ContextOptions))
            {
                var syncLog = new SyncLogService(context);
                var sync = SyncEngine.CreateInstance("default"); //new SyncEngine(syncLog);
                await sync.CheckSyncGet("sample_ftp_with_files");
            }
        }

        [Fact]
        public async Task SyncGet_FailedResult_DataShouldUpdated()
        {
            using (var context = new SyncLogDbContext(ContextOptions))
            {
                var syncLog = new SyncLogService(context);
                var sync = SyncEngine.CreateInstance("default"); //new SyncEngine(syncLog);
                await sync.CheckSyncGet("sample_ftp_with_files");
            }
        }

        [Fact]
        public async Task SyncSet_NotResponse_DataShouldUpdated()
        {
            using (var context = new SyncLogDbContext(ContextOptions))
            {
                var syncLog = new SyncLogService(context);
                var sync = SyncEngine.CreateInstance("default"); //new SyncEngine(syncLog);
                var payload = new Dictionary<string, object>();
                payload.Add("TransactionDate", "2021-11-15 14:53:00");
                payload.Add("Plant", "SAP");
                var idNo = await sync.SendSyncSet("sample_ftp_sap_in", "UT_RECORD_NO_RESPONSE", payload);
                var log = await context.SyncLogs.Where(e => e.IdNo == idNo).FirstOrDefaultAsync();
                log.Should().NotBeNull();
                log.IsSyncOut.Should().BeTrue();
                log.RecordId.Should().Be("UT_RECORD_NO_RESPONSE");
            }
        }

        [Fact]
        public async Task SyncSet_Response_DataShouldUpdated()
        {
            using (var context = new SyncLogDbContext(ContextOptions))
            {
                var syncLog = new SyncLogService(context);
                var sync = SyncEngine.CreateInstance("default"); //new SyncEngine(syncLog);
                var payload = new Dictionary<string, object>();
                payload.Add("TransactionDate", "2021-11-15 14:53:00");
                payload.Add("Plant", "SAP");
                var idNo = await sync.SendSyncSetResponse("sample_ftp_sap_in", "UT_RECORD_WITH_RESPONSE", payload);
                var log = await context.SyncLogs.Where(e => e.IdNo == idNo).FirstOrDefaultAsync();
                log.Should().NotBeNull();
                log.IsSyncOut.Should().BeTrue();
                log.RecordId.Should().Be("UT_RECORD_WITH_RESPONSE");
            }
        }


        [Fact]
        public async Task SyncSet_Response_DataShouldUpdated2()
        {
            using (var context = new SyncLogDbContext(ContextOptions))
            {
                var syncLog = new SyncLogService(context);
                var sync = SyncEngine.CreateInstance("default"); //new SyncEngine(syncLog);
                var payload = new Dictionary<string, object>() {
                    {"ID_No", "KAP202109270001"},
                    {"Transaction_Date", "11/30/2021 2:55:49 PM" },
                    {"PO_No","PN123"},
                    {"Details", new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>() {
                                { "PO_No", "PN123" },
                                { "Order_no", "DN5120060017967A" },
                                { "Item", new List<Dictionary<string, object>>()
                                    {
                                        new Dictionary<string, object>() {
                                            { "Part_No", "P6" },
                                            { "Status", "1" },
                                            { "Message", "" },
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
                var idNo = await sync.SendSyncSetResponse("GR", "1", payload);
            }
        }
    }
}
