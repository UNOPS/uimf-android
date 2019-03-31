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
    using App.Style;
    using UiMetadataFramework.Basic.Input;
    using UiMetadataFramework.Core;

    [Input(Type = "dropdown")]
    public class DropdownInput : IInputManager
    {
        private InputFieldMetadata metadata;
        private IList<DropdownItem> Items { get; set; }
        private Spinner Spinner { get; set; }

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.Spinner = new Spinner(Application.Context);
            this.Items = inputCustomProperties.GetCustomProperty<IList<DropdownItem>>("items");
            this.Items.Insert(0, new DropdownItem
            {
                Label = "",
                Value = ""
            });
            var adapter = new SpinnerCustomAdapter<string>(this.Items.Select(a => a.Label).ToList(), 
                myFormHandler.ManagersCollection.StyleRegister);
            this.Spinner.Adapter = adapter;
            myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("Spinner", this.Spinner);
            return this.Spinner;
        }

        public object GetValue()
        {
            if (this.Spinner.SelectedItemPosition != 0)
            {
                var selectedPosition = this.Spinner.SelectedItemPosition;
                var value = new DropdownValue<string>(this.Items[selectedPosition].Value);
                return value;
            }
            return null;
        }

		public bool IsValid(InputFieldMetadata inputFieldMetadata)
		{
			return !inputFieldMetadata.Required || !string.IsNullOrEmpty(this.GetValue()?.ToString());
		}

		public void SetValue(object value)
        {
            object dropdownValue;

            if (value is string)
            {
                dropdownValue = value;
            }
            else
            {
                dropdownValue = value.CastTObject<DropdownValue<object>>().Value;
            }

            var selectedItem = this.Items.FirstOrDefault(a => a.Value.Equals(dropdownValue));

            if (selectedItem != null)
            {
                var selectedPosition = this.Items.IndexOf(selectedItem);
                this.Spinner.SetSelection(selectedPosition);
            }
        }

        public class SpinnerCustomAdapter<T> : BaseAdapter<T>
        {
            public SpinnerCustomAdapter(List<T> objectList, StyleRegister StyleRegister)
            {
                this.ObjectList = objectList;
                this.StyleRegister = StyleRegister;
            }

            public override int Count => this.ObjectList.Count;

            public override T this[int position] => this.ObjectList[position];

            private List<T> ObjectList { get; }
            private StyleRegister StyleRegister { get; }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var textView = new TextView(Application.Context)
                {
                    Text = this.ObjectList[position]?.ToString()
                };
                StyleRegister.ApplyStyle("TextView SpinnerItem", textView);
                return textView;
            }
        }
    }
}