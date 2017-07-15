namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System.Collections.Generic;
	using System.Linq;
	using Android.App;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using Java.Lang;

	[Output(Type = "table")]
	public class TableOutput : IOutputManager
	{
		private LinearLayout OutputList { get; set; }

		public View GetView(Activity activity, string name, object value, FormActivity formActivity)
		{
			this.OutputList = new LinearLayout(activity) { Orientation = Orientation.Vertical };
			var label = new TextView(activity) { Text = name };
			var labelparam = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
				ViewGroup.LayoutParams.WrapContent);
			this.OutputList.AddView(label, labelparam);
			var listView = new ListView(activity);
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
			var view = (ViewGroup)convertView;
			var viewHolder = new ViewHolder { Objects = new List<TextView>() };
			var properties = this.ObjectList[position].GetType().GetProperties();
			if (view == null)
			{
				view = new LinearLayout(parent.Context) { Orientation = Orientation.Vertical };
				var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
					ViewGroup.LayoutParams.WrapContent);
				foreach (var unused in properties)
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

			for (var i = 0; i < properties.Length; i++)
			{
				var property = properties[i];
				holder.Objects[i].Text = property.Name + ": " + property.GetValue(this.ObjectList[position], null);
			}
			return view;
		}
	}

	public class ViewHolder : Object
	{
		public List<TextView> Objects { get; set; }
	}
}