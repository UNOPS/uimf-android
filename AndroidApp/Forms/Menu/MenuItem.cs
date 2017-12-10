namespace AndroidApp.Forms.Menu
{
    using UiMetadataFramework.Core;

    public class MenuItem
    {
        public MenuItem(string label, FormMetadata formMetadata = null)
        {
            this.Label = label;
            this.FormMetadata = formMetadata;
        }
        public string Label { get; set; }
        public FormMetadata FormMetadata { get; set; }
    }
}