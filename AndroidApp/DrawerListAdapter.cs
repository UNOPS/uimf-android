namespace AndroidApp
{
    using System.Collections.Generic;
    using Android.Graphics;
	using Android.Support.V7.Widget;
	using Android.Views;
	using Android.Widget;
    using AndroidApp.Forms.Menu;

    public class DrawerListAdapter : RecyclerView.Adapter
	{
		public DrawerListAdapter(IList<MenuItem> myDataSet, IOnItemClickListener listener)
		{
			this.Dataset = myDataSet;
			this.Listener = listener;
		}

		public override int ItemCount => this.Dataset.Count;
		private IList<MenuItem> Dataset { get; }
		private IOnItemClickListener Listener { get; }

		public override void OnBindViewHolder(RecyclerView.ViewHolder holderRaw, int position)
		{
			var holder = (ViewHolder)holderRaw;
		    holder.TextView.Text = this.Dataset[position].Label;
		    
            holder.TextView.Click += (sender, args) => { this.Listener.OnClick((View)sender, position); };
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var vi = LayoutInflater.From(parent.Context);
			var v = vi.Inflate(Resource.Layout.drawer_list_item, parent, false);
			var tv = v.FindViewById<TextView>(Android.Resource.Id.Text1);
		    tv.SetTextColor(this.Dataset[viewType].FormMetadata != null ? Color.Blue : Color.Black);

		    return new ViewHolder(tv);
		}

	    public override int GetItemViewType(int position)
	    {
	        //your code 
	        return position;
	    }

        //Associated Objects
        public interface IOnItemClickListener
		{
			void OnClick(View view, int position);
		}

		public class ViewHolder : RecyclerView.ViewHolder
		{
			public ViewHolder(TextView v) : base(v)
			{
				this.TextView = v;
			}

			public TextView TextView { get; set; }
		}
	}
}