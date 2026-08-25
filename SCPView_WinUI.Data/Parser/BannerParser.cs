using AngleSharp.Html.Parser;
using SCPView_WinUI.Data.Model;
using System;

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
            if (bannerImage != null)
            {
                banner.BannerImagePath = bannerImage.GetAttribute("src");
            }
            return banner;
        }
    }
}
