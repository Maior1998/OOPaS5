using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OOPaS5.Parser
{
    class XmlEGP :EGP
    {
        protected override TvStreamInfo[] ProcessData(string source)
        {
            XDocument doc = XDocument.Parse(source);
            XElement nodePointer = doc.Root.FirstNode as XElement;
            List<TvStreamInfo> channels = new List<TvStreamInfo>();
            while (nodePointer != null)
            {
                if (nodePointer.Name != "channel")
                {
                    nodePointer = nodePointer.NextNode as XElement;
                    continue;
                }
                Console.WriteLine($"{nodePointer.Name} - {nodePointer.Value}");
                channels.Add(new TvStreamInfo() { channelName = nodePointer.Value });
                nodePointer = nodePointer.NextNode as XElement;
            }
            TvStreamInfo[] res = channels.ToArray();

            return channels.ToArray();
        }

    }
}
