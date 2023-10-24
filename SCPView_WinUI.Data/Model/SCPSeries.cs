using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data.Model
{
    public class SCPSeries
    {
        public string Href { get; set; }
        public string SeriesName { get; set; }

        public SCPSeries()
        {
            Href = string.Empty;
            SeriesName = string.Empty;
        }

        public SCPSeries(string href, string seriesName)
        {
            this.Href = href;
            this.SeriesName = seriesName;
        }
    }
}
