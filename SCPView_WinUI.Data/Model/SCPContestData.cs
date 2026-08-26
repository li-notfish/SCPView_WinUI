using System.Collections.Generic;

namespace SCPView_WinUI.Data.Model
{
    public class SCPContestData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SCPContestItem> Items { get; set; }

        public SCPContestData()
        {
            Title = string.Empty;
            Description = string.Empty;
            Items = new List<SCPContestItem>();
        }
    }
}
