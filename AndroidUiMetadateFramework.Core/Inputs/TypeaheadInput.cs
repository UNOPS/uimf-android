namespace AndroidUiMetadateFramework.Core.Inputs
{
	using System.Collections.Generic;
	using System.Linq;
	using Android.App;
	using Android.Text;
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

		public View GetView(object inputCustomProperties)
		{
			this.ItemsList = new List<TypeaheadItem<object>>();
			var properties = inputCustomProperties.CastTObject<TypeaheadCustomProperties>();
			var source = (IEnumerable<object>)properties.Source;
			foreach (var item in source)
			{
				this.ItemsList.Add(item.CastTObject<TypeaheadItem<object>>());
			}

			ArrayAdapter<string> adapter = new ArrayAdapter<string>(Application.Context,
				Android.Resource.Layout.SimpleDropDownItem1Line, this.ItemsList.Select(a => a.Label).ToArray());

			this.InputText = new AutoCompleteTextView(Application.Context)
			{
				Adapter = adapter,
				Threshold = 0
			};
			return this.InputText;
		}

		public object GetValue()
		{
			return new TypeaheadItem<object>
			{
				Label = this.InputText.Text,
				Value = this.ItemsList.SingleOrDefault(a => a.Label.Equals(this.InputText.Text))?.Value
			};
		}

		public void SetValue(object value)
		{
			this.InputText.Text = value?.ToString();
		}
	}
}