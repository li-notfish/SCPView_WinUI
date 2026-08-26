using AngleSharp.Html.Parser;
using SCPView_WinUI.Data.Model;
using System.Collections.Generic;

namespace SCPView_WinUI.Data.Parser
{
    public class SCPContestListParser
    {
        public static SCPContestData Parse(string body)
        {
            var data = new SCPContestData();
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(body);

            var toc0 = doc.QuerySelector("#toc0");
            if (toc0 != null)
            {
                data.Title = toc0.TextContent.Trim();
            }

            var blockquote = doc.QuerySelector("blockquote");
            if (blockquote != null)
            {
                data.Description = blockquote.TextContent.Trim();
            }

            var liElements = doc.QuerySelectorAll("div.list-pages-box ul > li");
            foreach (var li in liElements)
            {
                var titleLink = li.QuerySelector("strong > a[href]");
                if (titleLink == null) continue;

                var authorLink = li.QuerySelector("span.printuser > a:last-child");

                string href = titleLink.GetAttribute("href") ?? "";
                if (href.StartsWith("/")) href = SCPUrl.REFERER + href;

                data.Items.Add(new SCPContestItem
                {
                    Title = titleLink.TextContent.Trim(),
                    Href = href,
                    Author = authorLink?.TextContent.Trim() ?? "未知作者"
                });
            }

            return data;
        }
    }
}
