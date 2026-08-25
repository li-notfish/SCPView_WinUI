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
        public List<BlockQuoteContent> BlockQuoteContents { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<string> Tables { get; set; }
        public SCPPageType PageType { get; set; }
        public List<SCPItemList> HubLinks { get; set; }
        public List<SCPFootnote> Footnotes { get; set; }

        public SCPItem()
        {
            Name = string.Empty;
            SafeLevel = string.Empty;
            SpecialMeasures = string.Empty;
            Contents = string.Empty;
            CollapsibleContents = new List<CollapsibleContent>();
            BlockQuoteContents = new List<BlockQuoteContent>();
            ImageUrls = new List<string>();
            Tables = new List<string>();
            PageType = SCPPageType.Standard;
            HubLinks = new List<SCPItemList>();
            Footnotes = new List<SCPFootnote>();
        }
    }
}
