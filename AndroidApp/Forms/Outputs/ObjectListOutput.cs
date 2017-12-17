namespace AndroidApp.Forms.Outputs
{
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Core;

    [Output(Type = "object-list")]
    public class ObjectListOutput : IOutputManager
    {
        private LinearLayout OutputList { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            this.OutputList = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
            var label = new TextView(Application.Context) { Text = outputField.Label };
            label.LayoutParameters = label.WrapContent();
            myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView", label);
            this.OutputList.AddView(label);
            var list = value.CastTObject<ObjectList<object>>();
            var listView = new ListView(Application.Context);
            var adapter = new ListCustomAdapter<object>(list.Items.ToList(), list.Metadata, myFormHandler);
            listView.Adapter = adapter;
            listView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, listView.GetListHeigth());
            myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("ListView", listView);
            this.OutputList.AddView(listView);
            return this.OutputList;
        }
    }

    public class ObjectList<T>
    {
        public IList<T> Items { get; set; }
        public IList<OutputFieldMetadata> Metadata { get; set; }
    }
}