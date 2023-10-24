using AngleSharp.Html.Parser;
using SCPView_WinUI.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data.Parser
{
    public static class BannerParser
    {
        public static SCPBanner Parser(string body)
        {
            SCPBanner banner = new SCPBanner();

            var parser = new HtmlParser();
            
            var doc = parser.ParseDocument(body);

            var bannerImage = doc.QuerySelector("div.summer-contest-banner > a > img");

            banner.BannerImagePath = bannerImage.GetAttribute("src");

            return banner;
        }
    }
}
