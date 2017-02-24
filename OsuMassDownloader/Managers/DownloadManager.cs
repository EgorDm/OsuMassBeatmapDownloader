using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuMapDownload;
using OsuMapDownload.Models;
using OsuMapDownload.Providers;

namespace OsuMassDownloader.Managers
{
    public static class DownloadManager
    {
        public static readonly object QUEUE_LOCK = new object();

        public static readonly SynchronizedCollection<MapsetDownloadTitle> QUEUE = new SynchronizedCollection<MapsetDownloadTitle>();
        public static readonly SynchronizedCollection<MapsetDownloadTitle> DOWNLOADING = new SynchronizedCollection<MapsetDownloadTitle>();
        public static readonly SynchronizedCollection<MapsetDownloadTitle> COMPLETED = new SynchronizedCollection<MapsetDownloadTitle>();

        public static readonly List<string> ERRORS = new List<string>();

        public static OsuDownloadProvider OsuProvider { get; set; }
        public static B1oodcatDownloadProvider B1oodcatProvider { get; set; }

        private static void UpdateDownload() {
            lock (QUEUE_LOCK) {
                while (QUEUE.Count > 0 && DOWNLOADING.Count < int.Parse(Program.Config["download"]["threads"])) {
                    var ms = QUEUE[0];
                    QUEUE.Remove(ms);
                    DOWNLOADING.Add(ms);
                    var task = ms.GetTask();
                    var cont = task.ContinueWith(delegate(Task task1) {
                        DOWNLOADING.Remove(ms);
                        COMPLETED.Add(ms);
                        if (ms.Status == MapsetDownloadStatus.Failed) {
                            if (ms.DownloadProvider == OsuProvider) {
                                ERRORS.Add(
                                    $"Failed downloading mapset: {ms.DownloadProvider.GetUrl(ms)}. Retrying via bloodcat.");
                                COMPLETED.Remove(ms);
                                ms.Reset(B1oodcatProvider);
                                QUEUE.Add(ms);
                            } else {
                                ERRORS.Add("Failed downloading mapset: " + ms.DownloadProvider.GetUrl(ms));
                            }
                        }
                        UpdateDownload();
                    });
                    ms.DownloadProvider.StartDownloadTask(task, ms);
                }
            }
        }

        public static void StartDownload() {
            DownloadUtils.SetThreadCountMax();
            if (!Directory.Exists(Program.DownloadPath)) {
                Directory.CreateDirectory(Program.DownloadPath);
            }
            UpdateDownload();
        }

        public static BeatmapDownloadProvider GetProvider() {
            if (OsuProvider != null) return OsuProvider;
            return B1oodcatProvider;
        }

        public static void DrawDownloads() {
            lock (QUEUE_LOCK) {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(
                    @"╒═════════════════════════════╤═══════════════Downloads════════════════════════════════════════════╕");
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (DOWNLOADING.Count > 0)
                    foreach (var download in DOWNLOADING.ToArray()) {
                        RenderDownload(download);
                    }
                Console.ForegroundColor = ConsoleColor.White;
                if (QUEUE.Count > 0)
                    foreach (var dl in QUEUE.ToArray()) {
                        RenderDownload(dl);
                    }
                Console.ForegroundColor = ConsoleColor.Green;
                if (COMPLETED.Count > 0)
                    foreach (var dl in COMPLETED.ToArray()) {
                        RenderDownload(dl);
                    }
                Console.ForegroundColor = ConsoleColor.Red;
                if (ERRORS.Count > 0)
                    foreach (var error in ERRORS.ToArray()) {
                        var err = error;
                        if (error.Length > 98) {
                            err = error.Substring(0, 98);
                        } else {
                            err += new StringBuilder().Append(' ', 98 - error.Length);
                        }
                        Console.WriteLine($@"║{err}║");
                    }
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(
                    @"╘═════════════════════════════╧═══════════════Downloads════════════════════════════════════════════╝");
                Console.ResetColor();
            }
        }

        public const int PROG_LEN = 68;

        public static void RenderDownload(MapsetDownloadTitle download) {
            var title = download.Title;
            if (download.Title.Length > 29) {
                title = download.Title.Substring(0, 29);
            } else {
                title += new StringBuilder().Append(' ', 29 - download.Title.Length);
            }

            var progress = new StringBuilder();
            progress.Append('█', (int) (download.Progress * PROG_LEN));
            progress.Append('░', PROG_LEN - (int) (download.Progress * PROG_LEN));
            Console.WriteLine($@"║{title}║{progress}║");
        }
    }
}