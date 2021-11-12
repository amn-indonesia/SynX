using FluentAssertions;
using System;
using Xunit;

namespace SynX.Test.UnitTest
{
    public class ConfigTest
    {
        [Fact]
        public void LoadDefaultConfigFilename()
        {
            var config = SynX.Core.SyncLogService.LoadAppSyncConfig();
            config.Should().NotBeNull();
            config.TempSyncFolder.Should().Be("d:/Temp/backup/temp");
            config.BackupInFolder.Should().Be("d:/Temp/backup/buin");
            config.BackupOutFolder.Should().Be("d:/Temp/backup/buout");
            config.Configs.Should().HaveCount(3);
            config.Configs[0].Id.Should().Be("sample_ftp_with_files");
            config.Transports.Should().HaveCount(5);
            config.Transports[0].Id.Should().Be("ftp_tam");
        }

        [Fact]
        public void LoadCustomConfigFilename()
        {
            var config = SynX.Core.SyncLogService.LoadAppSyncConfig("customsettings.json");
            config.Should().NotBeNull();
            config.TempSyncFolder.Should().Be("d:/custom/backup/temp");
            config.BackupInFolder.Should().Be("d:/custom/backup/buin");
            config.BackupOutFolder.Should().Be("d:/custom/backup/buout");
            config.Configs.Should().HaveCount(1);
            config.Configs[0].Id.Should().Be("gr_custom");
            config.Transports.Should().HaveCount(1);
            config.Transports[0].Id.Should().Be("ftp_custom");
        }

        [Fact]
        public void LoadDefaultConfig_GetConfig_ShouldReturnWithTransport()
        {
            var config = SynX.Core.SyncLogService.LoadAppSyncConfig();
            config.Should().NotBeNull();
            var syncConfig = config.GetConfig("sample_ftp_with_files");
            syncConfig.Should().NotBeNull();
            syncConfig.Id.Should().Be("sample_ftp_with_files");
            syncConfig.TransportConfig.Should().NotBeNull();
            syncConfig.TransportConfig.Id.Should().Be("ftp_tam");
        }

        //[Fact]
        //public void LoadSingleSyncConfig_WithTransport_ShouldReturn_AppSyncConfig()
        //{

        //}

        //[Fact]
        //public void LoadSingleSyncConfig_WithoutTransport_ShouldReturn_AppSyncConfig()
        //{

        //}

        //[Fact]
        //public void LoadMultipleSyncConfig_WithTransport_ShouldReturn_AppSyncConfig()
        //{

        //}

        //[Fact]
        //public void LoadMultipleSyncConfig_WithoutTransport_ShouldReturn_AppSyncConfig()
        //{

        //}
    }
}
