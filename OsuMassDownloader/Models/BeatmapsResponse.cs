using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OsuMassDownloader.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BeatmapsResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }

        [JsonProperty(PropertyName = "data")]
        public List<Beatmap> Data { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public JArray Errors { get; set; }

        public BeatmapsResponse() { }
    }
}
