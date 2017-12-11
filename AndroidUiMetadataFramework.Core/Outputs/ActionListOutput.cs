namespace AndroidUiMetadataFramework.Core.Outputs
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Graphics;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;

    [Output(Type = "action-list")]
    public class ActionListOutput : IOutputManager
    {
        private LinearLayout OutputView { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            this.OutputView = new LinearLayout(Application.Context)
            {
                Orientation = Orientation.Vertical
            };
            var actions = value.CastTObject<ActionList>();

            if (actions != null)
            {
                foreach (var btn in actions.Actions)
                {
                    var button = this.InitializeActionButton(btn, myFormHandler);
                    this.OutputView.AddView(button, this.OutputView.MatchParentWrapContent());
                }
            }

            return this.OutputView;
        }

        public Button InitializeActionButton(FormLink btn, MyFormHandler myFormHandler)
        {
            var button = new Button(Application.Context) { Text = btn.Label };
            button.SetAllCaps(false);
            button.Click += async (sender, args) =>
            {
                var formMetadata = myFormHandler.GetFormMetadata(btn.Form);
                var action = btn.Action?? FormLinkActions.OpenModal;
                myFormHandler.FormWrapper.UpdateView(myFormHandler, formMetadata, btn.InputFieldValues, action);
            };
            return button;
        }
    }
}