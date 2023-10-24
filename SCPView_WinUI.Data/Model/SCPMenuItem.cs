using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data.Model
{
    public class SCPMenuItem
    {
        public string Name { get; set; }
        public List<SCPSeries> Series { get; set; }

        public SCPMenuItem()
        {
            Name = string.Empty;
            Series = new List<SCPSeries>();
        }

        public SCPMenuItem(string name)
        {
            Name = name;
            Series = new List<SCPSeries>();
        }
    }
}
