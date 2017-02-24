using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu_database_reader;
using OsuMapDownload.Providers;
using OsuMassDownloader.Behaviours;
using OsuMassDownloader.Managers;
using OsuMassDownloader.Models;
using OsuMassDownloader.Providers;
using TournamentManagerApi;

namespace OsuMassDownloader
{
    internal class Program
    {
        public static readonly string MY_PATH =
            Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public static readonly string COOKIE_PATH = $"{MY_PATH}/cookies.txt";

        public static IniData Config { get; private set; }
        public static OsuMapsDb OsuDatabase { get; set; }
        public static string DownloadPath => Config["osu"]["path"] + "\\Songs";


        [STAThread]
        private static void Main(string[] args) {
            Console.WindowWidth = 100;
            var parser = new FileIniDataParser();
            Config = parser.ReadFile("config.ini");
            if (!InitApi()) return;
            if (!InitDownloads()) return;
            if (!InitDb()) return;

            while (true) {
                var test = new DownloadMapsBehaviour();
                test.Start();
                if (!InitDb()) return;
            }

            ConsoleExt.Die();
        }

        private static bool InitApi() {
            ApiManager.Init("2", "vNtt78Sy1VWHnzfKjfzhZZBe6y8QOXLDvBaFIcPu", "*");
            var proceed = false;
            var task = ApiManager.PrepareToken(b => { proceed = b; });
            task.RunSynchronously();
            if (!proceed) {
                ConsoleExt.Log("Could not establish connection with the api. Please make sure you authorize this app",
                    LogLevel.Error);
                ConsoleExt.Die();
                return false;
            }
            ConsoleExt.Log("Succesfully connected to api.");
            return true;
        }

        private static bool InitDownloads() {
            DownloadManager.OsuProvider = new CustomOsuDownloadProvider(Config["osu"]["username"], Config["osu"]["password"], int.Parse(Config["download"]["timeout"]), COOKIE_PATH);
            if (!string.IsNullOrEmpty(Config["osu"]["username"]) && !string.IsNullOrEmpty(Config["osu"]["password"])) {
                DownloadManager.OsuProvider.CheckOrLogin();
                DownloadManager.OsuProvider.LoginTask.Wait();
            }
            if (!DownloadManager.OsuProvider.LoggedIn) {
                ConsoleExt.Log(
                    "Could not login into osu with given credentials. Please check config.ini.\nUsing Bloodcat as standard.",
                    LogLevel.Warning);
            } else {
                ConsoleExt.Log("Logged into osu succesfully. Using osu as standard download location.");
            }
            DownloadManager.B1oodcatProvider = new B1oodcatDownloadProvider();
            ConsoleExt.Log("Download api succesfully setup.");
            return true;
        }

        private static bool InitDb() {
            try {
                OsuDatabase = OsuMapsDb.Read(Config["osu"]["path"] + "\\osu!.db");
                ConsoleExt.Log("Succesfully read database.");
                return true;
            } catch (Exception e) {
                ConsoleExt.Log(
                    "Wrong osu path. Could not find " + Config["osu"]["path"] + "\\osu!.db",
                    LogLevel.Error);
                ConsoleExt.Log(e.Message, LogLevel.Error);
                ConsoleExt.Die();
                return false;
            }
        }
    }
}