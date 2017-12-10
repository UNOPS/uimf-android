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
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Core;

	[Input(Type = "dropdown")]
	public class DropdownInput : IInputManager
	{
		private Spinner Spinner { get; set; }
		private IList<DropdownItem> Items { get; set; }
		private InputFieldMetadata metadata;

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
		{
		    this.Spinner = new Spinner(Application.Context);
		    this.Items = inputCustomProperties.GetCustomProperty<IList<DropdownItem>>("items");
		    this.Items.Insert(0, new DropdownItem
		    {
		        Label = "",
		        Value = ""
		    });
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(Application.Context, Android.Resource.Layout.SimpleSpinnerItem, this.Items.Select(a => a.Label).ToArray());
			this.Spinner.Adapter = adapter;
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
	}
}