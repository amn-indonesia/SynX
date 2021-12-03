using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SynX.Core;
using SynX.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SynX.Test.UnitTest
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
        public async Task SyncSet_Return_IDNo_XMLContentCreated()
        {
            using (var context = new SyncLogDbContext(ContextOptions))
            {
                var syncLog = new SyncLogService(context);
                var sync = SyncEngine.CreateInstance("default");

                var payload = new Dictionary<string, object>();
                payload.Add("TransactionDate", "2021-11-15 14:53:00");
                payload.Add("Plant", "SAP");


                var idNo = await sync.SendSyncSet("unit_test", "UT_RECORD_ID", payload);
                var log = await context.SyncLogs.Where(e => e.IdNo == idNo).FirstOrDefaultAsync();
                log.Should().NotBeNull();
                log.IsSyncOut.Should().BeTrue();
            }
        }
    }
}
