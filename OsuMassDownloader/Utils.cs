using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuMassDownloader
{
    public class Utils
    {
        public class MapFilter
        {
            public string Field { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }

            public MapFilter(string field, string @operator, string value) {
                Field = field;
                Operator = @operator;
                Value = value;
            }

            public override string ToString() {
                return $"{Field} {Operator} {Value}";
            }
        }

        private static readonly string[] OPERATORS = new[] {
            ">=",
            "<=",
            ">",
            "<",
            "="
        };

        public static List<MapFilter> ParseFilters(string filter) {
            var ret = new List<MapFilter>();
            var subFilters = filter.Split(';');
            foreach (var subFilter in subFilters) {
                foreach (var op in OPERATORS) {
                    if (!subFilter.Contains(op)) continue;
                    var subSubFilters = subFilter.Split(new string[] {op}, StringSplitOptions.None);
                    ret.Add(new MapFilter(subSubFilters[0], op, subSubFilters[1]));
                    break;
                }
            }
            return ret;
        }

        public static readonly Dictionary<string, string> VALID_FIELDS = new Dictionary<string, string>() {
            {"cs", "diff_size"},
            {"od", "diff_overall"},
            {"ar", "diff_approach"},
            {"hp", "diff_drain"},
            {"mode", "mode"},
            {"bpm", "bpm"},
            {"ranked", "approved"},
            {"length", "total_length"},
            {"stars", "difficultyrating"},
            {"page", "page"},
        };

        public static readonly Dictionary<string, string> VALID_OPERATORS = new Dictionary<string, string>() {
            {">=", "_gte"},
            {"<=", "_lte"},
            {">", "_gt"},
            {"<", "_lt"},
            {"=", ""},
        };

        public static string FiltersToQuery(List<MapFilter> filters) {
            var ret = new StringBuilder();
            var first = true;
            foreach (var mapFilter in filters) {
                if (!VALID_FIELDS.ContainsKey(mapFilter.Field) && VALID_OPERATORS.ContainsKey(mapFilter.Operator))
                    continue;
                if (first) ret.Append("?");
                if (!first) ret.Append("&");
                ret.AppendFormat("{0}{1}={2}", VALID_FIELDS[mapFilter.Field], VALID_OPERATORS[mapFilter.Operator],
                    mapFilter.Value);
                first = false;
            }
            return ret.ToString();
        }
    }
}