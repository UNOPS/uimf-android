namespace AndroidUiMetadataFramework.Core.Outputs
{
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Graphics;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;

    [Output(Type = "tabstrip")]
    public class TabstripOutput : IOutputManager
    {
        private GridLayout Layout { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            this.Layout = new GridLayout(Application.Context);
            var tabstrip = value.CastTObject<Tabstrip>();
            var currentTab = tabstrip.Tabs.SingleOrDefault(a => a.Form == tabstrip.CurrentTab);
            var columnIndex = 0;
            var rowIndex = 0;
            foreach (var tab in tabstrip.Tabs)
            {
                if (tab != null)
                {
                    var tv = new TextView(Application.Context)
                    {
                        Text = tab.Label
                    };
                    if (tab == currentTab)
                    {
                        tv.SetBackgroundResource(Resource.Drawable.TabBorders);
                        tv.SetTextColor(Color.White);
                    }

                    tv.SetPadding(10, 5, 10, 5);
                    tv.Click += async (sender, args) =>
                    {
                        var metadata = myFormHandler.GetFormMetadata(tab.Form);

                        myFormHandler.FormWrapper.UpdateView(myFormHandler, metadata, tab.InputFieldValues);
                    };
                    var row = GridLayout.InvokeSpec(rowIndex);
                    var column = GridLayout.InvokeSpec(columnIndex);
                    var param = new GridLayout.LayoutParams(row, column);

                    this.Layout.AddView(tv, param);
                    columnIndex++;
                    if (columnIndex % 3 == 0)
                    {
                        rowIndex++;
                        columnIndex = 0;
                    }
                }
            }
            return this.Layout;
        }
    }
}