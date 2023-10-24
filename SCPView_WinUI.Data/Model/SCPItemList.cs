namespace SCPView_WinUI.Data.Model
{
    public class SCPItemList
    {
        public string Href { get; set; }
        public string HrefName { get; set; }
        public string Name { get; set; }

        public SCPItemList()
        {
            Href = string.Empty;
            Name = string.Empty;
            HrefName = string.Empty;
        }

        public SCPItemList(string href, string hrefName, string name)
        {
            this.Href = href;
            this.HrefName = hrefName;
            this.Name = name;
        }
    }
}
