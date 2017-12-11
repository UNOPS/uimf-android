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
        private object CustomeSource { get; set; }
        private MyFormHandler MyFormHandler { get; set; }

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.ItemsList = new List<TypeaheadItem<object>>();
            this.CustomeSource = inputCustomProperties.GetCustomProperty<object>("source");
            this.MyFormHandler = myFormHandler;
 
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
                this.ItemsList = this.CustomeSource.GetTypeaheadSource(myFormHandler, new TypeaheadRequest<object> { Query = query })
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
            return new TypeaheadItem<object>
            {
                Value = null
            }; 
        }

        public void SetValue(object value)
        {
            var typeaheadValue = value.CastTObject<TypeaheadItem<object>>();
            if (typeaheadValue == null)
            {
                return;
            }
            var ids = new ValueList<object>
            {
                Items = new List<object> { typeaheadValue.Value }
            };
            var selectedItems = this.CustomeSource.GetTypeaheadSource(this.MyFormHandler,
                    new TypeaheadRequest<object> { Ids = ids })
                .Select(t => t.CastTObject<TypeaheadItem<object>>()).ToList();

            this.InputText.Text = selectedItems.SingleOrDefault()?.Label;
        }
    }
}