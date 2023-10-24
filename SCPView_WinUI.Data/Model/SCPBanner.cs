using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data.Model
{
    public class SCPBanner
    {
        public string BannerImagePath { get; set; }

        public SCPBanner()
        {
            BannerImagePath = string.Empty;
        }

        public SCPBanner(string bannerImagePath)
        {
            this.BannerImagePath = bannerImagePath;
        }
    }
}
