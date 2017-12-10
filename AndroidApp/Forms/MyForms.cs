namespace AndroidApp.Forms
{
    using System.Collections.Generic;
    using AndroidApp.Forms.Menu;
    using UiMetadataFramework.Core;

    public class MyForms
    {
        public IList<FormMetadata> Forms { get; set; }
        public IList<MenuMetadata> Menus { get; set; }
    }
}