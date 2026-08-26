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

            var bannerDiv = doc.QuerySelector("div.summercontest");
            if (bannerDiv == null) return banner;

            var link = bannerDiv.QuerySelector("a");
            if (link != null)
            {
                string href = link.GetAttribute("href") ?? "";
                if (!string.IsNullOrEmpty(href))
                {
                    if (href.StartsWith("/")) href = SCPUrl.REFERER + href;
                    banner.BannerLink = href;
                }
            }

            var bannerImage = bannerDiv.QuerySelector("img");
            if (bannerImage != null)
            {
                banner.BannerImagePath = bannerImage.GetAttribute("src") ?? "";
            }

            return banner;
        }
    }
}
