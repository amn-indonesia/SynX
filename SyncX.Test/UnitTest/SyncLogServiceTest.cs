using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SynX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SynX.Test.UnitTest
{
    public class SyncLogServiceTest : SyncLogSqliteTest
    {
        public SyncLogServiceTest() : base(
            new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Filename=test.db")
            .Options)
        {

        }

        [Fact]
        public async Task SyncSet_Success()
        {
            using(var context = new AppDbContext(ContextOptions))
            {
                var expected = new SyncLog()
                {
                    RecordId = "1",
                    SyncType = "UT",
                    IdNo = "IDNOUT1",
                    FileName = "sample.xml",
                    IsResponseFile = false,
                    IsSyncOut = true,
                    FileDate = DateTime.Now,
                    SyncStatus = "SENT TO SAP"
                };

                var syncLogService = new SyncLogService(context);
                await syncLogService.LogSyncSet(expected.RecordId, expected.SyncType, expected.IdNo, 
                    expected.FileName, expected.IsResponseFile, expected.SyncStatus);

                await AssertValues(expected, context);
            }
        }

        [Fact]
        public async Task SyncSetResponse_Success()
        {
            using (var context = new AppDbContext(ContextOptions))
            {
                var expected = new SyncLog()
                {
                    RecordId = "1",
                    SyncType = "UT",
                    IdNo = "IDNOUT1",
                    FileName = "sample.xml",
                    IsResponseFile = true,
                    IsSyncOut = true,
                    FileDate = DateTime.Now,
                    SyncStatus = "SENT TO SAP"
                };

                var syncLogService = new SyncLogService(context);
                await syncLogService.LogSyncSet(expected.RecordId, expected.SyncType, expected.IdNo,
                    expected.FileName, expected.IsResponseFile, expected.SyncStatus);

                await AssertValues(expected, context);
            }
        }

        [Fact]
        public async Task SyncGet_Success()
        {
            using (var context = new AppDbContext(ContextOptions))
            {
                var expected = new SyncLog()
                {
                    RecordId = null,
                    SyncType = "UT",
                    IdNo = "IDNOUT3",
                    FileName = "sample2.xml",
                    IsResponseFile = false,
                    IsSyncOut = false,
                    FileDate = DateTime.Now,
                    SyncStatus = "SUCCESS"
                };

                var syncLogService = new SyncLogService(context);
                await syncLogService.LogSyncGet(expected.IdNo, expected.SyncType, expected.FileName, expected.IsResponseFile,
                    expected.SyncStatus);
                await AssertValues(expected, context);
            }
        }

        [Fact]
        public async Task SyncGetResponse_Success()
        {
            using (var context = new AppDbContext(ContextOptions))
            {
                var expected = new SyncLog()
                {
                    RecordId = null,
                    SyncType = "UT",
                    IdNo = "IDNOUT3",
                    FileName = "sample2.xml",
                    IsResponseFile = true,
                    IsSyncOut = false,
                    FileDate = DateTime.Now,
                    SyncStatus = "SUCCESS"
                };

                var syncLogService = new SyncLogService(context);
                await syncLogService.LogSyncGet(expected.IdNo, expected.SyncType, expected.FileName, expected.IsResponseFile,
                    expected.SyncStatus);
                await AssertValues(expected, context);
            }
        }

        private async Task AssertValues(SyncLog expected, AppDbContext context)
        {
            var actual = await context.SyncLogs.Where(e => e.IdNo == expected.IdNo).FirstOrDefaultAsync();
            actual.Should().NotBeNull();
            actual.RecordId.Should().Be(expected.RecordId);
            actual.SyncType.Should().Be(expected.SyncType);
            actual.IdNo.Should().Be(expected.IdNo);
            actual.FileName.Should().Be(expected.FileName);
            actual.IsResponseFile.Should().Be(expected.IsResponseFile);
            actual.IsSyncOut.Should().Be(expected.IsSyncOut);
            actual.SyncStatus.Should().Be(expected.SyncStatus);
            actual.FileDate.Day.Should().Be(expected.FileDate.Day);
            actual.FileDate.Month.Should().Be(expected.FileDate.Month);
            actual.FileDate.Year.Should().Be(expected.FileDate.Year);
        }
    }
}
