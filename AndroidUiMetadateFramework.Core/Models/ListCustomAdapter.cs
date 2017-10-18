namespace AndroidUiMetadateFramework.Core.Models
{
	using System.Reflection;
	using System.Collections.Generic;
	using System.Linq;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Managers;
	using Java.Util;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using UiMetadataFramework.Basic.Response;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;

	public class ListCustomAdapter<T> : BaseAdapter<T>
	{
		public ListCustomAdapter(List<T> objectList, EnumerableOutputFieldProperties outputFieldProperty, MyFormHandler myFormHandler)
		{
			this.ObjectList = objectList;
			this.OutputFieldProperty = outputFieldProperty;
			this.MyFormHandler = myFormHandler;
			this.AllFormsMetadata = myFormHandler.AllFormsMetadata;
		}

		public override int Count => this.ObjectList.Count;

		public override T this[int position] => this.ObjectList[position];

		private List<T> ObjectList { get; }
		private EnumerableOutputFieldProperties OutputFieldProperty { get; }
		private MyFormHandler MyFormHandler { get; }
		private Dictionary<string, FormMetadata> AllFormsMetadata { get; }

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			PropertyInfo[] properties = null;
			var view = (ViewGroup)convertView;
			var viewHolder = new ListViewHolder { Objects = new List<View>() };
			//var type = this.ObjectList[position].GetType();

			//properties = this.ObjectList[position].GetType().GetProperties();

			if (view == null)
			{
				view = new LinearLayout(parent.Context) { Orientation = Orientation.Vertical };
				var param = view.MatchParentWrapContent();

					//foreach (var unused in properties)
					//{
					//	var textView = new TextView(parent.Context);
					//	view.AddView(textView, param);
					//	view.SetPadding(0, 10, 0, 10);
					//	viewHolder.Objects.Add(textView);
					//}

				var orderedOutputs = this.OutputFieldProperty.Columns.OrderBy(a => a.OrderIndex);

				foreach (var output in orderedOutputs)
				{
					if (!output.Hidden)
					{
						object value;
						if (this.ObjectList[position].GetType() == typeof(JObject))
						{
							var jsonObj = this.ObjectList[position] as JObject;
							value = jsonObj?.GetValue(output.Id.ToLower());
						}
						else
						{
							var propertyInfo = this.ObjectList[position].GetType().GetProperty(output.Id);
							value = propertyInfo?.GetValue(this.ObjectList[position], null);
						}
						if (value != null)
						{
							var manager = this.MyFormHandler.OutputManagerCollection.GetManager(output.Type);
							var outputView = manager.GetView(output, value, this.MyFormHandler, null, null);
							view.AddView(outputView, param);
							view.SetPadding(0, 10, 0, 10);
							viewHolder.Objects.Add(outputView);
						}
							
						}
				}

				//else
				//{
				//	var textView = new TextView(parent.Context);
				//	view.AddView(textView, param);
				//	view.SetPadding(0, 10, 0, 10);
				//	viewHolder.Objects.Add(textView);
				//}

				view.LayoutParameters = param;
				view.Tag = viewHolder;
			}

			//var holder = (ListViewHolder) view.Tag;

			//for (var i = 0; i < properties.Length; i++)
			//	{
			//		var property = properties[i];
			//	//	holder.Objects[i].Text = property.Name + ": " + property.GetValue(this.ObjectList[position], null);
			//	}
			//else
			//{
			//	holder.Objects[0].Text = this.ObjectList[position]?.ToString();
			//}

			return view;
		}
	}
}