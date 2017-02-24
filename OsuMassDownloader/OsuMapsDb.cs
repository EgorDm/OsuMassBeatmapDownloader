using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using osu_database_reader;
using OsuMassDownloader.Models;

namespace OsuMassDownloader
{
    public class OsuMapsDb
    {
        public Dictionary<int, Beatmap> Beatmaps;

        public static OsuMapsDb Read(string path) {
            var db = new OsuMapsDb();
            using (var r = new CustomReader(File.OpenRead(path))) {
                var version = r.ReadInt32();
                r.ReadInt32();
                r.ReadBoolean();
                r.ReadDateTime();
                r.ReadString();

                db.Beatmaps = new Dictionary<int, Beatmap>();
                var length = r.ReadInt32();
                for (var i = 0; i < length; i++) {
                    var currentIndex = (int) r.BaseStream.Position;
                    var entryLength = r.ReadInt32();

                    var entry = ReadFromReader(r, version);
                    if (!db.Beatmaps.ContainsKey(entry.SetId))
                        db.Beatmaps.Add(entry.SetId, entry);
                    if (r.BaseStream.Position != currentIndex + entryLength + 4) {
                        Debug.Fail(
                            $"Length doesn't match, {r.BaseStream.Position} instead of expected {currentIndex + entryLength + 4}");
                    }
                }
                r.ReadByte();
            }
            return db;
        }

        public static Beatmap ReadFromReader(CustomReader r, int version = 20160729) {
            var e = new Beatmap();
            e.Artist = r.ReadString();
            r.ReadString();
            e.Title = r.ReadString();
            r.ReadString();
            r.ReadString();
            e.Version = r.ReadString();
            r.ReadString();
            r.ReadString();
            r.ReadString();

            r.ReadByte();
            r.ReadUInt16();
            r.ReadUInt16();
            r.ReadUInt16();
            r.ReadDateTime();

            if (version >= 20140609) {
                r.ReadSingle();
                r.ReadSingle();
                r.ReadSingle();
                r.ReadSingle();
            } else {
                r.ReadByte();
                r.ReadByte();
                r.ReadByte();
                r.ReadByte();
            }
            r.ReadDouble();

            if (version >= 20140609) {
                r.ReadModsDoubleDictionary();
                r.ReadModsDoubleDictionary();
                r.ReadModsDoubleDictionary();
                r.ReadModsDoubleDictionary();
            }

            r.ReadInt32();
            r.ReadInt32();
            r.ReadInt32();

            r.ReadTimingPointList();
            e.ID = r.ReadInt32();
            e.SetId = r.ReadInt32();
            r.ReadInt32();

            r.ReadByte();
            r.ReadByte();
            r.ReadByte();
            r.ReadByte();

            r.ReadInt16();
            r.ReadSingle();
            r.ReadByte();

            r.ReadString();
            r.ReadString();

            r.ReadInt16();
            r.ReadString();
            r.ReadBoolean();
            r.ReadDateTime();


            r.ReadBoolean();
            r.ReadString();
            r.ReadDateTime();

            r.ReadBoolean();
            r.ReadBoolean();
            r.ReadBoolean();
            r.ReadBoolean();
            r.ReadBoolean();
            if (version < 20140609)
                r.ReadInt16();
            r.ReadInt32();
            r.ReadByte();

            return e;
        }
    }
}