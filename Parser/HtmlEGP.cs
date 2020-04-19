using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace OOPaS5.Parser
{
    public class HtmlEGP : EGP
    {
        private const string BaseUrl = "https://tv.mail.ru";
        private const int PauseTimeout = 500;
        private static readonly HtmlParser parser = new HtmlParser();
        protected override object GatherData()
        {
            string[] links = GetAllLinks(GetHtml(BaseUrl));
            Dictionary<int,string> res=new Dictionary<int, string>();
            foreach (string link in links)
            {
                string buffer = GetHtml(BaseUrl + link);
                IHtmlDocument doc = parser.ParseDocument(buffer);
                int id = int.Parse(linksRegex.Match(doc.QuerySelector("link").GetAttribute("href")).Groups[1].Value);
                res.Add(id,buffer);
            }
            return res;
        }

        protected override TvStreamInfo[] ProcessData(object source)
        {
            Dictionary<int, string> sourceDictionary = (Dictionary<int, string>) source;
            TvStreamInfo[] res = sourceDictionary.Select(pair=>new TvStreamInfo(){channelId = pair.Key}).ToArray();
            foreach (TvStreamInfo streamInfo in res)
            {
                IHtmlDocument document = parser.ParseDocument(sourceDictionary[streamInfo.channelId]);
                IHtmlCollection<IElement> times = document.QuerySelectorAll(
                    "[class=\"p-programms__item__time\"]");
                IHtmlCollection<IElement> names = document.QuerySelectorAll(
                    "[class=\"p-programms__item__name\"]");
                int length = times.Length;
                streamInfo.records=new TvShow[length];
                for (int j = 0; j < length; j++)
                {
                    streamInfo.records[j]=new TvShow()
                    {
                        startTime = DateTime.Parse(times[j].TextContent),
                        title=names[j].TextContent
                    };
                }
            }

            foreach (TvStreamInfo streamInfo in res)
            {
                for (int i = 0; i < streamInfo.records.Length - 1; i++)
                    streamInfo.records[i].endTime = streamInfo.records[i + 1].startTime;
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

        private static readonly Regex linksRegex=new Regex(@"channel/(\d+)/");
        /// <summary>
        /// Extract all anchor tags using AngleSharp
        /// </summary>
        private static string[] GetAllLinks(string hmtlSource)
        {
            IHtmlDocument document = parser.ParseDocument(hmtlSource);
            return document.QuerySelectorAll("a").Select(element => element.GetAttribute("href")).Where(link=>!(link is null) && linksRegex.IsMatch(link)).Distinct().ToArray();
        }

    }
}
