namespace AndroidUiMetadataFramework.Core.Inputs
{
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Basic.Input.Typeahead;

    [Input(Type = "typeahead")]
    public class TypeaheadInput : IInputManager
    {
        private AutoCompleteTextView InputText { get; set; }
        private List<TypeaheadItem<object>> ItemsList { get; set; }

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.ItemsList = new List<TypeaheadItem<object>>();
            var customeSource = inputCustomProperties.GetCustomProperty<object>("source");
            var source = customeSource.GetTypeaheadSource(myFormHandler);           

            foreach (var item in source)
            {
                this.ItemsList.Add(item.CastTObject<TypeaheadItem<object>>());
            }

            var adapter = new ArrayAdapter<string>(Application.Context,
                Android.Resource.Layout.SimpleDropDownItem1Line, this.ItemsList.Select(a => a.Label).ToList());

            this.InputText = new AutoCompleteTextView(Application.Context)
            {
                Adapter = adapter,
                Threshold = 0
            };

            this.InputText.TextChanged += async (sender, args) =>
            {
                adapter.Clear();
                var query = args.Text.ToString().Trim();
                this.ItemsList = customeSource.GetTypeaheadSource(myFormHandler, new TypeaheadRequest<string> { Query = query })
                .Select(t => t.CastTObject<TypeaheadItem<object>>()).ToList();
                var data = this.ItemsList.Select(t => t.Label).ToList();

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