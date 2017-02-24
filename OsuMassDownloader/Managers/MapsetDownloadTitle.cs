using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuMapDownload;
using OsuMapDownload.Models;

namespace OsuMassDownloader.Managers
{
    public class MapsetDownloadTitle : MapsetDownload
    {
        public string Title { get; private set; }
        public MapsetDownloadTitle(int id, string path, BeatmapDownloadProvider provider, string title) : base(id, path, provider) {
            Title = title;
        }
    }
}
