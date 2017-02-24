using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace OsuMassDownloader.Models
{
    public class SearchQuery
    {
        public List<Utils.MapFilter> Filter { get; set; }

        public int Page { get; set; }

        public SearchQuery(List<Utils.MapFilter> filter, int page = 1) {
            Filter = filter;
            Page = page;
        }

        public string GetUrl() {
            var ret = Utils.FiltersToQuery(Filter);
            if (ret.Equals(string.Empty)) {
                ret += "?";
            } else {
                ret += "&";
            }
            ret += $"page={Page}";
            return "http://tournament-manager.ml/api/beatmaps" + ret;
        }
    }
}
