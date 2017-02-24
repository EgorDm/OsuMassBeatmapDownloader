using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OsuMapDownload.Models;
using OsuMapDownload.Providers;
using OsuMassDownloader.Managers;
using OsuMassDownloader.Models;
using TournamentManagerApi;

namespace OsuMassDownloader.Behaviours
{
    public class DownloadMapsBehaviour
    {
        private SearchQuery Query;
        private Dictionary<int, Beatmap> ToDownload = new Dictionary<int, Beatmap>();
        private bool _finished = false;
        private int _downloadTotal = 0;

        public DownloadMapsBehaviour() {}

        public void Start() {
            while (!CheckFilter()) {}
            while (!_finished) {
                FindMaps();
                if (ToDownload.Count > 0) {
                    DownloadMaps();
                }
                Query.Page += 1;
            }
        }

        public bool CheckFilter() {
            ConsoleExt.Log("Input a filter. Example cs>5;cs<=9;stars>4.5", LogLevel.Instruction);
            var rawFilters = Console.ReadLine();
            var filters = Utils.ParseFilters(rawFilters);
            ConsoleExt.Log("Do you want to split maps with this filter?");
            ConsoleExt.Log(string.Join(" and ", filters));
            if (!ConsoleExt.Confirm()) return false;
            Query = new SearchQuery(filters);
            return true;
        }

        public void FindMaps() {
            var resp = ApiManager.ApiCallGetAbstract<BeatmapsResponse>(Query.GetUrl());
            if (resp.Data.Count == 0) {
                _finished = true;
                return;
            }
            ConsoleExt.Log($"Recieved {resp.Data.Count} maps from the api. Results found {resp.Total}. Current Page {Query.Page}.");
            foreach (var beatmap in resp.Data) {
                if (Program.OsuDatabase.Beatmaps.ContainsKey(beatmap.SetId)) continue;
                if (!ToDownload.ContainsKey(beatmap.SetId))
                    ToDownload.Add(beatmap.SetId, beatmap);
            }
            ConsoleExt.Log($"{ToDownload.Count} Of these maps you dont have.");
        }

        public bool DownloadMaps() {
            ConsoleExt.Log($"Starting downloading of {ToDownload.Count} maps");
            lock (DownloadManager.QUEUE_LOCK) {
                foreach (var beatmap in ToDownload) {
                    DownloadManager.QUEUE.Add(new MapsetDownloadTitle(beatmap.Key, Program.DownloadPath,
                        DownloadManager.GetProvider(), $"{beatmap.Value.Artist} - {beatmap.Value.Title}"));
                }
            }
            DownloadManager.StartDownload();
            while (DownloadManager.QUEUE.Count > 0 || DownloadManager.DOWNLOADING.Count > 0) {
                Thread.Sleep(100);
                DownloadManager.DrawDownloads();
            }
            ConsoleExt.Log($"Finished downloading of {ToDownload.Count} maps");
            lock (DownloadManager.QUEUE_LOCK) {
                DownloadManager.COMPLETED.Clear();
                DownloadManager.ERRORS.Clear();
            }
            _downloadTotal += ToDownload.Count;
            ConsoleExt.Log($"Downloaded {_downloadTotal} over the whole course.");
            ToDownload.Clear();
            return true;
        }
    }
}