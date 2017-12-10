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
            this.OutputList.AddView(label, this.OutputList.MatchParentWrapContent());
            var list = value.CastTObject<ObjectList<object>>();
            var listView = new ListView(Application.Context);
            listView.SetPadding(10, 0, 10, 0);
            listView.FastScrollEnabled = true;
            var adapter = new ListCustomAdapter<object>(list.Items.ToList(), list.Metadata, myFormHandler);
            listView.Adapter = adapter;

            var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, listView.GetListHeigth());
            this.OutputList.AddView(listView, param);
            return this.OutputList;
        }
    }

    public class ObjectList<T>
    {
        public IList<T> Items { get; set; }
        public IList<OutputFieldMetadata> Metadata { get; set; }
    }
}