using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using SCPView_WinUI.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
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
            item.Name = doc.QuerySelector("div#page-title").TextContent.Replace("\n","").Trim();
            var divContent = doc.QuerySelector("div#page-content");
            var pContent = divContent.QuerySelectorAll(":scope > p,ul,blockquote");
            var collapsibleContent = divContent.QuerySelectorAll("div.collapsible-block-content");
            GetPContent(ref item, pContent);
            GetCollapsibleContent(ref item, collapsibleContent);
            return item;
        }

        public static async Task<List<SCPSeries>> ParseZZO(string body)
        {
            var item = new List<SCPSeries>();

            var parser = new HtmlParser();
            var doc = parser.ParseDocument("body");
            var yuiContent = doc.QuerySelector("div.yui-content iframe");
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            string src = yuiContent.GetAttribute("src");
            doc = await context.OpenAsync(src) as IHtmlDocument;
            var lia = doc.QuerySelectorAll("li");
            foreach (var item1 in lia)
            {
                SCPSeries series = new SCPSeries();
                series.Href = item1.QuerySelector("a").GetAttribute("href");
                series.SeriesName = item1.TextContent;
                item.Add(series);
            }
            return item;
        }

        private static void GetPContent(ref SCPItem item, IHtmlCollection<IElement> elements)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var element in elements)
            {
                if(element.TextContent.Contains("项目等级："))
                {
                    if(element.QuerySelector("span")  != null)
                    {
                        element.RemoveElement(element.QuerySelector("span"));
                    }

                    item.SafeLevel = element.TextContent;
                    stringBuilder.Clear();
                }

                if(element.TextContent.Contains("特殊收容措施："))
                {
                    stringBuilder.Clear();
                }

                if(element.TextContent.Contains("描述："))
                {
                    item.SpecialMeasures = stringBuilder.ToString();
                    stringBuilder.Clear();
                }

                if(elements.Last() == element)
                {
                    item.Contents = stringBuilder.ToString();
                    stringBuilder.Clear();
                }
                stringBuilder.AppendLine(element.TextContent);
            }
        }

        private static void GetCollapsibleContent(ref SCPItem item, IHtmlCollection<IElement> elements)
        {
            foreach (var element in elements)
            {
                var cbleContent = new CollapsibleContent();
                if(element.TextContent.Contains("附件"))
                {
                    cbleContent.Name = element.TextContent.Substring(0,6).Replace("\n","").Trim();
                }
                cbleContent.Content = element.TextContent + "\n";
                item.CollapsibleContents.Add(cbleContent);
            }
        }
    }
}
