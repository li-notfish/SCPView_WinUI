namespace SCPView_WinUI.Data.Model
{
    public class BlockQuoteContent
    {
        public string QuoteName { get; set; }
        public string QuoteContent { get; set;}

        public BlockQuoteContent()
        {
            QuoteName = string.Empty;
            QuoteContent = string.Empty;
        }

        public BlockQuoteContent(string quoteName, string quoteContent)
        {
            QuoteName = quoteName;
            QuoteContent = quoteContent;
        }
    }
}
