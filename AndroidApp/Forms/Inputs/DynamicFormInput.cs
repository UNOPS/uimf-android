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
                    return null;
                }
                
                inputManager.Input.Value = value?.ToString();
                var dropdownValue = value as DropdownValue<string>;
                if (dropdownValue != null)
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
                this.Layout.AddView(label, this.Layout.MatchParentWrapContent());

                var manager = this.FormHandler.InputManagerCollection.GetManager(input.Type);

                var view = manager.GetView(input.CustomProperties, this.FormHandler);
                if (input.Value != null)
                {
                    manager.SetValue(input.Value);
                }
                this.InputsManager.Add(new FormInputManager(input, manager));
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
        public FormInputManager(InputItem input, IInputManager manager)
        {
            this.Input = input;
            this.Manager = manager;
        }

        public InputItem Input { get; set; }
        public IInputManager Manager { get; set; }
    }
}