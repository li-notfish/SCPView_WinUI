using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using SCPView_WinUI.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data.Parser
{
    public class SCPContentParser
    {
        public static async Task<SCPItem> ParseAsync(string body)
        {
            SCPItem item = new SCPItem();
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(body);

            var titleElement = doc.QuerySelector("div#page-title");
            if (titleElement == null) return item;
            item.Name = titleElement.TextContent.Replace("\n", "").Trim();

            var divContent = doc.QuerySelector("div#page-content");
            if (divContent == null) return item;

            item.PageType = DetectPageType(doc, divContent);

            switch (item.PageType)
            {
                case SCPPageType.Hub:
                    ParseHub(divContent, ref item);
                    break;
                case SCPPageType.Embedded:
                    await ParseEmbeddedAsync(divContent, item);
                    break;
                case SCPPageType.Complex:
                    ParseComplex(divContent, ref item);
                    break;
                case SCPPageType.LongNarrative:
                    ParseLongNarrative(divContent, ref item);
                    break;
                default:
                    ParseStandard(divContent, ref item);
                    break;
            }

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

        private static SCPPageType DetectPageType(IHtmlDocument doc, IElement divContent)
        {
            if (divContent.QuerySelector("div.content-panel.standalone.series") != null)
                return SCPPageType.Hub;

            var iframes = divContent.QuerySelectorAll("iframe");
            var pContent = divContent.QuerySelectorAll(":scope > p,ul,:scope > blockquote");
            if (iframes.Length > 0 && pContent.Count() <= 1)
                return SCPPageType.Embedded;

            if (divContent.QuerySelector("div.expoblock,yui-navset,.listblock") != null)
                return SCPPageType.Complex;

            bool hasStandardSections = false;
            foreach (var el in pContent)
            {
                string text = el.TextContent;
                if (text.Contains("特殊收容措施：") || text.Contains("描述："))
                {
                    hasStandardSections = true;
                    break;
                }
            }
            if (hasStandardSections)
                return SCPPageType.Standard;

            if (pContent.Count() > 15)
                return SCPPageType.LongNarrative;

            return SCPPageType.Standard;
        }

        private static void ParseStandard(IElement divContent, ref SCPItem item)
        {
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
        }

        private static SCPItem ParseStandard(IElement divContent, string name)
        {
            SCPItem item = new SCPItem();
            item.Name = name;
            item.PageType = SCPPageType.Standard;
            ParseStandard(divContent, ref item);
            return item;
        }

        private static void ParseHub(IElement divContent, ref SCPItem item)
        {
            var contentPanels = divContent.QuerySelectorAll("div.content-panel.standalone.series");
            if (contentPanels.Length == 0) return;

            foreach (var contentPanel in contentPanels)
            {
                var links = contentPanel.QuerySelectorAll("a");
                foreach (var link in links)
                {
                    string href = link.GetAttribute("href") ?? "";
                    string name = link.TextContent.Trim();
                    if (string.IsNullOrEmpty(href) || string.IsNullOrEmpty(name)) continue;
                    if (href.StartsWith("/")) href = SCPUrl.REFERER + href;

                    item.HubLinks.Add(new SCPItemList
                    {
                        Href = href,
                        HrefName = name,
                        Name = name
                    });
                }
            }

            var collapsibleContent = divContent.QuerySelectorAll("div.collapsible-block-content");
            GetCollapsibleContent(ref item, collapsibleContent);
        }

        private static async Task ParseEmbeddedAsync(IElement divContent, SCPItem item)
        {
            var iframe = divContent.QuerySelector("iframe");
            if (iframe != null)
            {
                string src = iframe.GetAttribute("src") ?? "";
                if (!string.IsNullOrEmpty(src))
                {
                    try
                    {
                        if (src.StartsWith("//")) src = "https:" + src;
                        else if (src.StartsWith("/")) src = SCPUrl.REFERER + src;

                        var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
                        var iframeDoc = await context.OpenAsync(src) as IHtmlDocument;
                        if (iframeDoc != null)
                        {
                            var iframeContent = iframeDoc.QuerySelector("div#page-content");
                            if (iframeContent != null)
                            {
                                var parsed = ParseStandard(iframeContent, item.Name);
                                item.SafeLevel = parsed.SafeLevel;
                                item.SpecialMeasures = parsed.SpecialMeasures;
                                item.Contents = parsed.Contents;
                                item.CollapsibleContents = parsed.CollapsibleContents;
                                item.BlockQuoteContents = parsed.BlockQuoteContents;
                                item.ImageUrls = parsed.ImageUrls;
                                item.Tables = parsed.Tables;
                                item.Footnotes = parsed.Footnotes;
                                return;
                            }
                        }
                    }
                    catch { }
                    item.Contents = src;
                }
            }

            var pContent = divContent.QuerySelectorAll(":scope > p");
            if (pContent.Count() > 0)
            {
                var descBuilder = new StringBuilder();
                foreach (var p in pContent)
                {
                    descBuilder.AppendLine(p.TextContent.Trim());
                }
                item.SpecialMeasures = descBuilder.ToString().Trim();
            }
        }

        private static void ParseComplex(IElement divContent, ref SCPItem item)
        {
            var pContent = divContent.QuerySelectorAll(":scope > p,ul,:scope > blockquote");
            GetPContent(ref item, pContent);

            var expoblocks = divContent.QuerySelectorAll("div.expoblock");
            foreach (var block in expoblocks)
            {
                string text = block.TextContent.Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    item.CollapsibleContents.Add(new CollapsibleContent
                    {
                        Name = "展开内容",
                        Content = text
                    });
                }
            }

            var listblocks = divContent.QuerySelectorAll(".listblock");
            foreach (var block in listblocks)
            {
                var items = block.QuerySelectorAll("li");
                foreach (var li in items)
                {
                    string text = li.TextContent.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        item.Contents += text + "\n";
                    }
                }
            }

            var collapsibleContent = divContent.QuerySelectorAll("div.collapsible-block-content");
            GetCollapsibleContent(ref item, collapsibleContent);

            var images = divContent.QuerySelectorAll("img");
            item.ImageUrls = images
                .Select(img => img.GetAttribute("src"))
                .Where(src => !string.IsNullOrEmpty(src))
                .ToList();

            var tables = divContent.QuerySelectorAll("table");
            item.Tables = tables.Select(t => t.TextContent.Trim()).ToList();

            item.Contents = item.Contents.Trim();
        }

        private static void ParseLongNarrative(IElement divContent, ref SCPItem item)
        {
            var pContent = divContent.QuerySelectorAll(":scope > p,ul,:scope > blockquote");
            GetPContent(ref item, pContent);

            var footnotes = divContent.QuerySelectorAll("sup > a,div.footnotes-footer");
            int fnId = 1;
            foreach (var fn in footnotes)
            {
                string text = fn.TextContent.Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    item.Footnotes.Add(new SCPFootnote(fnId++, text));
                }
            }

            var collapsibleContent = divContent.QuerySelectorAll("div.collapsible-block-content");
            GetCollapsibleContent(ref item, collapsibleContent);

            var images = divContent.QuerySelectorAll("img");
            item.ImageUrls = images
                .Select(img => img.GetAttribute("src"))
                .Where(src => !string.IsNullOrEmpty(src))
                .ToList();

            var tables = divContent.QuerySelectorAll("table");
            item.Tables = tables.Select(t => t.TextContent.Trim()).ToList();
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
