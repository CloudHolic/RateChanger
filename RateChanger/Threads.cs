using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

using RateChanger.Beatmap;
using RateChanger.Util;

// ReSharper disable AssignNullToNotNullAttribute
namespace RateChanger
{
    public partial class MainWindow
    {
        private void Worker()
        {
            string path = "";
            bool? isChecked = false;

            isWorking = true;

            try
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    path = PathTextBox.Text;
                    isChecked = OszCheckBox.IsChecked;
                    GlobalData.Rate = RateUpDown.Value ?? 1;
                }));

                GlobalData.Directory = Path.GetDirectoryName(path);
                GlobalData.OsuName = Path.GetFileName(path);
                GlobalData.Map = new BeatmapInfo(path);
                GlobalData.Mp3Name = GlobalData.Map.Gen.AudioFilename;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var mp3Thread = new Thread(Mp3Change);
            var patternThread = new Thread(PatternChange);

            mp3Thread.Start();
            patternThread.Start();

            mp3Thread.Join();
            patternThread.Join();

            string[] delFiles = {Path.GetFileNameWithoutExtension(GlobalData.Mp3Name) + "_" + GlobalData.Rate + ".mp3",
                GlobalData.Map.Meta.ArtistUnicode + " - " + GlobalData.Map.Meta.TitleUnicode + "_" + GlobalData.Rate +
                " (" + GlobalData.Map.Meta.Creator + ") [" + GlobalData.Map.Meta.Version + "].osu"};

            if (isErrorOccurred)
            {
                MessageBox.Show(@"An error occurred. Please try again.", @"Osu! Speed Changer", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                foreach (var cur in delFiles)
                    File.Delete(Path.Combine(GlobalData.Directory, cur));
            }
            else
            {
                if (isChecked == true)
                {
                    var newDir = Path.Combine(Path.GetDirectoryName(GlobalData.Directory),
                        GlobalData.Map.Meta.ArtistUnicode + " - " + GlobalData.Map.Meta.TitleUnicode + "_" +
                        GlobalData.Rate);
                    var zipPath = Path.Combine(Path.GetDirectoryName(newDir),
                        GlobalData.Map.Meta.ArtistUnicode + " - " + GlobalData.Map.Meta.TitleUnicode + "_" +
                        GlobalData.Rate + ".osz");
                    string[] exts = { ".osu", ".mp3" };
                    
                    DirectoryUtil.CopyFolder(GlobalData.Directory, newDir);
                    DirectoryUtil.DeleteFiles(newDir, exts, delFiles);

                    ZipFile.CreateFromDirectory(newDir, zipPath);
                    Directory.Delete(newDir, true);

                    foreach (var cur in delFiles)
                        File.Delete(Path.Combine(GlobalData.Directory, cur));
                }

                MessageBox.Show(@"Finished!", @"Osu! Speed Changer", MessageBoxButtons.OK, MessageBoxIcon.None);
            }

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                RateChangerWindow.Title = "Osu! Speed Changer by CloudHolic";
                PathTextBox.Text = "";
                RateUpDown.Value = 1;
            }));

            isErrorOccurred = isWorking = false;
        }

        private void Mp3Change()
        {
            try
            {
                var curPath = Path.Combine(GlobalData.Directory, GlobalData.Mp3Name);
                var newPath = Path.Combine(GlobalData.Directory,
                    Path.GetFileNameWithoutExtension(GlobalData.Mp3Name) + "_" + GlobalData.Rate + ".mp3");

                var psInfo = new ProcessStartInfo("process.bat")
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    Arguments =
                        "\"" + curPath + "\" \"" + newPath + "\" \"-rate=" + Math.Round(GlobalData.Rate * 100 - 100) +
                        "\""
                };

                var process = Process.Start(psInfo);
                process?.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isErrorOccurred = true;
            }
        }

        private void PatternChange()
        {
            try
            {
                var curPath = Path.Combine(GlobalData.Directory, GlobalData.OsuName);
                var newPath = Path.Combine(GlobalData.Directory,
                    GlobalData.Map.Meta.ArtistUnicode + " - " + GlobalData.Map.Meta.TitleUnicode + "_" + GlobalData.Rate +
                    " (" + GlobalData.Map.Meta.Creator + ") [" + GlobalData.Map.Meta.Version + "].osu");

                var fileString = File.ReadAllLines(curPath);

                for (var i = 0; i < fileString.Length; i++)
                {
                    //  General
                    if (fileString[i].StartsWith("AudioFilename:"))
                    {
                        fileString[i] = "AudioFilename: " +
                                        Path.GetFileNameWithoutExtension(GlobalData.Map.Gen.AudioFilename) +
                                        "_" + GlobalData.Rate + ".mp3";
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
                    if (fileString[i].StartsWith("Title:"))
                    {
                        fileString[i] = "Title:" + GlobalData.Map.Meta.Title + "_" + GlobalData.Rate;
                        continue;
                    }
                    if (fileString[i].StartsWith("TitleUnicode:"))
                    {
                        fileString[i] = "TitleUnicode:" + GlobalData.Map.Meta.TitleUnicode + "_" + GlobalData.Rate;
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
                            if (fileString[i + j + 1] == "")
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
                                                    (GlobalData.Map.Timing[j].MsPerBeat > 0
                                                        ? Convert.ToString(
                                                            GlobalData.Map.Timing[j].MsPerBeat / GlobalData.Rate,
                                                            CultureInfo.CurrentCulture)
                                                        : Convert.ToString(GlobalData.Map.Timing[j].MsPerBeat,
                                                            CultureInfo.CurrentCulture)) +
                                                    "," + GlobalData.Map.Timing[j].Meter + "," +
                                                    GlobalData.Map.Timing[j].SampleType + "," +
                                                    GlobalData.Map.Timing[j].SampleSet + "," +
                                                    GlobalData.Map.Timing[j].Volume + "," +
                                                    (GlobalData.Map.Timing[j].Inherited ? "1" : "0") + "," +
                                                    (GlobalData.Map.Timing[j].Kiai ? "1" : "0");
                        continue;
                    }

                    //  HitObjects
                    if (fileString[i] == "[HitObjects]")
                    {
                        for (var j = 0; j < GlobalData.Map.Hits.Count; j++)
                        {
                            var cur = fileString[i + j + 1].Split(',');
                            cur[2] = Convert.ToString((int)(Convert.ToInt32(cur[2]) / GlobalData.Rate));

                            if (cur.Length == 7)
                                cur[6] = Convert.ToString((int)(Convert.ToInt32(cur[5]) / GlobalData.Rate));
                            else if (cur.Length == 6)
                            {
                                var addition = cur[5].Split(':');
                                if (addition.Length == 6)
                                {
                                    addition[0] = Convert.ToString((int)(Convert.ToInt32(addition[0]) / GlobalData.Rate));
                                    cur[5] = string.Join(":", addition);
                                }
                            }
                            fileString[i + j + 1] = string.Join(",", cur);
                        }
                    }
                }

                File.WriteAllLines(newPath, fileString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isErrorOccurred = true;
            }
        }
    }
}
