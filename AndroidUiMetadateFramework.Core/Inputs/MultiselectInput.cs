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

	[Input(Type = "multiselect")]
	public class MultiselectInput : IInputManager
	{
		private MultiAutoCompleteTextView InputText { get; set; }
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

			this.InputText = new MultiAutoCompleteTextView(Application.Context)
			{
				Adapter = adapter,
				Threshold = 0
			};

			this.InputText.SetTokenizer(new MultiAutoCompleteTextView.CommaTokenizer());
			return this.InputText;
		}

		public object GetValue()
		{
			if (!string.IsNullOrEmpty(this.InputText.Text))
			{
				return new MultiSelect<string>
				{
					Items = this.InputText.Text.Split(',')
				};
			}
			else
			{
				return null;
			}
			
		}

		public void SetValue(object value)
		{
			//this.InputText.Text = value?.ToString();
		}

	}
}