using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OsuMassDownloader.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Beatmap
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "beatmapset_id")]
        public int SetId { get; set; }
        [JsonProperty(PropertyName = "artist")]
        public string Artist { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        public Beatmap() {}

        public override string ToString() {
            return $"{SetId}/{ID} {Artist} - {Title} [{Version}]";
        }
    }
}
