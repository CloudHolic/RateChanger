using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;
using NLog;
using RateChanger.Beatmap;
using RateChanger.Util;
using MessageBox = System.Windows.Forms.MessageBox;

// ReSharper disable AssignNullToNotNullAttribute
namespace RateChanger
{
    public struct ThreadStruct
    {
        public bool IsGui { get; set; }
        public string Path { get; set; }
        public bool OszChecked { get; set; }
        public bool NightCore { get; set; }
        public double Rate { get; set; }
        public string OutPutDir { get; set; }
    }

    public class WorkerThread
    {
        private static volatile WorkerThread _instance;
        private static readonly object Lock = new object();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public bool IsWorking { get; private set; }
        public bool IsErrorOccurred { get; set; }

        public static WorkerThread Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        if(_instance == null)
                            _instance = new WorkerThread();
                    }
                }
                return _instance;
            }
        }
        
        // Return after worker thread stops.
        public void StartWorker(ThreadStruct info)
        {
            var thread = new Thread(Worker);
            thread.Start(info);
            thread.Join();
        }

        private void Worker(object threadInfo)
        {
            string[] invalidString = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };

            IsWorking = true;
            var info = (ThreadStruct) threadInfo;

            try
            {
                GlobalData.Directory = Path.GetDirectoryName(info.Path);
                GlobalData.OutputDir = info.OutPutDir;
                GlobalData.OsuName = Path.GetFileName(info.Path);
                GlobalData.Map = new BeatmapInfo(info.Path);
                GlobalData.Mp3Name = GlobalData.Map.Gen.AudioFilename;
                GlobalData.Rate = info.Rate;
                GlobalData.Nightcore = info.NightCore;
                GlobalData.NewOsuName = GlobalData.Map.Meta.Artist + " - " + GlobalData.Map.Meta.Title + " (" +
                    GlobalData.Map.Meta.Creator + ") [" + GlobalData.Map.Meta.Version + " x" + GlobalData.Rate +
                    (GlobalData.Nightcore ? "_P" : "") + "].osu";

                foreach (var cur in invalidString)
                    GlobalData.NewOsuName = GlobalData.NewOsuName.Replace(cur, "");
            }
            catch (Exception ex)
            {
                if(info.IsGui)
                    MessageBox.Show(ex.Message, @"Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    Log.Error(ex, "Error occurred while getting GlobalData.");
            }

            var mp3Thread = new Thread(Mp3Change);
            var patternThread = new Thread(PatternChange);

            mp3Thread.Start(info.IsGui);
            patternThread.Start(info.IsGui);

            mp3Thread.Join();
            patternThread.Join();

            string[] delFiles = { Path.GetFileNameWithoutExtension(GlobalData.Mp3Name) + "_" + GlobalData.Rate +
                    (GlobalData.Nightcore ? "_P": "") + ".mp3", GlobalData.NewOsuName };

            if (IsErrorOccurred)
            {
                if(info.IsGui)
                    MessageBox.Show(@"An error occurred. Please try again.", @"Osu! Beatmap Rate Converter (made by CloudHolic)",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    Log.Error("Error occurred while running sub-threads.");

                foreach (var cur in delFiles)
                    File.Delete(Path.Combine(GlobalData.Directory, cur));
            }
            else
            {
                string[] exts = { ".osu", ".mp3", ".osb" };

                if (info.OszChecked)
                {
                    var newDir = GlobalData.Map.Meta.Artist + " - " + GlobalData.Map.Meta.Title + " x" +
                        GlobalData.Rate + (GlobalData.Nightcore ? "_P" : "");
                    var zipFile = GlobalData.Map.Meta.Artist + " - " + GlobalData.Map.Meta.Title + " x" +
                        GlobalData.Rate + (GlobalData.Nightcore ? "_P" : "") + ".osz";
                    string newPath, zipPath;

                    foreach (var cur in invalidString)
                    {
                        newDir = newDir.Replace(cur, "");
                        zipFile = zipFile.Replace(cur, "");
                    }

                    if (GlobalData.OutputDir == GlobalData.Directory)
                    {
                        newPath = Path.Combine(Path.GetDirectoryName(GlobalData.Directory), newDir);
                        zipPath = Path.Combine(Path.GetDirectoryName(GlobalData.Directory), zipFile);
                    }
                    else
                    {
                        newPath = Path.Combine(GlobalData.OutputDir, newDir);
                        zipPath = Path.Combine(GlobalData.OutputDir, zipFile);
                    }

                    DirectoryUtil.CopyFolder(GlobalData.Directory, newPath, exts);
                    foreach (var cur in delFiles)
                        File.Move(Path.Combine(GlobalData.Directory, cur), Path.Combine(newPath, cur));

                    ZipFile.CreateFromDirectory(newPath, zipPath);
                    Directory.Delete(newPath, true);
                }
                else if (GlobalData.OutputDir != GlobalData.Directory)
                {
                    DirectoryUtil.CopyFolder(GlobalData.Directory, GlobalData.OutputDir, exts);

                    foreach (var cur in delFiles)
                        File.Move(Path.Combine(GlobalData.Directory, cur), Path.Combine(GlobalData.OutputDir, cur));
                }

                if(info.IsGui)
                    MessageBox.Show(@"Finished!", @"Osu! Beatmap Rate Converter (made by CloudHolic)", MessageBoxButtons.OK, MessageBoxIcon.None);
                else
                    Log.Info($"Finished request {GlobalData.OsuName} to {GlobalData.NewOsuName}.");
            }

            IsErrorOccurred = IsWorking = false;
        }

        private void Mp3Change(object isGui)
        {
            try
            {
                var curPath = Path.Combine(GlobalData.Directory, GlobalData.Mp3Name);
                var newPath = Path.Combine(GlobalData.Directory, Path.GetFileNameWithoutExtension(GlobalData.Mp3Name) +
                    "_" + GlobalData.Rate + (GlobalData.Nightcore ? "_P" : "") + ".mp3");

                var psInfo = new ProcessStartInfo("process.bat")
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Ref"),
                    Arguments = "\"" + curPath + "\" \"" + newPath + (GlobalData.Nightcore ? "\" \"-rate=" : "\" \"-tempo=") +
                        Math.Round((GlobalData.Rate * 100) - 100) + "\""
                };

                var process = Process.Start(psInfo);
                process?.WaitForExit();
            }
            catch (Exception ex)
            {
                if((bool)isGui)
                    MessageBox.Show(ex.Message, @"Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    Log.Error(ex, "Error occurred while changing MP3.");
                IsErrorOccurred = true;
            }
        }

        private void PatternChange(object isGui)
        {
            try
            {
                var curPath = Path.Combine(GlobalData.Directory, GlobalData.OsuName);
                var newPath = Path.Combine(GlobalData.Directory, GlobalData.NewOsuName);

                var fileString = File.ReadAllLines(curPath);
                for (var i = 0; i < fileString.Length; i++)
                {
                    //  General
                    if (fileString[i].StartsWith("AudioFilename:"))
                    {
                        fileString[i] = "AudioFilename: " + Path.GetFileNameWithoutExtension(GlobalData.Map.Gen.AudioFilename) +
                            "_" + GlobalData.Rate + (GlobalData.Nightcore ? "_P" : "") + ".mp3";
                        continue;
                    }
                    if (fileString[i].StartsWith("PreviewTime:"))
                    {
                        fileString[i] = "PreviewTime: " + (int)(GlobalData.Map.Gen.PreviewTime / GlobalData.Rate);
                        continue;
                    }

                    //  Editor
                    if (fileString[i].StartsWith("Bookmarks:"))
                    {
                        fileString[i] = "Bookmarks: ";
                        foreach (var cur in GlobalData.Map.Edit.Bookmarks)
                            fileString[i] += (int)(cur / GlobalData.Rate) + ",";
                        fileString[i].Remove(fileString[i].Length - 1);
                        continue;
                    }

                    //  Metadata
                    if (fileString[i].StartsWith("Version:"))
                    {
                        fileString[i] = "Version:" + GlobalData.Map.Meta.Version + " x" + GlobalData.Rate +
                            (GlobalData.Nightcore ? "_P" : "");
                        continue;
                    }
                    if (fileString[i].StartsWith("BeatmapID"))
                    {
                        fileString[i] = "BeatmapID:-1";
                        continue;
                    }
                    if (fileString[i].StartsWith("BeatmapSetID:"))
                    {
                        fileString[i] = "BeatmapSetID:-1";
                        continue;
                    }

                    //  Events
                    if (fileString[i] == @"//Storyboard Sound Samples")
                    {
                        for (var j = 0; ; j++)
                        {
                            if (fileString[i + j + 1] == "" || fileString[i + j + 1].StartsWith(@"//"))
                                break;

                            var cur = fileString[i + j + 1].Split(',');
                            cur[1] = Convert.ToString((int)(Convert.ToInt32(cur[1]) / GlobalData.Rate));
                            fileString[i + j + 1] = string.Join(",", cur);
                        }
                    }

                    //  TimingPoints
                    if (fileString[i] == "[TimingPoints]")
                    {
                        for (var j = 0; j < GlobalData.Map.Timing.Count; j++)
                            fileString[i + j + 1] = (int)(GlobalData.Map.Timing[j].Offset / GlobalData.Rate) + "," +
                                (GlobalData.Map.Timing[j].MsPerBeat > 0 ? Convert.ToString(GlobalData.Map.Timing[j].MsPerBeat /
                                GlobalData.Rate, CultureInfo.CurrentCulture) : Convert.ToString(GlobalData.Map.Timing[j].MsPerBeat,
                                CultureInfo.CurrentCulture)) + "," + GlobalData.Map.Timing[j].Meter + "," +
                                GlobalData.Map.Timing[j].SampleType + "," + GlobalData.Map.Timing[j].SampleSet + "," +
                                GlobalData.Map.Timing[j].Volume + "," + (GlobalData.Map.Timing[j].Inherited ? "1" : "0") + "," +
                                (GlobalData.Map.Timing[j].Kiai ? "1" : "0");
                        continue;
                    }

                    //  HitObjects
                    if (fileString[i] == "[HitObjects]")
                    {
                        for (var j = i + 1; j < fileString.Length; j++)
                        {
                            if (string.IsNullOrEmpty(fileString[j]))
                                break;

                            var cur = fileString[j].Split(',');
                            cur[2] = Convert.ToString((int)(Convert.ToInt32(cur[2]) / GlobalData.Rate));

                            if (cur.Length == 7)
                                cur[5] = Convert.ToString((int)(Convert.ToInt32(cur[5]) / GlobalData.Rate));
                            else if (cur.Length == 6)
                            {
                                var addition = cur[5].Split(':');
                                addition[0] = Convert.ToString((int)(Convert.ToInt32(addition[0]) / GlobalData.Rate));
                                cur[5] = string.Join(":", addition);
                                if (addition.Length == 5)
                                    cur[5] += ":";
                            }
                            fileString[j] = string.Join(",", cur);
                        }
                    }
                }

                File.WriteAllLines(newPath, fileString);
            }
            catch (Exception ex)
            {
                if((bool)isGui)
                    MessageBox.Show(ex.Message, @"Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    Log.Error(ex, "Error occurred while changing pattern.");
                IsErrorOccurred = true;
            }
        }
    }
}

