namespace AndroidUiMetadateFramework.Core.Inputs
{
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadateFramework.Core.Attributes;
    using AndroidUiMetadateFramework.Core.Managers;
    using AndroidUiMetadateFramework.Core.Models;
    using UiMetadataFramework.Basic.Input.Typeahead;

    [Input(Type = "typeahead")]
    public class TypeaheadInput : IInputManager
    {
        private AutoCompleteTextView InputText { get; set; }
        private List<TypeaheadItem<object>> ItemsList { get; set; }

        public View GetView(object inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.ItemsList = new List<TypeaheadItem<object>>();
            var properties = inputCustomProperties.CastTObject<TypeaheadCustomProperties>();
            var source = properties.GetTypeaheadSource(myFormHandler);           

            foreach (var item in source)
            {
                this.ItemsList.Add(item.CastTObject<TypeaheadItem<object>>());
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(Application.Context,
                Android.Resource.Layout.SimpleDropDownItem1Line, this.ItemsList.Select(a => a.Label).ToList<string>());

            this.InputText = new AutoCompleteTextView(Application.Context)
            {
                Adapter = adapter,
                Threshold = 0
            };


			this.InputText.TextChanged += async (sender, args) =>
			{
				adapter.Clear();
				var query = args.Text.ToString().Trim();

				this.ItemsList = properties.GetTypeaheadSource(myFormHandler, new TypeaheadRequest<string> { Query = query }).Select(t => t.CastTObject<TypeaheadItem<object>>()).ToList();
				var data = this.ItemsList.Select(t => t.Label).ToList<string>();

				adapter.AddAll(data);
			};


			return this.InputText;
        }

        public object GetValue()
        {
            if (!string.IsNullOrEmpty(this.InputText.Text))
            {
                return new TypeaheadItem<object>
                {
                    Label = this.InputText.Text,
                    Value = this.ItemsList.SingleOrDefault(a => a.Label.Equals(this.InputText.Text))?.Value
                };
            }
            return null;
        }

        public void SetValue(object value)
        {
            TypeaheadItem<object> typeaheadValue = value.CastTObject<TypeaheadItem<object>>();
            var label = this.ItemsList.SingleOrDefault(a => a.Value.Equals(typeaheadValue.Value))?.Label;
            this.InputText.Text = label;
        }
    }
}