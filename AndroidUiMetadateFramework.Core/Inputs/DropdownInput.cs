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
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Core;

	[Input(Type = "dropdown")]
	public class DropdownInput : IInputManager
	{
		private Spinner Spinner { get; set; }
		private IList<DropdownItem> Items { get; set; }
		private InputFieldMetadata metadata;

		public View GetView(object inputCustomProperties, MyFormHandler myFormHandler)
		{
			//this.Spinner = new Spinner(myFormHandler.Activity, SpinnerMode.Dialog);
		    this.Spinner = new Spinner(Application.Context);
            DropdownProperties list = inputCustomProperties.CastTObject<DropdownProperties>();
            this.Items = list.Items;
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
			var dropdownValue = value.CastTObject<DropdownValue<object>>();
			DropdownItem selectedItem = this.Items.FirstOrDefault(a => a.Value.Equals(dropdownValue.Value));

			if (selectedItem != null)
			{
				var selectedPosition = this.Items.IndexOf(selectedItem);
				this.Spinner.SetSelection(selectedPosition);
			}
		}
	}
}