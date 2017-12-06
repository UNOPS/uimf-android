namespace AndroidUiMetadateFramework.Core.Models
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Android.App;
	using Android.Content;
	using Android.Views;
	using Android.Widget;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using UiMetadataFramework.Basic.Input.Typeahead;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;

    public static class Extension
	{
		public static T CastTObject<T>(this object obj)
		{
			if (obj.GetType() == typeof(JObject))
			{
				return JsonConvert.DeserializeObject<T>(obj.ToString());
			}
			if (obj.GetType() == typeof(JValue))
			{
				return ((JValue)obj).ToObject<T>();
			}
			if (obj.GetType() == typeof(JArray))
			{
				return ((JArray)obj).ToObject<T>();
			}
			return (T)obj;
		}

		public static int ConvertPixelsToDp(this int pixelValue)
		{
			var dp = (int)(pixelValue / Application.Context.Resources.DisplayMetrics.Density);
			return dp;
		}

		public static int GetListHeigth(this ListView listView)
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

		public static ListView IntializeListView(this IList<object> itemList, OutputFieldMetadata outputField, MyFormHandler myFormHandler)
		{
			var listView = new ListView(Application.Context);
			listView.SetPadding(10, 0, 10, 0);
			EnumerableOutputFieldProperties outputFieldProperty = outputField.CustomProperties.CastTObject<EnumerableOutputFieldProperties>();
			listView.FastScrollEnabled = true;
			listView.SetCustomAdapter(itemList, outputFieldProperty, myFormHandler);

			return listView;
		}

		public static LinearLayout.LayoutParams MatchParentWrapContent(this View view)
		{
			return new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
				ViewGroup.LayoutParams.WrapContent);
		}

		public static ListView Refresh(this ListView listView,
			IList<object> newItemList,
			OutputFieldMetadata outputField,
			MyFormHandler myFormHandler,
			Dictionary<string, FormMetadata> allFormsMetadata)
		{
			EnumerableOutputFieldProperties outputFieldProperty = outputField.CustomProperties.CastTObject<EnumerableOutputFieldProperties>();
			listView.SetCustomAdapter(newItemList, outputFieldProperty, myFormHandler);
			return listView;
		}

		public static int ScreenHeightDp(this Context context)
		{
			var metrics = Application.Context.Resources.DisplayMetrics;
			var heightInDp = metrics.HeightPixels.ConvertPixelsToDp();
			return heightInDp.ConvertPixelsToDp();
		}

		public static int ScreenWidthDp(this Context context)
		{
			var metrics = Application.Context.Resources.DisplayMetrics;
			var widthInDp = metrics.WidthPixels.ConvertPixelsToDp();
			return widthInDp.ConvertPixelsToDp();
		}

		public static void SetCustomAdapter(this ListView listView,
			IList<object> itemList,
			EnumerableOutputFieldProperties outputFieldProperty,
			MyFormHandler myFormHandler)
		{
			var adapter = new ListCustomAdapter<object>(itemList.ToList(), outputFieldProperty, myFormHandler);
			listView.Adapter = adapter;
		}

	    public static IEnumerable<object> GetTypeaheadSource(this TypeaheadCustomProperties customProperties, MyFormHandler myFormHandler)
	    {
	        if (customProperties.Source is string)
	        {
	            var list = new Dictionary<string, object> { { "query", "" } };
	            var obj = JsonConvert.SerializeObject(list);

                var dataSource = customProperties.Source.ToString();
	            var request = new InvokeForm.Request
	            {
	                Form = dataSource,
	                InputFieldValues = obj
                };
	            var result = Task.Run(
	                () => myFormHandler.InvokeFormAsync(new[] { request }, false));

	            var response = result.Result;
	            var typeahead = response[0].Data.CastTObject<TypeaheadResponse<object>>();
	            if (typeahead != null)
	            {
	                return typeahead.Items;
                }
	        }
	        else
	        {
	            return (IEnumerable<object>)customProperties.Source;

	        }
	        return new List<object>();
	    }
    }
}