namespace AndroidApp.Forms.Inputs
{
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using Humanizer;
    using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Core;

	[Input(Type = "dynamic-form")]
    public class DynamicFormInput : IInputManager
    {
        public MyFormHandler FormHandler { get; set; }
        public LinearLayout Layout { get; set; }
        private List<FormInputManager> InputsManager { get; set; }

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.Layout = new LinearLayout(Application.Context)
            {
                Orientation = Orientation.Vertical
            };
            this.FormHandler = myFormHandler;
            return this.Layout;
        }

		public bool IsValid(InputFieldMetadata inputFieldMetadata)
		{
			return !inputFieldMetadata.Required || string.IsNullOrEmpty(this.GetValue()?.ToString());
		}

		public object GetValue()
        {
            if (this.InputsManager == null)
            {
                return null;
            }

            var result = new DynamicForm
            {
                Inputs = new List<InputItem>()
            };
            foreach (var inputManager in this.InputsManager)
            {
                var value = inputManager.Manager.GetValue();
                if (value == null && inputManager.Input.Required)
                {
                    this.FormHandler.ManagersCollection.StyleRegister.ApplyStyle("ValidationError", inputManager.InputView);
                    return null;
                }
                
                inputManager.Input.Value = value?.ToString();
                if (value is DropdownValue<string> dropdownValue)
                {
                    inputManager.Input.Value = dropdownValue.Value;
                }
                   
                result.Inputs.Add(inputManager.Input);
            }
            return result;
        }

        public void SetValue(object value)
        {
            var form = value.CastTObject<DynamicForm>();
            this.InputsManager = new List<FormInputManager>();
            this.Layout.RemoveAllViews();
            var orderedInputs = form.Inputs.OrderBy(a => a.OrderIndex).ToList();
            this.InputsManager.Clear();

            foreach (var input in orderedInputs)
            {
                var label = new TextView(Application.Context) { Text = input.Label.Humanize(LetterCasing.Sentence) };
                label.LayoutParameters = label.WrapContent();
                this.FormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView", label);

                this.Layout.AddView(label, this.Layout.MatchParentWrapContent());

                var manager = this.FormHandler.ManagersCollection.InputManagerCollection.GetManager(input.Type);

                var view = manager.GetView(input.CustomProperties, this.FormHandler);
                if (input.Value != null)
                {
                    manager.SetValue(input.Value);
                }
                this.InputsManager.Add(new FormInputManager(input, manager, view));
                this.Layout.AddView(view, this.Layout.MatchParentWrapContent());
            }
        }
    }

    public class DynamicForm
    {
        public IList<InputItem> Inputs { get; set; }
    }

    public class InputItem
    {
        public IDictionary<string, object> CustomProperties { get; set; }
        public int FormId { get; set; }
        public string Id { get; set; }
        public int InputId { get; set; }
        public string Label { get; set; }
        public int OrderIndex { get; set; }
        public bool Required { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class FormInputManager
    {
        public FormInputManager(InputItem input, IInputManager manager, View view)
        {
            this.Input = input;
            this.Manager = manager;
            this.InputView = view;
        }

        public InputItem Input { get; set; }
        public View InputView { get; set; }
        public IInputManager Manager { get; set; }
    }
}