namespace AndroidUiMetadataFramework.Core.Models
{
	using System.Collections.Generic;
	using Android.Views;
	using Object = Java.Lang.Object;

	public class ListViewHolder : Object
	{
		public List<View> Objects { get; set; }
	}
}