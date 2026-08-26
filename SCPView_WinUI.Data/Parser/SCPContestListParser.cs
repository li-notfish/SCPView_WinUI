using AngleSharp.Html.Parser;
using SCPView_WinUI.Data.Model;
using System.Collections.Generic;

namespace SCPView_WinUI.Data.Parser
{
    public class SCPContestListParser
    {
        public static List<SCPContestItem> Parse(string body)
        {
            var items = new List<SCPContestItem>();
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(body);

            var liElements = doc.QuerySelectorAll("div.list-pages-box ul > li");
            foreach (var li in liElements)
            {
                var titleLink = li.QuerySelector("strong > a[href]");
                if (titleLink == null) continue;

                var authorLink = li.QuerySelector("span.printuser > a:last-child");

                string href = titleLink.GetAttribute("href") ?? "";
                if (href.StartsWith("/")) href = SCPUrl.REFERER + href;

                items.Add(new SCPContestItem
                {
                    Title = titleLink.TextContent.Trim(),
                    Href = href,
                    Author = authorLink?.TextContent.Trim() ?? "未知作者"
                });
            }

            return items;
        }
    }
}
