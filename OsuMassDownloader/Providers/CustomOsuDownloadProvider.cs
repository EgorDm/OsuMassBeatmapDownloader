using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OsuMapDownload.Models;
using OsuMapDownload.Providers;

namespace OsuMassDownloader.Providers
{
    public class CustomOsuDownloadProvider : OsuDownloadProvider
    {
        public int Timeout { get; private set; }

        public CustomOsuDownloadProvider(string username, string password, CookieContainer cookies, int timeout, string cookieFilePath = null) : base(username, password, cookies, cookieFilePath) {
            Timeout = timeout;
        }
        public CustomOsuDownloadProvider(string username, string password, int timeout, string cookiePath = null) : base(username, password, cookiePath) {
            Timeout = timeout;
        }

        public override WebRequest PrepareRequest(MapsetDownload download) {
            var webRequest = (HttpWebRequest)WebRequest.Create(GetUrl(download));
            webRequest.Timeout = Timeout;
            webRequest.CookieContainer = Cookies;
            return webRequest;
        }
    }
}
