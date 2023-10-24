using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data.Model
{
    public class CollapsibleContent
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public CollapsibleContent()
        {
            Name = string.Empty;
            Content = string.Empty;
        }

        public CollapsibleContent(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}
