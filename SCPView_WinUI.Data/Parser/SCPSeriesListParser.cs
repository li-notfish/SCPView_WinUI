using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using SCPView_WinUI.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data.Parser
{
    public class SCPSeriesListParser
    {
        public static List<SCPMenuItem> Parse(string body)
        {
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(body);
            var sideBar = doc.QuerySelector("#side-bar");
            if (sideBar == null) return new List<SCPMenuItem>();

            var divBlock = sideBar.QuerySelectorAll("div.side-block")
                .Where(x => x.ClassList.Count() == 1)
                .FirstOrDefault();
            if (divBlock == null) return new List<SCPMenuItem>();

            List<SCPMenuItem> scpMenuItems = new List<SCPMenuItem>();
            for (int i = 1; i < 8; i += 2)
            {
                if (i + 1 >= divBlock.Children.Length) break;
                var header = divBlock.Children[i];
                var listBlock = divBlock.Children[i + 1];
                scpMenuItems.Add(new SCPMenuItem
                {
                    Name = header.TextContent,
                    Series = ParseItem(listBlock)
                });
            }
            return scpMenuItems;
        }

        public static List<SCPSeries> ParseItem(IElement listBlock)
        {
            List<SCPSeries> scpSeries = new List<SCPSeries>();
            if (listBlock == null) return scpSeries;
            var aContent = listBlock.QuerySelectorAll("a");
            foreach (var item in aContent)
            {
                scpSeries.Add(new SCPSeries
                {
                    SeriesName = item.TextContent,
                    Href = item.GetAttribute("href") ?? ""
                });
            }
            return scpSeries;
        }
    }
}
