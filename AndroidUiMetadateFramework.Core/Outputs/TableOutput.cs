namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System.Collections.Generic;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using UiMetadataFramework.Core;

	[Output(Type = "table")]
	public class TableOutput : IOutputManager
	{
		private LinearLayout OutputList { get; set; }

		public View GetView(OutputFieldMetadata outputField, object value, MyFormHandler myFormHandler, FormMetadata formMetadata, List<FormInputManager> inputsManager)
		{
			this.OutputList = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
			var label = new TextView(Application.Context) { Text = outputField.Label };
			this.OutputList.AddView(label, this.OutputList.MatchParentWrapContent());
			var list = value.CastTObject<IList<object>>();
			var listView = list.IntializeListView(outputField, myFormHandler);
			var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, listView.GetListHeigth());
			this.OutputList.AddView(listView, param);
			return this.OutputList;
		}
	}
}