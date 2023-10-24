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
            var divBlock = sideBar.QuerySelectorAll("div.side-block")
                .Where(x => x.ClassList.Count() == 1)
                .First();
            List<SCPMenuItem> scpMenuItems = new List<SCPMenuItem>();
            for (int i = 1; i < 8; i+=2)
            {
                scpMenuItems.Add(new SCPMenuItem
                {
                    Name = divBlock.Children[i ].TextContent,
                    Series = ParseItem(divBlock.Children[i+1])
                });
            }
            return scpMenuItems;
        }

        public static List<SCPSeries> ParseItem(IElement listBlock)
        {
            List<SCPSeries> scpSeries = new List<SCPSeries>();
            var aContent = listBlock.QuerySelectorAll("a");
            foreach (var item in aContent)
            {
                scpSeries.Add(new SCPSeries
                {
                    SeriesName = item.TextContent,
                    Href = item.Attributes[0].Value
                });
            }

            return scpSeries;
        }
    }
}
