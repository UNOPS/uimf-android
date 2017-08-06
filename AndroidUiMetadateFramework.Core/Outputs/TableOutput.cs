namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using Object = Java.Lang.Object;
	using String = Java.Lang.String;

	[Output(Type = "table")]
	public class TableOutput : IOutputManager
	{
		private LinearLayout OutputList { get; set; }

		public View GetView(string name, object value, FormActivity formActivity)
		{
			this.OutputList = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
			var label = new TextView(Application.Context) { Text = name };
			var labelparam = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
				ViewGroup.LayoutParams.WrapContent);
			this.OutputList.AddView(label, labelparam);
			var listView = new ListView(Application.Context);
			listView.SetPadding(10, 0, 10, 0);
			var list = ((IEnumerable<object>)value).ToList();
			var adapter = new CustomAdapter<object>(list.ToList());
			listView.FastScrollEnabled = true;
			listView.Adapter = adapter;
			var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, this.GetListHeigth(listView));
			this.OutputList.AddView(listView, param);
			return this.OutputList;
		}

		public int GetListHeigth(ListView listView)
		{
			var listAdapter = listView.Adapter;
			if (listAdapter == null)
			{
				return 0;
			}

			var totalHeight = 0;

			for (var i = 0; i < listAdapter.Count; i++)
			{
				var listItem = listAdapter.GetView(i, null, listView);
				listItem.Measure(0, 0);
				totalHeight += listItem.MeasuredHeight;
			}

			return totalHeight;
		}
	}

	public class CustomAdapter<T> : BaseAdapter<T>
	{
		public CustomAdapter(List<T> objectList)
		{
			this.ObjectList = objectList;
		}

		public override int Count => this.ObjectList.Count;

		public override T this[int position] => this.ObjectList[position];

		private List<T> ObjectList { get; }

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			PropertyInfo[] properties = null;
			var view = (ViewGroup)convertView;
			var viewHolder = new ViewHolder { Objects = new List<TextView>() };
			var type = this.ObjectList[position].GetType();
			bool isPremitive = type.IsPrimitive || type == typeof(Decimal) || type == typeof(String) || type == typeof(DateTime);

			if (!isPremitive)
			{
				properties = this.ObjectList[position].GetType().GetProperties();

			}
			if (view == null)
			{
				view = new LinearLayout(parent.Context) { Orientation = Orientation.Vertical };
				var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
					ViewGroup.LayoutParams.WrapContent);
				if (!isPremitive)
				{
					foreach (var unused in properties)
					{
						var textView = new TextView(parent.Context);
						view.AddView(textView, param);
						view.SetPadding(0, 10, 0, 10);
						viewHolder.Objects.Add(textView);
					}
				}
				else
				{
					var textView = new TextView(parent.Context);
					view.AddView(textView, param);
					view.SetPadding(0, 10, 0, 10);
					viewHolder.Objects.Add(textView);
				}
					
				view.LayoutParameters = param;
				view.Tag = viewHolder;
			}

			var holder = (ViewHolder)view.Tag;

			if (!isPremitive)
			{
				for (var i = 0; i < properties.Length; i++)
				{
					var property = properties[i];
					holder.Objects[i].Text = property.Name + ": " + property.GetValue(this.ObjectList[position], null);
				}
			}
			else
			{
				holder.Objects[0].Text = this.ObjectList[position]?.ToString();
			}
				
			return view;
		}
	}

	public class ViewHolder : Object
	{
		public List<TextView> Objects { get; set; }
	}
}