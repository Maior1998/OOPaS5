using System.Collections.Generic;
using AngleSharp.Html.Dom;

namespace OOPaS5.Parser
{
    public partial class HtmlEGP
    {
        public class HtmlDataProvider
        {
            public virtual object GetData()
            {
                string[] links = GetAllLinks(GetHtml(BaseUrl));
                Dictionary<int, string> res = new Dictionary<int, string>();
                foreach (string link in links)
                {
                    string buffer = GetHtml(BaseUrl + link);
                    IHtmlDocument doc = parser.ParseDocument(buffer);
                    int id = int.Parse(linksRegex.Match(doc.QuerySelector("link").GetAttribute("href")).Groups[1].Value);
                    res.Add(id, buffer);
                }
                return res;
            }
        }
    }
}