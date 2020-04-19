using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Win32;

namespace OOPaS5.Parser
{
    public class XmlEGP : EGP
    {
        
        protected override object GatherData()
        {
            string filePath = ChoosePath();
            return File.ReadAllText(filePath);
        }
        private static string ChoosePath()
        {
            OpenFileDialog file = new OpenFileDialog()
            {
                Title = "Choose source file",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            file.ShowDialog();
            return file.FileName;
        }
        protected override TvStreamInfo[] ProcessData(object source)
        {
            XDocument doc = XDocument.Parse(source.ToString());
            XElement nodePointer = doc.Root;

            IEnumerable<XElement> channelsNodes = nodePointer.Descendants(XName.Get("channel"));
            IEnumerable<XElement> programmsNodes = nodePointer.Descendants(XName.Get("programme"));
            List<TvStreamInfo> channels = new List<TvStreamInfo>();
            foreach (XElement xElement in channelsNodes)
            {
                Console.WriteLine(xElement.Value);
                TvStreamInfo currenTvStreamInfo = new TvStreamInfo()
                {
                    channelName = xElement.Value,
                    channelId = int.Parse(xElement.Attribute(XName.Get("id")).Value),
                };
                IEnumerable<XElement> currChanelShows = programmsNodes
                    .Where(elem =>
                        int.Parse(elem.Attribute(XName.Get("channel")).Value) == currenTvStreamInfo.channelId)
                    .ToArray();
                currenTvStreamInfo.records = new TvShow[currChanelShows.Count()];
                int i = 0;
                foreach (XElement currChanelShow in currChanelShows)
                {
                    currenTvStreamInfo.records[i] = new TvShow()
                    {
                        startTime = ParseTime(currChanelShow.Attribute(XName.Get("start")).Value),
                        endTime = ParseTime(currChanelShow.Attribute(XName.Get("stop")).Value)
                    };
                    foreach (XElement xNode in currChanelShow.Nodes())
                    {
                        switch (xNode.Name.LocalName)
                        {
                            case "desc":
                                currenTvStreamInfo.records[i].description = xNode.Value;
                                break;
                            case "title":
                                currenTvStreamInfo.records[i].title = xNode.Value;
                                break;
                            case "category":
                                currenTvStreamInfo.records[i].type = TvShow.GetTypeByString(xNode.Value);
                                break;
                        }
                    }
                    i++;
                }
                channels.Add(currenTvStreamInfo);
            }

            Console.WriteLine();
            return channels.ToArray();
        }

        private static readonly Regex timeParse = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})");
        private static DateTime ParseTime(string source)
        {
            Match match = timeParse.Match(source);
            return new DateTime
            (
                int.Parse(match.Groups["year"].Value),
                int.Parse(match.Groups["month"].Value),
                int.Parse(match.Groups["day"].Value),
                int.Parse(match.Groups["hour"].Value),
                int.Parse(match.Groups["minute"].Value),
                int.Parse(match.Groups["second"].Value)
            );
        }
    }
}
