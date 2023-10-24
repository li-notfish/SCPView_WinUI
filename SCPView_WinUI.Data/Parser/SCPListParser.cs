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
    public class SCPListParser
    {
        public static Dictionary<string,List<SCPItemList>> Parse(string body)
        {
            Dictionary<string, List<SCPItemList>> scpList = new Dictionary<string, List<SCPItemList>>();
            var parser = new HtmlParser();
            var pageContent = parser.ParseDocument(body);
            bool isCNSite = pageContent.QuerySelector("div#page-title").InnerHtml.Contains("CN");
            var contentPanel = pageContent.QuerySelector("div#page-content > div.content-panel.standalone.series");
            string selectors = isCNSite ? "h1,ul" : ":scope > h1,ul";
            var h1Andul = contentPanel.QuerySelectorAll(selectors).Skip(2).ToCollection();
            foreach (var item in h1Andul)
            {
                IElement element = item;
                if(item.NextElementSibling is not null)
                {
                    element = item.NextElementSibling;
                    if (!element.TagName.Contains("UL") || item.TagName.Contains("UL"))
                    {
                        continue;
                    }
                    
                }
                else
                {
                    break;
                }
                var itemList = ParseList(element,isCNSite);
                scpList.TryAdd(item.TextContent,itemList);
            }
            return scpList;
        }

        public static List<SCPItemList> ParseList(IElement node, bool isCnSite)
        {
            List<SCPItemList> lists = new List<SCPItemList>();
            var scpNames = node.QuerySelectorAll("li");
            foreach (var scpName in scpNames)
            {
                var list = new SCPItemList();
                var tagA = scpName.QuerySelector("a");
                list.Href = tagA.Attributes[0].Value;
                list.HrefName = tagA.TextContent;
                var innerText = scpName.TextContent.Replace(list.HrefName, "").Replace("-","").TrimStart();
                list.Name = innerText;
                lists.Add(list);
            }
            return lists;
        }
    }
}
