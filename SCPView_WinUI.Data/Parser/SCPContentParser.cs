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

            RemoveJunkElements(divContent);

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

        private static List<IElement> CollectElements(IElement divContent)
        {
            var result = new List<IElement>();
            CollectFromNode(divContent, result, 0);
            return result;
        }

        private static void CollectFromNode(IElement node, List<IElement> result, int depth)
        {
            if (depth > 5) return;
            foreach (var child in node.Children)
            {
                string tag = child.TagName.ToUpperInvariant();
                if (tag == "P" || tag == "UL" || tag == "BLOCKQUOTE")
                {
                    result.Add(child);
                }
                else if (tag == "DIV")
                {
                    if (child.ClassList.Contains("footer-wikiwalk-nav"))
                        continue;

                    if (child.ClassList.Contains("yui-navset"))
                    {
                        var tabContent = child.QuerySelector("div.yui-content");
                        if (tabContent != null)
                            CollectFromNode(tabContent, result, depth + 1);
                        continue;
                    }

                    if (child.Id != null && child.Id.StartsWith("wiki-tab-"))
                    {
                        CollectFromNode(child, result, depth + 1);
                        continue;
                    }

                    bool hasContent = false;
                    foreach (var desc in child.Descendants<IElement>())
                    {
                        string dt = desc.TagName.ToUpperInvariant();
                        if (dt == "P" || dt == "UL" || dt == "BLOCKQUOTE")
                        {
                            hasContent = true;
                            break;
                        }
                    }
                    if (hasContent)
                        CollectFromNode(child, result, depth + 1);
                }
            }
        }

        private static SCPPageType DetectPageType(IHtmlDocument doc, IElement divContent)
        {
            if (divContent.QuerySelector("div.content-panel.standalone.series") != null)
                return SCPPageType.Hub;

            var iframes = divContent.QuerySelectorAll("iframe");
            var pContent = CollectElements(divContent);
            if (iframes.Length > 0 && pContent.Count <= 1)
                return SCPPageType.Embedded;

            if (HasStandardSections(pContent))
                return SCPPageType.Standard;

            var listblockPContent = divContent.QuerySelectorAll(".listblock > p").ToList();
            if (HasStandardSections(listblockPContent))
                return SCPPageType.Standard;

            if (divContent.QuerySelector("div.expoblock,.yui-navset,.listblock") != null)
                return SCPPageType.Complex;

            if (pContent.Count > 15)
                return SCPPageType.LongNarrative;

            return SCPPageType.Standard;
        }

        private static bool HasStandardSections(System.Collections.Generic.List<IElement> elements)
        {
            foreach (var el in elements)
            {
                string text = el.TextContent;
                if (text.Contains("特殊收容措施：") || text.Contains("描述："))
                    return true;
            }
            return false;
        }

        private static void RemoveJunkElements(IElement divContent)
        {
            var junk = divContent.QuerySelectorAll(
                "div.colmod-block.creditHorm,div.page-rate-widget-box,#u-credit-view,div.footer-wikiwalk-nav");
            foreach (var j in junk) j.Remove();

            var listBoxes = divContent.QuerySelectorAll("div.list-pages-box");
            foreach (var lb in listBoxes)
            {
                if (lb.QuerySelector("div.list-pages-item") == null)
                    lb.Remove();
            }
        }

        private static bool IsSubPageLink(IElement element)
        {
            if (!element.TagName.Equals("P", StringComparison.OrdinalIgnoreCase))
                return false;
            var links = element.QuerySelectorAll("a");
            if (links.Length != 1) return false;
            string text = links[0].TextContent.Trim();
            if (text.Contains("察看") || text.Contains("查看") ||
                text.Contains("繼續") || text.Contains("點此"))
                return true;
            return false;
        }

        private static void ParseStandard(IElement divContent, ref SCPItem item)
        {
            var pContent = CollectElements(divContent);

            var subPageLinks = pContent.Where(el => IsSubPageLink(el)).ToList();
            foreach (var linkEl in subPageLinks)
            {
                var a = linkEl.QuerySelector("a");
                if (a != null)
                {
                    string href = a.GetAttribute("href") ?? "";
                    if (!string.IsNullOrEmpty(href))
                    {
                        if (href.StartsWith("/")) href = SCPUrl.REFERER + href;
                        item.SubPageUrls.Add(href);
                    }
                }
                pContent.Remove(linkEl);
            }

            var images = divContent.QuerySelectorAll("img");
            item.ImageUrls = images
                .Select(img => img.GetAttribute("src"))
                .Where(src => !string.IsNullOrEmpty(src))
                .ToList();

            var tables = divContent.QuerySelectorAll("table");
            item.Tables = tables.Select(t => t.TextContent.Trim()).ToList();

            GetPContent(ref item, pContent);
            ExtractCompareBlocks(divContent, ref item);
        }

        private static void ExtractCompareBlocks(IElement divContent, ref SCPItem item)
        {
            var compares = divContent.QuerySelectorAll(".listblock .compare");
            foreach (var compare in compares)
            {
                var header = compare.QuerySelector(".header");
                string headerText = header?.TextContent.Trim() ?? "CSS Demo";

                var codeBlocks = compare.QuerySelectorAll("div.code pre");
                foreach (var codeBlock in codeBlocks)
                {
                    string text = codeBlock.TextContent.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        item.CollapsibleContents.Add(new CollapsibleContent
                        {
                            Name = headerText,
                            Content = text
                        });
                    }
                }

                var output = compare.QuerySelector(".output");
                if (output != null)
                {
                    string outputText = output.TextContent.Trim();
                    if (!string.IsNullOrEmpty(outputText))
                    {
                        item.CollapsibleContents.Add(new CollapsibleContent
                        {
                            Name = "输出",
                            Content = outputText
                        });
                    }
                }
            }
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
                    string codeName = link.TextContent.Trim();
                    if (string.IsNullOrEmpty(href) || string.IsNullOrEmpty(codeName)) continue;
                    if (href.StartsWith("/")) href = SCPUrl.REFERER + href;

                    string descriptiveName = "";
                    var parentP = link.Parent;
                    if (parentP != null)
                    {
                        string fullText = parentP.TextContent.Trim();
                        string linkText = link.TextContent.Trim();
                        int idx = fullText.LastIndexOf(linkText);
                        if (idx >= 0)
                        {
                            descriptiveName = fullText.Substring(idx + linkText.Length)
                                .Trim().TrimStart('-').Trim();
                        }
                    }

                    item.HubLinks.Add(new SCPItemList
                    {
                        Href = href,
                        HrefName = codeName,
                        Name = descriptiveName
                    });
                }
            }
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
            var pContent = CollectElements(divContent);
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
                        item.ContentBlocks.Add(new ContentBlock
                        {
                            Type = ContentBlockType.Text,
                            Text = text
                        });
                    }
                }
            }

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
            var contentBuilder = new StringBuilder();

            var h1Elements = divContent.QuerySelectorAll("h1");
            var h1Texts = new HashSet<string>();
            foreach (var h1 in h1Elements)
            {
                string text = h1.TextContent.Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    string h1Text = "【" + text + "】";
                    h1Texts.Add(h1Text);
                    contentBuilder.AppendLine(h1Text);
                    item.ContentBlocks.Add(new ContentBlock
                    {
                        Type = ContentBlockType.Text,
                        Text = h1Text
                    });
                }
            }

            var pContent = CollectElements(divContent);
            foreach (var el in pContent)
            {
                string text = el.TextContent.Trim();
                if (string.IsNullOrEmpty(text)) continue;

                if (h1Texts.Contains(text)) continue;

                if (el.TagName.Equals("BLOCKQUOTE", StringComparison.OrdinalIgnoreCase) ||
                    el.ClassList.Contains("blockquote"))
                {
                    item.BlockQuoteContents.Add(new BlockQuoteContent { QuoteContent = text });
                    item.ContentBlocks.Add(new ContentBlock
                    {
                        Type = ContentBlockType.Blockquote,
                        Text = text
                    });
                }
                else
                {
                    contentBuilder.AppendLine(text);
                    item.ContentBlocks.Add(new ContentBlock
                    {
                        Type = ContentBlockType.Text,
                        Text = text
                    });
                }
            }

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

            var images = divContent.QuerySelectorAll("img");
            item.ImageUrls = images
                .Select(img => img.GetAttribute("src"))
                .Where(src => !string.IsNullOrEmpty(src))
                .ToList();

            var tables = divContent.QuerySelectorAll("table");
            item.Tables = tables.Select(t => t.TextContent.Trim()).ToList();

            item.Contents = contentBuilder.ToString().Trim();
        }

        private static void GetPContent(ref SCPItem item, List<IElement> elements)
        {
            var section = ParseSection.Header;
            var proceduresBuilder = new StringBuilder();
            var descriptionBuilder = new StringBuilder();

            foreach (var element in elements)
            {
                string text = element.TextContent.Trim();

                if (element.TagName.Equals("BLOCKQUOTE", StringComparison.OrdinalIgnoreCase) ||
                    element.ClassList.Contains("blockquote"))
                {
                    var bq = new BlockQuoteContent();
                    bq.QuoteContent = text;
                    item.BlockQuoteContents.Add(bq);
                    item.ContentBlocks.Add(new ContentBlock
                    {
                        Type = ContentBlockType.Blockquote,
                        Text = text
                    });
                    continue;
                }

                if (text.Contains("项目等级："))
                {
                    int idx = text.IndexOf("项目等级：");
                    item.SafeLevel = text.Substring(idx + "项目等级：".Length).Trim();
                    section = ParseSection.Header;
                    item.ContentBlocks.Add(new ContentBlock
                    {
                        Type = ContentBlockType.Text,
                        Text = text
                    });
                    continue;
                }

                if (text.Contains("特殊收容措施："))
                {
                    section = ParseSection.Procedures;
                    int idx = text.IndexOf("特殊收容措施：");
                    string after = text.Substring(idx + "特殊收容措施：".Length).Trim();
                    if (!string.IsNullOrEmpty(after))
                        proceduresBuilder.AppendLine(after);
                    item.ContentBlocks.Add(new ContentBlock
                    {
                        Type = ContentBlockType.Text,
                        Text = text
                    });
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
                    item.ContentBlocks.Add(new ContentBlock
                    {
                        Type = ContentBlockType.Text,
                        Text = text
                    });
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

                item.ContentBlocks.Add(new ContentBlock
                {
                    Type = ContentBlockType.Text,
                    Text = text
                });
            }

            item.Contents = descriptionBuilder.ToString().Trim();
        }

        private enum ParseSection
        {
            Header,
            Procedures,
            Description
        }
    }
}
