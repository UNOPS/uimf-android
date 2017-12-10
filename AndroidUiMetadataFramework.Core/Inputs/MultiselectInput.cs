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

    [Input(Type = "multiselect")]
    public class MultiselectInput : IInputManager
    {
        private MultiAutoCompleteTextView InputText { get; set; }
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
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(Application.Context,
                Android.Resource.Layout.SimpleDropDownItem1Line, this.ItemsList.Select(a => a.Label).ToList());

            this.InputText = new MultiAutoCompleteTextView(Application.Context)
            {
                Adapter = adapter,
                Threshold = 0
            };

            this.InputText.TextChanged += async (sender, args) =>
            {
                adapter.Clear();
                var query = args.Text.ToString().Split(',').Last().Trim();
                this.ItemsList = customeSource.GetTypeaheadSource(myFormHandler, new TypeaheadRequest<string> { Query = query })
                    .Select(t => t.CastTObject<TypeaheadItem<object>>()).ToList();
                var data = this.ItemsList.Select(t => t.Label).ToList();

                adapter.AddAll(data);

            };

            this.InputText.SetTokenizer(new MultiAutoCompleteTextView.CommaTokenizer());
            return this.InputText;
        }

        public object GetValue()
        {
            var items = this.InputText.Text.Split(',');
            var selectedItems = this.ItemsList.Where(a => items.Contains(a.Label)).Select(a => a.Value);

            return new MultiSelect<object>
            {
                Items = selectedItems.ToList()
            };
        }

        public void SetValue(object value)
        {
            var typeahead = value.CastTObject<MultiSelect<object>>();
            if (typeahead != null)
            {
                this.InputText.Text = string.Join(",", typeahead.Items);
            }
        }
    }
}