namespace SCPView_WinUI.Data.Model
{
    public class SCPFootnote
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public SCPFootnote()
        {
            Id = 0;
            Content = string.Empty;
        }

        public SCPFootnote(int id, string content)
        {
            Id = id;
            Content = content;
        }
    }
}
