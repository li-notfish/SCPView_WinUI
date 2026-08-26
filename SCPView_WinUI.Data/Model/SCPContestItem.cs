namespace SCPView_WinUI.Data.Model
{
    public class SCPContestItem
    {
        public string Title { get; set; }
        public string Href { get; set; }
        public string Author { get; set; }

        public SCPContestItem()
        {
            Title = string.Empty;
            Href = string.Empty;
            Author = string.Empty;
        }
    }
}
