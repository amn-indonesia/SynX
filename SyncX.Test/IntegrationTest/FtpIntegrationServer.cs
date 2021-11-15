using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynX.Test.IntegrationTest
{
    public class FtpIntegrationServer : IDisposable
    {
        private readonly Process ftpProcess;

        public FtpIntegrationServer(string rootDirectory, int port = 21, bool hideFtpWindow = false)
        {
            var psInfo = new ProcessStartInfo
            {
                FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IntegrationTest", "ftpdmin.exe"),
                Arguments = String.Format("-p {0} -ha 127.0.0.1 \"{1}\"", port, rootDirectory),
                WindowStyle = hideFtpWindow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal
            };
            ftpProcess = Process.Start(psInfo);
        }

        public void Dispose()
        {
            if (!ftpProcess.HasExited)
            {
                ftpProcess.Kill();
                ftpProcess.WaitForExit();
            }
        }
    }
}
