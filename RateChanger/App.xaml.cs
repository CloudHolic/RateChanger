using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using NLog;
using Application = System.Windows.Application;

namespace RateChanger
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private const string FileDownloadPath = "C:/CloudBot/Downloads";
        private const string FileOutputPath = "C:/CloudBot/Outputs";

        // ReSharper disable once InconsistentNaming
        private const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                if (AttachConsole(ATTACH_PARENT_PROCESS))
                {
                    var mapsetId = int.Parse(e.Args[0]);
                    var rate = double.Parse(e.Args[1]);
                    var nightcore = (e.Args[2] + e.Args[3]).Contains("n");

                    var filePath = Path.Combine(FileDownloadPath, $"{mapsetId}.osz");
                    using (var client = new WebClient())
                    {
                        var url = "https://osu.ppy.sh/d/" + mapsetId;
                        client.DownloadFile(url, filePath);
                    }

                    var threadInfo = new ThreadStruct
                    {
                        IsGui = false,
                        Path =  filePath,
                        OszChecked = true,
                        NightCore = nightcore,
                        Rate = rate,
                        OutPutDir = FileOutputPath
                    };
                    WorkerThread.Instance.StartWorker(threadInfo);
                    Console.WriteLine($@"{WorkerThread.Instance.IsErrorOccurred},");
                    
                    FreeConsole();
                    SendKeys.SendWait("{Enter}");
                    Shutdown();
                }
            }
            else
            {
                base.OnStartup(e);
            }
        }
    }
}
