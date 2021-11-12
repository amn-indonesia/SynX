using FluentAssertions;
using SynX;
using SynX.Core;
using SynX.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SynX.Test.IntegrationTest
{
    public class FtpTransportTest
    {
        [Fact]
        public void GetListFileFromFtp_Return_Empty()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_without_files", out SyncConfig config);
            var result = ftp.GetFileList(config);
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetListFileFromFtp_Return_List()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_with_files", out SyncConfig config);
            string remotePath = config.TransportConfig.RemotePath;
            var result = ftp.GetFileList(config);
            result.Should().NotBeEmpty();
            result.Should().HaveCountGreaterThanOrEqualTo(3);
            result.Should().Contain($"{remotePath}/File1.txt");
            result.Should().Contain($"{remotePath}/File2.txt");
            result.Should().Contain($"{remotePath}/File3.txt");
        }

        [Fact]
        public void DownloadFile_Exist_Return_True()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_with_files", out SyncConfig config);
            string sourceFile = Path.Combine(config.TransportConfig.RemotePath, "File1.txt");
            string destFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download.txt");
            
            var result = ftp.DownloadFile(sourceFile, destFile, config);
            result.Should().BeTrue();
            File.Exists(destFile).Should().BeTrue();
        }

        [Fact]
        public void DownloadFile_NotExists_Return_False()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_without_files", out SyncConfig config);
            string sourceFile = Path.Combine(config.TransportConfig.RemotePath, "NoFile.txt");
            string destFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NoDownload.txt");

            var result = ftp.DownloadFile(sourceFile, destFile, config);
            result.Should().BeFalse();
        }

        [Fact]
        public void ReadFile_Exists_Return_FileContent()
        {
            string expected = "isi dari baris pertama" + Environment.NewLine +
                "isi dari baris kedua" + Environment.NewLine +
                "isi dari baris ketiga" + Environment.NewLine + 
                Environment.NewLine + Environment.NewLine +
                "isi dari baris ke enam";
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_with_files", out SyncConfig config);
            string sourceFile = Path.Combine(config.TransportConfig.RemotePath, "File1.txt");

            var result = ftp.ReadFileContent(sourceFile, config);
            result.Should().NotBeEmpty();
            result.Should().Be(expected);
        }

        [Fact]
        public void ReadFile_NotExists_Return_StringEmpty()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_without_files", out SyncConfig config);
            string sourceFile = Path.Combine(config.TransportConfig.RemotePath, "FileNotExists.txt");

            var result = ftp.ReadFileContent(sourceFile, config);
            result.Should().BeEmpty();
        }

        [Fact]
        public void RemoveFile_Exists_Return_True()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_with_files", out SyncConfig config);
            string sourceFile = Path.Combine(config.TransportConfig.RemotePath, "PleaseRemoveMe.txt");

            var result = ftp.RemoveFile(sourceFile, config);
            result.Should().BeTrue();
        }

        [Fact]
        public void RemoveFile_NotExists_Return_False()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_without_files", out SyncConfig config);
            string sourceFile = Path.Combine(config.TransportConfig.RemotePath, "FilenotExists.txt");

            var result = ftp.RemoveFile(sourceFile, config);
            result.Should().BeFalse();
        }

        [Fact]
        public void UploadFile_ValidPath_Return_True()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_with_files", out SyncConfig config);
            string sourceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            var result = ftp.UploadFile(sourceFile, config);
            result.Should().BeTrue();
            string destFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DownloadUploaded.txt");

            string remoteFileName = Path.GetFileName(sourceFile);
            result = ftp.DownloadFile(remoteFileName, destFile, config);
            result.Should().BeTrue();
            File.Exists(destFile).Should().BeTrue();
        }

        [Fact]
        public void UploadFile_InvalidPath_Return_False()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_invalid_path", out SyncConfig config);
            string sourceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var result = ftp.UploadFile(sourceFile, config);
            result.Should().BeFalse();
        }

        [Fact]
        public void MoveFile_Valid_Return_True()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_with_files", out SyncConfig config);
            string sourceFile = Path.Combine(config.TransportConfig.RemotePath, "MoveMe.txt");
            string destinationFileName = Path.Combine(config.TransportConfig.RemotePath, "Moved.txt");
            var result = ftp.MoveFile(sourceFile, destinationFileName, config);
            result.Should().BeTrue();
        }

        [Fact]
        public void MoveFile_Invalid_Return_false()
        {
            ITransportAdapter ftp = GetTransportAdapter("sample_ftp_invalid_path", out SyncConfig config);
            string sourceFile = Path.Combine(config.TransportConfig.RemotePath, "MoveMe.txt");
            string destinationFileName = Path.Combine(config.TransportConfig.RemotePath, "Moved.txt");
            var result = ftp.MoveFile(sourceFile, destinationFileName, config);
            result.Should().BeFalse();
        }

        private ITransportAdapter GetTransportAdapter(string configId, out SyncConfig config)
        {
            // load config by id
            var appConfig = SyncLogService.LoadAppSyncConfig();
            config = appConfig.GetConfig(configId);
            config.Should().NotBeNull();

            // load transport config
            var transportConfig = config.TransportConfig;
            transportConfig.Should().NotBeNull();

            // generate transport adapter instance
            ITransportAdapter ftp = SyncEngine.GetTransportAdapter(config.TransportAdapter);
            ftp.Should().NotBeNull();

            return ftp;
        }
    }
}
