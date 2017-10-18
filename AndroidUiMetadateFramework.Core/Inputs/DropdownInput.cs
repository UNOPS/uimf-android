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
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Core;

	[Input(Type = "dropdown")]
	public class DropdownInput : IInputManager
	{
		private Spinner Spinner { get; set; }
		private IList<DropdownItem> Items { get; set; }
		private InputFieldMetadata metadata;

		public View GetView(object inputCustomProperties)
		{
			this.Spinner = new Spinner(Application.Context);
			DropdownProperties list = inputCustomProperties.CastTObject<DropdownProperties>();			
			this.Items = list.Items;
			ArrayAdapter<string> adapter = new ArrayAdapter<string>(Application.Context, Android.Resource.Layout.SimpleSpinnerItem, this.Items.Select(a => a.Label).ToArray());
			this.Spinner.Adapter = adapter;
			return this.Spinner;
		}

		public object GetValue()
		{
			var selectedPosition = this.Spinner.SelectedItemPosition;
			var value = new DropdownValue<string>(this.Items[selectedPosition].Value);
			return value;
		}

		public void SetValue(object value)
		{
			var strValue = value.CastTObject<string>();
			DropdownItem selectedItem = this.Items.FirstOrDefault(a => a.Value == strValue);

			if (selectedItem != null)
			{
				var selectedPosition = this.Items.IndexOf(selectedItem);
				this.Spinner.SetSelection(selectedPosition);
			}
		}
	}
}