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
        private object CustomeSource { get; set; }
        private MyFormHandler MyFormHandler { get; set; }

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.ItemsList = new List<TypeaheadItem<object>>();
            this.MyFormHandler = myFormHandler;
            this.CustomeSource = inputCustomProperties.GetCustomProperty<object>("source");
            var adapter = new ArrayAdapter<string>(Application.Context,
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
                this.ItemsList = this.CustomeSource.GetTypeaheadSource(myFormHandler, new TypeaheadRequest<object> { Query = query })
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
            if (typeahead == null)
            {
                return;
            }
            var ids = new ValueList<object>
            {
                Items = typeahead.Items
            };
            var selectedItems = this.CustomeSource.GetTypeaheadSource(this.MyFormHandler, 
                    new TypeaheadRequest<object> {Ids = ids})
                .Select(t => t.CastTObject<TypeaheadItem<object>>().Label).ToList();
            var result = "";
            foreach (var selected in selectedItems)
            {
                result += selected+",";
            }
            this.InputText.Text = result;
        }
    }
}