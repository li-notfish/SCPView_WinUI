using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data.Model
{
    public class SCPItem
    {
        public string Name { get; set; }
        public string SafeLevel { get; set; }
        public string SpecialMeasures { get; set; }
        public string Contents { get; set; }
        public List<CollapsibleContent> CollapsibleContents { get; set; }


        public SCPItem()
        {
            Name = string.Empty;
            SafeLevel = string.Empty;
            SpecialMeasures = string.Empty;
            Contents = string.Empty;
            CollapsibleContents = new List<CollapsibleContent>();
        }

        public SCPItem(string name,string safeLevel, string contents,List<BlockQuoteContent> blockQuoteContents , List<CollapsibleContent> collapsibleContents)
        {
            this.Name = name;
            this.SafeLevel = safeLevel;
            this.Contents = contents;
            CollapsibleContents = collapsibleContents;
        }
    }
}
