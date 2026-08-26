namespace SCPView_WinUI.Data.Model
{
    public enum ContentBlockType
    {
        Text,
        Blockquote
    }

    public class ContentBlock
    {
        public ContentBlockType Type { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
