using FluentFTP;
using SynX.Core;
using SynX.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SynX.Transport.Ftp
{
    public class FtpTransportAdapter : ITransportAdapter
    {
        private Exception exception;

        public Exception ExceptionData
        {
            get { return exception; }
            set { exception = value; }
        }


        public bool DownloadFile(string remoteFileName, string localFileName, SyncConfig syncConfig)
        {
            var transport = syncConfig.TransportConfig;
            var results = new List<string>();
            var remotePath = transport.RemotePath;
            if (!string.IsNullOrEmpty(syncConfig.RemotePath)) remotePath = syncConfig.RemotePath;

            using (var client = new FtpClient(transport.Host, transport.Port, transport.UserName, transport.Password))
            {
                client.Connect();
                try
                {
                    client.SetWorkingDirectory(remotePath);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return false;
                }

                if (!client.FileExists(remoteFileName)) return false;
                client.DownloadFile(localFileName, remoteFileName, FtpLocalExists.Overwrite);
            }

            return true;
        }

        public List<string> GetFileList(SyncConfig syncConfig)
        {
            var transport = syncConfig.TransportConfig;
            var results = new List<string>();
            var remotePath = transport.RemotePath;
            if (!string.IsNullOrEmpty(syncConfig.RemotePath)) remotePath = syncConfig.RemotePath;

            using (var client = new FtpClient(transport.Host, transport.Port, transport.UserName, transport.Password))
            {
                client.Connect();
                try
                {
                    client.SetWorkingDirectory(remotePath);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return results;
                }

                foreach (var item in client.GetListing(remotePath))
                {
                    if (item.Type == FtpFileSystemObjectType.File)
                    {
                        if (Path.GetExtension(item.FullName).ToLower() != ".xml")
                            continue;

                        results.Add(item.FullName);
                    }
                }
            }

            return results;
        }

        public bool MoveFile(string remoteSourceFileName, string remoteDestinationFileName, SyncConfig syncConfig)
        {
            var transport = syncConfig.TransportConfig;
            var remotePath = transport.RemotePath;
            if (!string.IsNullOrEmpty(syncConfig.RemotePath)) remotePath = syncConfig.RemotePath;

            using (var client = new FtpClient(transport.Host, transport.Port, transport.UserName, transport.Password))
            {
                client.Connect();
                try
                {
                    client.SetWorkingDirectory(remotePath);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return false;
                }

                if (!client.FileExists(remoteSourceFileName)) return false;
                client.Rename(remoteSourceFileName, remoteDestinationFileName);
            }

            return true;
        }

        public bool MoveToBackup(string remoteFileName, SyncConfig syncConfig)
        {
            var transport = syncConfig.TransportConfig;
            var remotePath = transport.RemotePath;
            if (!string.IsNullOrEmpty(syncConfig.RemotePath)) remotePath = syncConfig.RemotePath;

            using (var client = new FtpClient(transport.Host, transport.Port, transport.UserName, transport.Password))
            {
                client.Connect();
                try
                {
                    client.SetWorkingDirectory(remotePath);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return false;
                }

                if (!client.FileExists(remoteFileName)) return false;

                var localFileName = Path.Combine(syncConfig.BackupOutPath, Path.GetFileName(remoteFileName));
                client.DownloadFile(localFileName, remoteFileName, FtpLocalExists.Overwrite);
                if(!File.Exists(localFileName)) return false;

                client.DeleteFile(remoteFileName);
            }

            return true;
        }

        public string ReadFileContent(string remoteFileName, SyncConfig syncConfig)
        {
            var transport = syncConfig.TransportConfig;
            string result = string.Empty;
            var remotePath = transport.RemotePath;
            if (!string.IsNullOrEmpty(syncConfig.RemotePath)) remotePath = syncConfig.RemotePath;

            using (var client = new FtpClient(transport.Host, transport.Port, transport.UserName, transport.Password))
            {
                client.Connect();
                try
                {
                    client.SetWorkingDirectory(remotePath);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return string.Empty;
                }

                if (!client.FileExists(remoteFileName)) return String.Empty;

                using (var istream = client.OpenRead(remoteFileName))
                {
                    try
                    {
                        byte[] bytes = new byte[istream.Length];
                        istream.Read(bytes, 0, (int)istream.Length);
                        result = Encoding.UTF8.GetString(bytes);
                    }
                    finally
                    {
                        istream.Close();
                    }
                }
            }

            return result;
        }

        public bool RemoveFile(string remoteFileName, SyncConfig syncConfig)
        {
            var transport = syncConfig.TransportConfig;
            var remotePath = transport.RemotePath;
            if (!string.IsNullOrEmpty(syncConfig.RemotePath)) remotePath = syncConfig.RemotePath;

            using (var client = new FtpClient(transport.Host, transport.Port, transport.UserName, transport.Password))
            {
                client.Connect();
                try
                {
                    client.SetWorkingDirectory(remotePath);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return false;
                }

                if (!client.FileExists(remoteFileName)) return false;
                client.DeleteFile(remoteFileName);
            }

            return true;
        }

        public bool UploadFile(string localFileName, SyncConfig syncConfig)
        {
            var transport = syncConfig.TransportConfig;
            var fileName = Path.GetFileName(localFileName);
            var remotePath = transport.RemotePath;
            if (!string.IsNullOrEmpty(syncConfig.RemotePath)) remotePath = syncConfig.RemotePath;

            string remoteFileName = Path.Combine(remotePath, fileName);

            if (!File.Exists(localFileName)) return false;

            using (var client = new FtpClient(transport.Host, transport.Port, transport.UserName, transport.Password))
            {
                client.Connect();
                try
                {
                    client.SetWorkingDirectory(remotePath);
                }
                catch(Exception ex)
                {
                    exception = ex;
                    return false;
                }

                client.UploadFile(localFileName, remoteFileName);
            }

            return true;
        }
    }
}
