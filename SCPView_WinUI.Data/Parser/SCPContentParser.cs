using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using SCPView_WinUI.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data.Parser
{
    public class SCPContentParser
    {
        public static SCPItem Parse(string body)
        {
            SCPItem item = new SCPItem();
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(body);

            var titleElement = doc.QuerySelector("div#page-title");
            if (titleElement == null) return item;
            item.Name = titleElement.TextContent.Replace("\n", "").Trim();

            var divContent = doc.QuerySelector("div#page-content");
            if (divContent == null) return item;

            var pContent = divContent.QuerySelectorAll(":scope > p,ul,:scope > blockquote");
            if (pContent.Count() <= 1)
            {
                pContent = divContent.QuerySelectorAll("div.list-pages-item > p,ul,blockquote");
            }
            var collapsibleContent = divContent.QuerySelectorAll("div.collapsible-block-content");

            var images = divContent.QuerySelectorAll("img");
            item.ImageUrls = images
                .Select(img => img.GetAttribute("src"))
                .Where(src => !string.IsNullOrEmpty(src))
                .ToList();

            var tables = divContent.QuerySelectorAll("table");
            item.Tables = tables.Select(t => t.TextContent.Trim()).ToList();

            GetPContent(ref item, pContent);
            GetCollapsibleContent(ref item, collapsibleContent);
            return item;
        }

        public static async Task<List<SCPSeries>> ParseZZO(string body)
        {
            var item = new List<SCPSeries>();
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(body);
            var yuiContent = doc.QuerySelector("div.yui-content iframe");
            if (yuiContent == null) return item;
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            string src = yuiContent.GetAttribute("src");
            if (string.IsNullOrEmpty(src)) return item;
            doc = await context.OpenAsync(src) as IHtmlDocument;
            if (doc == null) return item;
            var lia = doc.QuerySelectorAll("li");
            foreach (var item1 in lia)
            {
                var a = item1.QuerySelector("a");
                if (a == null) continue;
                SCPSeries series = new SCPSeries();
                series.Href = a.GetAttribute("href");
                series.SeriesName = item1.TextContent;
                item.Add(series);
            }
            return item;
        }

        private static void GetPContent(ref SCPItem item, IHtmlCollection<IElement> elements)
        {
            var section = ParseSection.Header;
            var proceduresBuilder = new StringBuilder();
            var descriptionBuilder = new StringBuilder();

            foreach (var element in elements)
            {
                string text = element.TextContent.Trim();

                if (element.TagName.Equals("BLOCKQUOTE", StringComparison.OrdinalIgnoreCase))
                {
                    var bq = new BlockQuoteContent();
                    bq.QuoteContent = text;
                    item.BlockQuoteContents.Add(bq);
                    continue;
                }

                if (text.Contains("项目等级："))
                {
                    var span = element.QuerySelector("span");
                    item.SafeLevel = span != null ? span.TextContent.Trim() : text;
                    section = ParseSection.Header;
                    continue;
                }

                if (text.Contains("特殊收容措施："))
                {
                    section = ParseSection.Procedures;
                    int idx = text.IndexOf("特殊收容措施：");
                    string after = text.Substring(idx + "特殊收容措施：".Length).Trim();
                    if (!string.IsNullOrEmpty(after))
                        proceduresBuilder.AppendLine(after);
                    continue;
                }

                if (text.Contains("描述："))
                {
                    item.SpecialMeasures = proceduresBuilder.ToString().Trim();
                    proceduresBuilder.Clear();
                    section = ParseSection.Description;
                    int idx = text.IndexOf("描述：");
                    string after = text.Substring(idx + "描述：".Length).Trim();
                    if (!string.IsNullOrEmpty(after))
                        descriptionBuilder.AppendLine(after);
                    continue;
                }

                switch (section)
                {
                    case ParseSection.Procedures:
                        proceduresBuilder.AppendLine(text);
                        break;
                    case ParseSection.Description:
                        descriptionBuilder.AppendLine(text);
                        break;
                }
            }

            item.Contents = descriptionBuilder.ToString().Trim();
        }

        private static void GetCollapsibleContent(ref SCPItem item, IHtmlCollection<IElement> elements)
        {
            foreach (var element in elements)
            {
                if (element.TextContent.Contains("请按如下方式引用此页："))
                    break;

                var cbleContent = new CollapsibleContent();
                if (element.TextContent.Contains("附件"))
                {
                    cbleContent.Name = element.TextContent.Substring(0, 6).Replace("\n", "").Trim();
                }
                cbleContent.Content = element.TextContent + "\n";
                item.CollapsibleContents.Add(cbleContent);
            }
        }

        private enum ParseSection
        {
            Header,
            Procedures,
            Description
        }
    }
}
