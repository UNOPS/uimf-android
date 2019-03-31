namespace AndroidUiMetadataFramework.Core.Outputs
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Core;

    [Output(Type = "table")]
    public class TableOutput : IOutputManager
    {
        private LinearLayout OutputList { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            this.OutputList = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
            if (!string.IsNullOrEmpty(outputField.Label))
            {
                var label = new TextView(Application.Context) { Text = outputField.Label + ": " };
                label.LayoutParameters = label.WrapContent();
                myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView", label);
                this.OutputList.AddView(label);
            }
            var list = value.CastTObject<IList<object>>();
            var listView = list.IntializeListView(outputField, myFormHandler);
            listView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, listView.GetListHeigth());
            myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("ListView", listView);
            this.OutputList.AddView(listView);
            return this.OutputList;
        }
    }
}