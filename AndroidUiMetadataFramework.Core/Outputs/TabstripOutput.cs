namespace AndroidUiMetadataFramework.Core.Outputs
{
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using Com.Google.Android.Flexbox;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;

    [Output(Type = "tabstrip")]
    public class TabstripOutput : IOutputManager
    {
        private FlexboxLayout Layout { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            this.Layout = new FlexboxLayout(Application.Context);
            var tabstrip = value.CastTObject<Tabstrip>();
            var currentTab = tabstrip.Tabs.SingleOrDefault(a => a.Form == tabstrip.CurrentTab);
            foreach (var tab in tabstrip.Tabs)
            {
                if (tab != null)
                {
                    var tv = new TextView(Application.Context)
                    {
                        Text = tab.Label
                    };

                    tv.Click += async (sender, args) =>
                    {
                        var metadata = myFormHandler.GetFormMetadata(tab.Form);

                        myFormHandler.FormWrapper.UpdateView(myFormHandler, new FormParameter(metadata, tab.InputFieldValues));
                    };

                    myFormHandler.ManagersCollection.StyleRegister.ApplyStyle(tab == currentTab ? "Tab CurrentTab" : "Tab", tv);
                    this.Layout.AddView(tv);
                    this.Layout.FlexDirection = FlexboxLayout.FlexDirectionRow;
                    this.Layout.FlexWrap = FlexboxLayout.FlexWrapWrap;
                }
            }
            return this.Layout;
        }
    }
}