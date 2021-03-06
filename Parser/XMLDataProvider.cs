﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;

namespace OOPaS5.Parser
{
    public partial class HtmlEGP
    {
        public class XMLDataProvider : HtmlDataProvider
        {
            protected object GatherData()
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

            protected TvStreamInfo[] ProcessData(object source)
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

            private static readonly Regex timeParse =
                new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})");

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

            public override object GetData()
            {
                TvStreamInfo[] raw = ProcessData(GatherData());
                Dictionary<int, string> res = new Dictionary<int, string>();

                
                foreach (TvStreamInfo tvStreamInfo in raw)
                {
                    XmlDocument doc = new XmlDocument();
                    XmlElement htmlRoot = doc.CreateElement("html");
                    doc.AppendChild(htmlRoot);
                    XmlElement body = doc.CreateElement("body");
                    htmlRoot.AppendChild(body);

                    XmlElement buffer = doc.CreateElement("div");
                    buffer.InnerText = tvStreamInfo.channelName;
                    buffer.SetAttribute("class", "hdr__inner");
                    body.AppendChild(buffer);

                    foreach (TvShow record in tvStreamInfo.records)
                    {
                        buffer = doc.CreateElement("div");
                        buffer.InnerText = record.startTime.ToShortTimeString();
                        buffer.SetAttribute("class", "p-programms__item__time");

                        body.AppendChild(buffer);

                        buffer = doc.CreateElement("div");
                        buffer.InnerText = record.title;
                        buffer.SetAttribute("class", "p-programms__item__name");
                        body.AppendChild(buffer);
                    }


                    
                    res.Add(tvStreamInfo.channelId,doc.OuterXml);
                }
                return res;
            }
        }
    }
}