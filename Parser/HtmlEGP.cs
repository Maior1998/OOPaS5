using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace OOPaS5.Parser
{
    public partial class HtmlEGP : EGP
    {
        private const string BaseUrl = "https://tv.mail.ru";
        private const int PauseTimeout = 200;
        private static readonly HtmlParser parser = new HtmlParser();

        private HtmlDataProvider provider;
        public HtmlEGP(HtmlDataProvider provider)
        {
            this.provider = provider;
        }

        protected override object GatherData()
        {
            return provider.GetData();
        }

        protected override TvStreamInfo[] ProcessData(object source)
        {
            Dictionary<int, string> sourceDictionary = (Dictionary<int, string>)source;
            TvStreamInfo[] res = sourceDictionary.Select(pair => new TvStreamInfo() { channelId = pair.Key }).ToArray();
            foreach (TvStreamInfo streamInfo in res)
            {
                IHtmlDocument document = parser.ParseDocument(sourceDictionary[streamInfo.channelId]);
                IHtmlCollection<IElement> times = document.QuerySelectorAll(
                    "[class=\"p-programms__item__time\"]");
                IHtmlCollection<IElement> names = document.QuerySelectorAll(
                    "[class=\"p-programms__item__name\"]");
                streamInfo.channelName = document
                    .QuerySelector("[class=\"hdr__inner\"]")
                    .TextContent.Split('—')[0].Trim();
                int length = times.Length;
                streamInfo.records = new TvShow[length];
                for (int j = 0; j < length; j++)
                {
                    streamInfo.records[j] = new TvShow()
                    {
                        startTime = DateTime.Parse(times[j].TextContent),
                        title = names[j].TextContent
                    };
                }
            }

            foreach (TvStreamInfo streamInfo in res)
            {
                for (int i = 0; i < streamInfo.records.Length - 1; i++)
                    streamInfo.records[i].endTime = streamInfo.records[i + 1].startTime;
                if (streamInfo.records.Length != 0)
                    streamInfo.records[streamInfo.records.Length - 1].endTime =
                        streamInfo.records[streamInfo.records.Length - 1].startTime.AddHours(1);
            }

            return res;
        }

        private static string GetHtml(string htmLPath)
        {
            Thread.Sleep(PauseTimeout);
            Console.Write($"donloading {htmLPath} . . . ");
            using HttpClient client = new HttpClient();
            string content = client.GetStringAsync(htmLPath).Result;
            Console.WriteLine("done.");
            return content;
        }

        private static readonly Regex linksRegex = new Regex(@"channel/(\d+)/");
        /// <summary>
        /// Extract all anchor tags using AngleSharp
        /// </summary>
        private static string[] GetAllLinks(string hmtlSource)
        {
            IHtmlDocument document = parser.ParseDocument(hmtlSource);
            return document.QuerySelectorAll("a").Select(element => element.GetAttribute("href")).Where(link => !(link is null) && linksRegex.IsMatch(link)).Distinct().ToArray();
        }

    }
}
